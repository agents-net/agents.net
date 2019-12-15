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

        public void Subscribe(MessageDefinition trigger, Agent agent)
        {
            if (disposed)
            {
                return;
            }
            publisher.Subscribe(trigger, agent);
        }

        public void SubscribeDecorator(MessageDefinition decoratedMessage, DecoratorAgent agent)
        {
            if (disposed)
            {
                return;
            }
            publisher.SubscribeDecorator(decoratedMessage, agent);
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
            private readonly Dictionary<MessageDefinition, List<Agent>> subscribedAgents =
                new Dictionary<MessageDefinition, List<Agent>>();

            private readonly Dictionary<MessageDefinition, List<DecoratorAgent>> decoratorAgents = 
                new Dictionary<MessageDefinition, List<DecoratorAgent>>();

            public void Subscribe(MessageDefinition trigger, Agent agent)
            {
                if (!subscribedAgents.ContainsKey(trigger))
                {
                    subscribedAgents.Add(trigger, new List<Agent>());
                }
                subscribedAgents[trigger].Add(agent);
            }

            public void SubscribeDecorator(MessageDefinition decoratedMessage, DecoratorAgent decoratorAgent)
            {
                if (!decoratorAgents.ContainsKey(decoratedMessage))
                {
                    decoratorAgents.Add(decoratedMessage, new List<DecoratorAgent>());
                }
                decoratorAgents[decoratedMessage].Add(decoratorAgent);
            }

            public void Publish(Message messageContainer)
            {
                PublishSingleMessage(messageContainer);
                foreach (Message message in messageContainer.Children)
                {
                    PublishSingleMessage(message);
                }
            }

            private void PublishSingleMessage(Message message)
            {
                if (decoratorAgents.TryGetValue(message.Definition, out List<DecoratorAgent> decoratingAgents) &&
                    !IsCompletelyDecorated(out DecoratorAgent nextDecoratingAgent))
                {
#if NETSTANDARD2_1
                    ThreadPool.QueueUserWorkItem(nextDecoratingAgent.Execute, message, false);
#else
                    ThreadPool.QueueUserWorkItem((o) => nextDecoratingAgent.Execute((Message) o), message);
#endif
                }
                else if (subscribedAgents.TryGetValue(message.Definition, out List<Agent> agents))
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

                bool IsCompletelyDecorated(out DecoratorAgent decoratingAgent)
                {
                    decoratingAgent = decoratingAgents.FirstOrDefault(b => !b.DecoratorDefinition.ProducingTriggers
                                                                             .Any(message.Contains));
                    return decoratingAgent == null;
                }
            }

            public void Dispose()
            {
                subscribedAgents.Clear();
            }
        }
    }
}
