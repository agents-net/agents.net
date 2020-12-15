﻿#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;
using System.Collections.Generic;
using System.Linq;

namespace Agents.Net
{
    /// <summary>
    /// This class is a helper which aggregates messages which were executed parallel with the <see cref="Agent.OnMessages"/> method.
    /// </summary>
    /// <typeparam name="T">The type of the message which should be aggregated.</typeparam>
    /// <remarks>
    /// <para>
    /// When a batch of messages is published using the <see cref="Agent.OnMessages"/> they can be united again when all last messages of the execution chain are of the same type.
    /// </para>
    /// <code>
    ///  --------------         ---------------------          -----------------
    /// | SplitMessage | ----> | IntermediateMessage | -----> | FinishedMessage |
    ///  --------------         ---------------------  |       -----------------
    ///                                                |
    ///                                                |       ------------------
    ///                                                 ----> | ExceptionMessage |
    ///                                                        ------------------
    /// </code>
    /// <para>
    /// Looking at the example above it would not be possible to unite the <c>SplitMessages</c> again using this class as at least one <c>IntermediateMessage</c> let to an <c>ExceptionMessage</c>.
    /// </para>
    /// <example>
    /// Here a typical example how to setup and use the aggregator in a class:
    /// <code>
    /// [Consumes(typeof(FinishedMessage))]
    /// public class MessageAggregatorAgent : Agent
    /// {
    ///     private readonly MessageAggregator&lt;FinishedMessage&gt; aggregator;
    /// 
    ///     public MessageAggregatorAgent(IMessageBoard messageBoard) : base(messageBoard)
    ///     {
    ///         aggregator = new MessageAggregator&lt;FinishedMessage&gt;(OnAggregated);
    ///     }
    /// 
    ///     private void OnAggregated(IReadOnlyCollection&lt;FinishedMessage&gt; aggregate)
    ///     {
    ///         //Execute your code here
    ///     }
    /// 
    ///     protected override void ExecuteCore(Message messageData)
    ///     {
    ///         aggregator.Aggregate(messageData);
    ///     }
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    public class MessageAggregator<T> where T:Message
    {
        private readonly Action<IReadOnlyCollection<T>> onAggregated;

        /// <summary>
        /// Initialized a new instance of the class <see cref="MessageAggregator{T}"/>.
        /// </summary>
        /// <param name="onAggregated">The action which is executed when all messages were aggregated.</param>
        public MessageAggregator(Action<IReadOnlyCollection<T>> onAggregated)
        {
            this.onAggregated = onAggregated;
        }

        private readonly Dictionary<IReadOnlyCollection<Message>, HashSet<MessageStore<T>>> aggregatedMessages =
            new Dictionary<IReadOnlyCollection<Message>, HashSet<MessageStore<T>>>();
        private readonly object dictionaryLock = new object();

        /// <summary>
        /// Tries to add the message to this instance.
        /// </summary>
        /// <param name="message">The message to add to this instance.</param>
        /// <returns><c>true</c> if the message was added; otherwise <c>false</c></returns>
        /// <exception cref="ArgumentNullException">Thrown if the message is <c>null</c>.</exception>
        /// <remarks>
        /// This is useful, when the <see cref="Agent"/> consumes more than the aggregated message.
        /// </remarks>
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

        /// <summary>
        /// Adds the message to this instance.
        /// </summary>
        /// <param name="message">The message to add to this instance.</param>
        /// <exception cref="ArgumentNullException">Thrown if the message is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the <paramref name="message"/> is does not contain the type <typeparamref name="T"/>.</exception>
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
