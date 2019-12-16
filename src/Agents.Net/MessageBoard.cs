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
    public class MessageBoard : IDisposable
    {
        private readonly MessagePublisher publisher = new MessagePublisher();
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ConcurrentQueue<Message> pendingMessages = new ConcurrentQueue<Message>();
        private readonly List<Message> publishedMessages = new List<Message>();
        private Thread spinningThread;
        private bool disposed;

        public MessageBoard()
        {
            pendingMessages.Enqueue(new InitializeMessage());
        }

        public void Publish(Message message)
        {
            if (disposed)
            {
                return;
            }
            pendingMessages.Enqueue(message);
        }

        public void Start()
        {
            spinningThread = new Thread(() =>
            {
                try
                {
                    while (!disposed)
                    {
                        if (pendingMessages.TryDequeue(out Message message))
                        {
                            PublishAllMessages(message);
                        }
                        else
                        {
                            Thread.SpinWait(10);
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e, "Exception in spinning thread.");
                }
            }) {Priority = ThreadPriority.AboveNormal};
            spinningThread.Start();

            void PublishAllMessages(Message messageContainer)
            {
                publishedMessages.Add(messageContainer);
                Logger.Trace(messageContainer);
                publisher.Publish(messageContainer);
            }
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
            spinningThread?.Join();
            DisposeMessages();
            publisher.Dispose();

            void DisposeMessages()
            {
                while (pendingMessages.TryDequeue(out Message message))
                {
                    if (message is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }

                foreach (IDisposable disposable in publishedMessages.OfType<IDisposable>())
                {
                    disposable.Dispose();
                }

                publishedMessages.Clear();
            }
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
                lock (execution.Results)
                {
                    execution.Results.Add(result);
                }

                if (execution.Results.Count == execution.Interceptions &&
                    execution.Results.All(r => r != InterceptionAction.DoNotPublish))
                {
                    PublishFinalMessageContainer(execution.Message.HeadMessage);
                }
            }

            private void PublishSingleMessage(Message message)
            {
                if (registeredAgents.TryGetValue(message.Definition, out List<Agent> agents))
                {
                    foreach (Agent agent in agents)
                    {
#if NETSTANDARD2_1
                        ThreadPool.QueueUserWorkItem(agent.Execute, message, false);
#else
                        ThreadPool.QueueUserWorkItem((o) => agent.Execute((Message) o), message);
#endif
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
