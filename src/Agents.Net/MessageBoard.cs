#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NLog;

namespace Agents.Net
{
    public sealed class MessageBoard : IDisposable, IMessageBoard
    {
        private readonly MessagePublisher publisher = new MessagePublisher();
        private bool disposed;

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

        public void Start()
        {
            Publish(new InitializeMessage());
        }

        private void PublishAllMessages(Message messageContainer)
        {
            publisher.Publish(messageContainer);
        }

        public void Register(MessageDefinition trigger, Agent agent)
        {
            if (disposed)
            {
                return;
            }
            publisher.Register(trigger, agent);
        }

        public void RegisterInterceptor(MessageDefinition trigger, InterceptorAgent agent)
        {
            if (disposed)
            {
                return;
            }
            publisher.RegisterInterceptor(trigger, agent);
        }

        public void Dispose()
        {
            disposed = true;
            publisher.Dispose();
        }

        private class MessagePublisher : IDisposable
        {
            private readonly Dictionary<MessageDefinition, List<Agent>> registeredAgents =
                new Dictionary<MessageDefinition, List<Agent>>();

            private readonly Dictionary<MessageDefinition, List<InterceptorAgent>> interceptorAgents = 
                new Dictionary<MessageDefinition, List<InterceptorAgent>>();

            public void Register(MessageDefinition trigger, Agent agent)
            {
                if (!registeredAgents.ContainsKey(trigger))
                {
                    registeredAgents.Add(trigger, new List<Agent>());
                }
                registeredAgents[trigger].Add(agent);
            }

            public void RegisterInterceptor(MessageDefinition trigger, InterceptorAgent agent)
            {
                if (!interceptorAgents.ContainsKey(trigger))
                {
                    interceptorAgents.Add(trigger, new List<InterceptorAgent>());
                }
                interceptorAgents[trigger].Add(agent);
            }

            public void Publish(Message messageContainer)
            {
                List<InterceptorAgent> interceptors = null;
                foreach (Message message in messageContainer.Children.Concat(new []{messageContainer}))
                {
                    if (!interceptorAgents.TryGetValue(message.Definition, out List<InterceptorAgent> agents))
                    {
                        continue;
                    }

                    if (interceptors == null)
                    {
                        interceptors = new List<InterceptorAgent>();
                    }

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
                PublishSingleMessage(container);
                foreach (Message message in container.Children)
                {
                    PublishSingleMessage(message);
                }
            }

            private void ExecuteInterception(InterceptionExecution execution)
            {
                InterceptionAction result = execution.Interceptor.Intercept(execution.Message);
                bool canPublishFinalMessage;
                lock (execution.Results)
                {
                    execution.Results.Add(result);
                    canPublishFinalMessage = execution.Results.Count == execution.Interceptions &&
                                             execution.Results.All(r => r != InterceptionAction.DoNotPublish);
                }

                if (canPublishFinalMessage)
                {
                    PublishFinalMessageContainer(execution.Message.HeadMessage);
                }
                else
                {
                    (execution.Message.HeadMessage as IDisposable)?.Dispose();
                    foreach (Message child in execution.Message.HeadMessage.Children.ToArray())
                    {
                        (child as IDisposable)?.Dispose();
                    }
                }
            }

            private void PublishSingleMessage(Message message)
            {
                if (!registeredAgents.TryGetValue(message.Definition, out List<Agent> agents))
                {
                    (message as IDisposable)?.Dispose();
                    return;
                }

                message.SetUserCount(agents.Count);

                foreach (Agent agent in agents)
                {
#if NETSTANDARD2_1
                    ThreadPool.QueueUserWorkItem((Message m) =>
                    {
                        agent.Execute(message);
                        message.Used();
                    }, message, false);
#else
                        ThreadPool.QueueUserWorkItem((o) => 
                        {
                            agent.Execute((Message) o);
                            message.Used();
                        }, message);
#endif
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
