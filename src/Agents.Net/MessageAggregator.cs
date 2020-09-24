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
using System.Linq;

namespace Agents.Net
{
    public class MessageAggregator<T> where T:Message
    {
        private readonly Action<IReadOnlyCollection<T>> onAggregated;

        public MessageAggregator(Action<IReadOnlyCollection<T>> onAggregated)
        {
            this.onAggregated = onAggregated;
        }

        private readonly Dictionary<IReadOnlyCollection<Message>, HashSet<MessageStore<T>>> aggregatedMessages =
            new Dictionary<IReadOnlyCollection<Message>, HashSet<MessageStore<T>>>();
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

            HashSet<MessageStore<T>> completedMessageBatch = null;
            lock (dictionaryLock)
            {
                if (!aggregatedMessages.ContainsKey(root))
                {
                    aggregatedMessages.Add(root, new HashSet<MessageStore<T>>());
                }
                aggregatedMessages[root].Add(aggregatedMessage);
                if (aggregatedMessages[root].Count == root.Count)
                {
                    completedMessageBatch = aggregatedMessages[root];
                    aggregatedMessages.Remove(root);
                }
            }

            if (completedMessageBatch != null)
            {
                onAggregated(completedMessageBatch.Select<MessageStore<T>,T>(m => m).ToArray());
                foreach (MessageStore<T> messageStore in completedMessageBatch)
                {
                    messageStore.Dispose();
                }
            }
        }
    }
}
