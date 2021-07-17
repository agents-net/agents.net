#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Agents.Net
{
    /// <summary>
    /// This class is a helper which aggregates messages of the same type that are executed in parallel.
    /// </summary>
    /// <typeparam name="T">The type of the message which should be aggregated.</typeparam>
    /// <remarks>
    /// <para>
    /// When a batch of messages is published using the this class they can be united again when all last messages of the execution chain are of the same type or an exception message.
    /// </para>
    /// <code>
    ///  --------------         ---------------------          -----------------
    /// | SplitMessage | ----> | IntermediateMessage | -----> | FinishedMessage |
    ///  --------------         ---------------------  |       -----------------
    ///                                                |
    ///                                                |       ------------------
    ///                                                *----> | ExceptionMessage |
    ///                                                |       ------------------
    ///                                                |
    ///                                                |       ------------------
    ///                                                 ----> | OtherEndMessage  |
    ///                                                        ------------------
    /// </code>
    /// <para>
    /// Looking at the example above it would not be possible to unite the <c>SplitMessages</c> again using this class as at least one <c>IntermediateMessage</c> let to an <c>OtherEndMessage</c>.
    /// </para>
    /// <example>
    /// Here a typical example how to setup and use the aggregator in a class:
    /// <code>
    /// [Consumes(typeof(FinishedMessage))]
    /// [Consumes(typeof(ExceptionMessage))]
    /// [Produces(typeof(StartMessage))]
    /// public class MessageAggregatorAgent : Agent
    /// {
    ///     private readonly MessageAggregator&lt;FinishedMessage&gt; aggregator = new MessageAggregator&lt;FinishedMessage&gt;();
    /// 
    ///     public MessageAggregatorAgent(IMessageBoard messageBoard) : base(messageBoard)
    ///     {
    ///     }
    /// 
    ///     protected override void ExecuteCore(Message messageData)
    ///     {
    ///         if(aggregator.TryAggregate(messageData))
    ///         {
    ///             return;
    ///         }
    ///         //create startMessages
    ///         aggregator.SendAndAggregate(startMessages, OnMessage);
    ///     }
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    public class MessageAggregator<T> where T:Message
    {
        private readonly Action<IReadOnlyCollection<T>> onAggregated;
        private readonly bool autoTerminate;

        /// <summary>
        /// A constant value to tell the <see cref="MessageAggregator{T}"/> that it has to wait forever.
        /// </summary>
        public const int NoTimout = -1;

        /// <summary>
        /// Initialized a new instance of the class <see cref="MessageAggregator{T}"/>.
        /// </summary>
        /// <param name="onAggregated">The action which is executed when all messages were aggregated.</param>
        /// <param name="autoTerminate">
        /// If set to <c>true</c>, the aggregator will terminate all message domains of the aggregated messages before the execution.
        /// Default is <c>true</c>.
        /// </param>
        public MessageAggregator(Action<IReadOnlyCollection<T>> onAggregated, bool autoTerminate = true)
        {
            this.onAggregated = onAggregated;
            this.autoTerminate = autoTerminate;
        }

        /// <summary>
        /// Initialized a new instance of the class <see cref="MessageAggregator{T}"/>.
        /// </summary>
        public MessageAggregator()
        {
            
        }

        private readonly Dictionary<IReadOnlyCollection<Message>, HashSet<MessageStore<T>>> aggregatedMessages =
            new Dictionary<IReadOnlyCollection<Message>, HashSet<MessageStore<T>>>();
        private readonly object dictionaryLock = new object();

        /// <summary>
        /// The method to send the start messages and wait for all end messages.
        /// </summary>
        /// <param name="startMessages">The start messages to send.</param>
        /// <param name="onMessage">The action to send the message.</param>
        /// <param name="onAggregated">The action to execute once a <see cref="MessageAggregatorResult{T}"/> was created.</param>
        /// <param name="timeout">
        /// Optionally a timeout after which the method will return, without sending the result.
        /// By default the timeout is <see cref="NoTimout"/>
        /// </param>
        /// <param name="cancellationToken">
        /// Optionally a cancellation token to cancel the continue operation. By default no CancellationToken will be used.
        /// </param>
        /// <remarks>
        /// <para>
        /// This function is useful when the aggregated messages need to be modified - for example
        /// filtered - before aggregating them. In all other cases it is better to use <see cref="SendAndAggregate"/>
        /// to automatically create and send an aggregated message. 
        /// </para>
        /// <example>
        /// This is an example, how to use this method correctly:
        /// <code>
        /// [Consumes(typeof(FinishedMessage))]
        /// [Consumes(typeof(ExceptionMessage))]
        /// [Produces(typeof(StartMessage))]
        /// [Produces(typeof(AggregatedMessage))]
        /// public class MessageAggregatorAgent : Agent
        /// {
        ///     private readonly MessageAggregator&lt;FinishedMessage&gt; aggregator = new MessageAggregator&lt;FinishedMessage&gt;();
        /// 
        ///     public MessageAggregatorAgent(IMessageBoard messageBoard) : base(messageBoard)
        ///     {
        ///     }
        /// 
        ///     protected override void ExecuteCore(Message messageData)
        ///     {
        ///         if(aggregator.TryAggregate(messageData))
        ///         {
        ///             return;
        ///         }
        ///         //create startMessages
        ///         aggregator.SendAndContinue(startMessages, OnMessage, result =>
        ///         {
        ///             //manipulate the results and produce aggregated message
        ///             OnMessage(aggregatedMessage);
        ///         });
        ///     }
        /// }
        /// </code>
        /// </example>
        /// </remarks>
        public void SendAndContinue(IEnumerable<Message> startMessages, Action<Message> onMessage,
                                    Action<MessageAggregatorResult<T>> onAggregated, int timeout = NoTimout,
                                    CancellationToken cancellationToken = default)
        {
            //use massage gates
            //no need anymore for SiblingDomains
            //TODO Delete annoying name argument from message and agents
        }

        /// <summary>
        /// The method to send the start messages and aggregate all end messages in a <see cref="MessagesAggregated{T}"/>.
        /// </summary>
        /// <param name="startMessages">The start messages to send.</param>
        /// <param name="onMessage">The action to send the message.</param>
        /// <remarks>
        /// <para>For an example how to use this method see the documentation of the type.</para>
        /// <para>The aggregation message is only send if the <see cref="MessageAggregatorResult{TEnd}.Result"/> is <see cref="WaitResultKind.Success"/>. Otherwise an <see cref="AggregatedExceptionMessage"/> or <see cref="ExceptionMessage"/> is send.</para>
        /// </remarks>
        public void SendAndAggregate(IEnumerable<Message> startMessages, Action<Message> onMessage)
        {
            
        }

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
                T[] messages = completedMessageBatch.Select<MessageStore<T>, T>(m => m).ToArray();
                if (autoTerminate)
                {
                    MessageDomain.TerminateDomainsOf(messages);
                }
                onAggregated(messages);
                foreach (MessageStore<T> messageStore in completedMessageBatch)
                {
                    messageStore.Dispose();
                }
            }
        }
    }
}
