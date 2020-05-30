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
        private readonly Action<ICollection<T>> onAggregated;

        public MessageAggregator(Action<ICollection<T>> onAggregated)
        {
            this.onAggregated = onAggregated;
        }

        private readonly Dictionary<IReadOnlyCollection<Message>, HashSet<T>> aggregatedMessages =
            new Dictionary<IReadOnlyCollection<Message>, HashSet<T>>();
        private readonly object dictionaryLock = new object();

        public bool TryAggregate(Message message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (!message.Is<T>())
            {
                return false;
            }
            Aggregate(message);
            return true;
        }

        public void Aggregate(Message message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (!message.TryGet(out T aggregatedMessage))
            {
                throw new InvalidOperationException($"Cannot aggregate the message {message}. Aggregated type is {typeof(T)}");
            }

            IReadOnlyCollection<Message> root = aggregatedMessage.MessageDomain.SiblingDomainRootMessages;
            if (root == null)
            {
                throw new InvalidOperationException($"Cannot aggregate message {message} because domain " +
                                                    $"does not contain a {nameof(MessageDomainsCreatedMessage)}");
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
                if (aggregatedMessages[root].Count == root.Count)
                {
                    completedMessageBatch = aggregatedMessages[root];
                    aggregatedMessages.Remove(root);
                }
                Logger.Trace($"Exit aggregation lock. Batch:{completedMessageBatch != null}");
            }

            if (completedMessageBatch != null)
            {
                Logger.Trace($"Execute aggregated message. Source:{message.Id}");
                onAggregated(completedMessageBatch);
            }
        }
    }
}
