#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Agents.Net
{
    /// <summary>
    /// This implementation implements the <see cref="IMessageBoard"/> interface for in-process agents.
    /// </summary>
    /// <remarks>
    /// This implementation guaranties the following qualities:
    /// <list type="bullet">
    /// <item>
    /// <term>parallel</term>
    /// <description>All agents are executed parallel.</description>
    /// </item>
    /// </list>
    /// </remarks>
    public sealed class MessageBoard : IDisposable, IMessageBoard
    {
        private readonly MessagePublisher publisher = new MessagePublisher();
        private bool disposed;

        /// <inheritdoc />
        public void Publish(Message message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (disposed)
            {
                return;
            }

            PublishAllMessages(message.HeadMessage);
        }

        /// <inheritdoc />
        public void Start()
        {
            Publish(new InitializeMessage());
        }

        private void PublishAllMessages(Message messageContainer)
        {
            publisher.Publish(messageContainer);
        }

        /// <inheritdoc />
        public void Register(params Agent[] agents)
        {
            if (disposed)
            {
                return;
            }

            foreach (Agent agent in agents)
            {
                RegisterDefinedConsumingMessages(agent);
                if (agent is InterceptorAgent interceptorAgent)
                {
                    RegisterDefinedInterceptingMessages(interceptorAgent);
                }
            }
        }

        private void RegisterDefinedInterceptingMessages(InterceptorAgent interceptorAgent)
        {
            IEnumerable<InterceptsAttribute> attributes = interceptorAgent.GetType()
                .GetCustomAttributes(typeof(InterceptsAttribute), true)
                .OfType<InterceptsAttribute>();
            foreach (InterceptsAttribute attribute in attributes)
            {
                publisher.RegisterInterceptor(attribute.MessageType, interceptorAgent);
            }
        }

        private void RegisterDefinedConsumingMessages(Agent agent)
        {
            IEnumerable<ConsumesAttribute> attributes = agent.GetType().GetCustomAttributes(typeof(ConsumesAttribute), true)
                                                             .OfType<ConsumesAttribute>().Where(c => !c.Implicitly);
            foreach (ConsumesAttribute attribute in attributes)
            {
                publisher.Register(attribute.MessageType, agent);
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            disposed = true;
            publisher.Dispose();
        }

        private class MessagePublisher : IDisposable
        {
            private readonly Dictionary<Type, List<Agent>> registeredAgents =
                new Dictionary<Type, List<Agent>>();

            private readonly Dictionary<Type, List<InterceptorAgent>> interceptorAgents = 
                new Dictionary<Type, List<InterceptorAgent>>();

            private readonly List<Agent> registeredMessageAgents = new List<Agent>();

            private readonly List<InterceptorAgent> registeredMessageInterceptors = new List<InterceptorAgent>();

            public void Register(Type trigger, Agent agent)
            {
                if (trigger == typeof(Message))
                {
                    registeredMessageAgents.Add(agent);
                }
                if (!registeredAgents.ContainsKey(trigger))
                {
                    registeredAgents.Add(trigger, new List<Agent>());
                }
                registeredAgents[trigger].Add(agent);
            }

            public void RegisterInterceptor(Type trigger, InterceptorAgent agent)
            {
                if (trigger == typeof(Message))
                {
                    registeredMessageInterceptors.Add(agent);
                }
                if (!interceptorAgents.ContainsKey(trigger))
                {
                    interceptorAgents.Add(trigger, new List<InterceptorAgent>());
                }
                interceptorAgents[trigger].Add(agent);
            }

            public void Publish(Message messageContainer)
            {
                List<InterceptorAgent> interceptors = registeredMessageInterceptors.Count > 0
                                                          ? new List<InterceptorAgent>(registeredMessageInterceptors)
                                                          : null;
                foreach (Message message in messageContainer.DescendantsAndSelf)
                {
                    if (!interceptorAgents.TryGetValue(message.MessageType, out List<InterceptorAgent> agents))
                    {
                        continue;
                    }

                    interceptors = interceptors ?? new List<InterceptorAgent>();

                    interceptors.AddRange(agents);
                }

                if (interceptors != null)
                {
                    Intercept();
                }
                else
                {
                    PublishFinalMessageContainer(messageContainer);
                }

                void Intercept()
                {
                    int interceptions = interceptors.Count;
                    List<InterceptionAction> results = new List<InterceptionAction>();
                    foreach (InterceptorAgent interceptor in interceptors)
                    {
#if NETSTANDARD2_1
                        ThreadPool.QueueUserWorkItem(ExecuteInterception, new InterceptionExecution(interceptions, results, messageContainer, interceptor),
                                                     false);
#else
                        ThreadPool.QueueUserWorkItem((o) => ExecuteInterception((InterceptionExecution) o), 
                                                     new InterceptionExecution(interceptions, results, messageContainer, interceptor));
#endif
                    }
                }
            }

            private void PublishFinalMessageContainer(Message container)
            {
                List<Agent> consumers = registeredMessageAgents.Count > 0
                                            ? new List<Agent>(registeredMessageAgents)
                                            : null;
                foreach (Message message in container.DescendantsAndSelf)
                {
                    if (!registeredAgents.TryGetValue(message.MessageType, out List<Agent> agents))
                    {
                        continue;
                    }

                    consumers = consumers ?? new List<Agent>();
                    consumers.AddRange(agents);
                }

                foreach (Message message in container.DescendantsAndSelf)
                {
                    message.SetUserCount(consumers?.Count ?? 0);
                }

                foreach (Agent consumer in consumers??Enumerable.Empty<Agent>())
                {
                    
#if NETSTANDARD2_1
                    ThreadPool.QueueUserWorkItem((Message m) =>
                    {
                        consumer.Execute(container);
                        container.Used(true);
                    }, container, false);
#else
                        ThreadPool.QueueUserWorkItem((o) => 
                        {
                            Message message = (Message) o;
                            consumer.Execute(message);
                            message.Used(true);
                        }, container);
#endif
                }
            }

            private void ExecuteInterception(InterceptionExecution execution)
            {
                InterceptionAction result = execution.Interceptor.Intercept(execution.Message);
                bool canPublishFinalMessage;
                bool disposeFinalMessage;
                lock (execution.Results)
                {
                    execution.Results.Add(result);
                    if (execution.Results.Count != execution.Interceptions)
                    {
                        return;
                    }

                    Dictionary<InterceptionResult, InterceptionAction[]> groupedResults = execution.Results
                        .GroupBy(r => r.Result)
                        .ToDictionary(g => g.Key, g => g.ToArray());
                    if (groupedResults.ContainsKey(InterceptionResult.Delay))
                    {
                        if (groupedResults.ContainsKey(InterceptionResult.DoNotPublish))
                        {
                            foreach (Message message in execution.Message.HeadMessage.DescendantsAndSelf.ToArray())
                            {
                                message.Dispose();
                            }
                            Publish(new ExceptionMessage($"Intercepted message {execution.Message} has action DoNotPublish and Delay, which is not allowed.", execution.Message, execution.Interceptor));
                            return;
                        }

                        int delays = groupedResults[InterceptionResult.Delay].Length;
                        foreach (InterceptionAction delayAction in groupedResults[InterceptionResult.Delay])
                        {
                            delayAction.DelayToken.Register(() =>
                            {
                                int decremented = Interlocked.Decrement(ref delays);
                                if (decremented == 0)
                                {
                                    PublishFinalMessageContainer(execution.Message.HeadMessage);
                                }
                            });
                        }
                    }

                    
                    canPublishFinalMessage = !(groupedResults.ContainsKey(InterceptionResult.Delay) || 
                                               groupedResults.ContainsKey(InterceptionResult.DoNotPublish));
                    disposeFinalMessage = groupedResults.ContainsKey(InterceptionResult.DoNotPublish);
                }

                if (canPublishFinalMessage)
                {
                    PublishFinalMessageContainer(execution.Message.HeadMessage);
                }
                else if(disposeFinalMessage)
                {
                    foreach (Message message in execution.Message.HeadMessage.DescendantsAndSelf.ToArray())
                    {
                        message.Dispose();
                    }
                }
            }

            public void Dispose()
            {
                registeredAgents.Clear();
            }

            private class InterceptionExecution
            {
                public int Interceptions { get; }
                public List<InterceptionAction> Results { get; }
                public Message Message { get; }
                public InterceptorAgent Interceptor { get; }

                public InterceptionExecution(in int interceptions, List<InterceptionAction> results, Message message, InterceptorAgent interceptor)
                {
                    Interceptions = interceptions;
                    Results = results;
                    Message = message;
                    Interceptor = interceptor;
                }
            }
        }
    }
}
