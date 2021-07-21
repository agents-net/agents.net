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
    /// This class is deprecated. Please use <see cref="MessageAggregator{TStart,TEnd}"/> instead.
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
    [Obsolete("This class is no longer maintained. Please switch to the new MessageAggregator<TStart,TEnd> type. This method will be removed with version 2022.6")]
    public class MessageAggregator<T> where T:Message
    {
        private readonly Action<IReadOnlyCollection<T>> onAggregated;
        private readonly bool autoTerminate;

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
    
    /// <summary>
    /// This class is a helper which aggregates messages of the same type that are executed in parallel.
    /// </summary>
    /// <typeparam name="TEnd">The type of the message which should be aggregated.</typeparam>
    /// <typeparam name="TStart">The type of the message which should is send.</typeparam>
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
    public class MessageAggregator<TStart,TEnd> 
        where TEnd : Message
        where TStart : Message
    {
        /// <summary>
        /// A constant value to tell the <see cref="MessageAggregator{T}"/> that it has to wait forever.
        /// </summary>
        public const int NoTimout = -1;

        private readonly List<MessageGate<TStart, TEnd>> gates = new List<MessageGate<TStart, TEnd>>();
        private readonly object listLock = new object();

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
        public void SendAndContinue(IEnumerable<TStart> startMessages, Action<Message> onMessage,
                                    Action<MessageAggregatorResult<TEnd>> onAggregated, int timeout = NoTimout,
                                    CancellationToken cancellationToken = default)
        {
            (TStart, MessageGate<TStart, TEnd>)[] gatesBatch = startMessages.Select(m => (m, new MessageGate<TStart, TEnd>()))
                                                                            .ToArray();
            int aggregated = 0;
            ConcurrentBag<MessageStore<TEnd>> endMessages = new ConcurrentBag<MessageStore<TEnd>>();
            ConcurrentBag<MessageStore<ExceptionMessage>> exceptions = new ConcurrentBag<MessageStore<ExceptionMessage>>();
            WaitResultKind resultKind = WaitResultKind.Success;
            lock (listLock)
            {
                gates.AddRange(gatesBatch.Select(p => p.Item2));
            }
            foreach ((TStart message, MessageGate<TStart, TEnd> gate) part in gatesBatch)
            {
                part.gate.SendAndContinue(part.message, onMessage, result =>
                {
                    AggregateResult(result);
                }, timeout, cancellationToken);
            }
            //TODO Delete annoying name argument from message and agents
            //TODO Move NoTimeout to timeout class
            
            void AggregateResult(MessageGateResult<TEnd> messageGateResult)
            {
                if (messageGateResult.EndMessage != null)
                {
                    endMessages.Add(messageGateResult.EndMessage);
                }

                foreach (ExceptionMessage exception in messageGateResult.Exceptions)
                {
                    exceptions.Add(exception);
                }

                if (messageGateResult.Result != WaitResultKind.Success)
                {
                    //It is ok to override other values with the latest
                    resultKind = messageGateResult.Result;
                }

                if (Interlocked.Increment(ref aggregated) == gatesBatch.Length)
                {
                    onAggregated(
                        new MessageAggregatorResult<TEnd>(resultKind, endMessages.Select(s => (TEnd)s).ToArray(), exceptions.Select(s => (ExceptionMessage)s).ToArray()));
                    foreach (MessageStore<TEnd> store in endMessages)
                    {
                        store.Dispose();
                    }
                    foreach (MessageStore<ExceptionMessage> store in exceptions)
                    {
                        store.Dispose();
                    }
                }
            }
        }

        /// <summary>
        /// The method to send the start messages and aggregate all end messages in a <see cref="MessagesAggregated{T}"/>.
        /// </summary>
        /// <param name="startMessages">The start messages to send.</param>
        /// <param name="onMessage">The action to send the message.</param>
        /// <remarks>
        /// <para>For an example how to use this method see the documentation of the type.</para>
        /// </remarks>
        public void SendAndAggregate(IReadOnlyCollection<TStart> startMessages, Action<Message> onMessage)
        {
            SendAndContinue(startMessages, onMessage, result =>
            {
                onMessage(new MessagesAggregated<TEnd>(result));
            });
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

            lock (listLock)
            {
                return gates.Aggregate(false, (b, gate) => b |= gate.Check(message));
            }
        }

        /// <summary>
        /// Adds the message to this instance.
        /// </summary>
        /// <param name="message">The message to add to this instance.</param>
        /// <exception cref="ArgumentNullException">Thrown if the message is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the <paramref name="message"/> is does not contain the type <typeparamref name="TEnd"/>.</exception>
        public void Aggregate(Message message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (!TryAggregate(message))
            {
                throw new InvalidOperationException($"Cannot aggregate the message {message}. Aggregated type is {typeof(TEnd)}");
            }
        }
    }
}
