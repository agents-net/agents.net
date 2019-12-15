#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Collections.Generic;
using NLog;

namespace Agents.Net
{
    public class MessageAggregator<T> where T:Message
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly Action<ICollection<T>> aggregateAction;

        public MessageAggregator(Action<ICollection<T>> aggregateAction)
        {
            this.aggregateAction = aggregateAction;
        }

        private readonly Dictionary<MessageDomainsCreatedMessage, HashSet<T>> aggregatedMessages =
            new Dictionary<MessageDomainsCreatedMessage, HashSet<T>>();
        private readonly object dictionaryLock = new object();

        public bool TryAggregate(Message message)
        {
            if (!message.Is<T>())
            {
                return false;
            }
            Aggregate(message);
            return true;
        }

        public void Aggregate(Message message)
        {
            if (!message.TryGet(out T aggregatedMessage))
            {
                throw new InvalidOperationException($"Cannot aggregate the message {message}. Aggregated type is {typeof(T)}");
            }

            if (!aggregatedMessage.MessageDomain.Root.TryGet(out MessageDomainsCreatedMessage root))
            {
                throw new InvalidOperationException($"Cannot aggregate message {message} because domain root " +
                                                    $"message does not contain a {nameof(MessageDomainsCreatedMessage)}");
            }

            if (aggregatedMessage.MessageDomain.IsTerminated)
            {
                return;
            }

            HashSet<T> completedMessageBatch = null;
            lock (dictionaryLock)
            {
                Logger.Trace($"Enter aggregation lock. Source:{message.Id}");
                if (!aggregatedMessages.ContainsKey(root))
                {
                    aggregatedMessages.Add(root, new HashSet<T>());
                }
                aggregatedMessages[root].Add(aggregatedMessage);
                if (aggregatedMessages[root].Count == root.DomainRootMessages.Length)
                {
                    completedMessageBatch = aggregatedMessages[root];
                    aggregatedMessages.Remove(root);
                }
                Logger.Trace($"Exit aggregation lock. Batch:{completedMessageBatch != null}");
            }

            if (completedMessageBatch != null)
            {
                Logger.Trace($"Execute aggregated message. Source:{message.Id}");
                aggregateAction(completedMessageBatch);
            }
        }
    }
}
