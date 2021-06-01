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
    /// This is a helper class that collects thread safe all messages until all message types are collected.
    /// </summary>
    /// <typeparam name="T1">First message type.</typeparam>
    /// <typeparam name="T2">Second message type.</typeparam>
    /// <remarks>
    /// <para>
    /// The collector collects messages until all message types are collected. Afterwards it executes an action with the collected <see cref="MessageCollection"/>. In the following different scenarios are described, which explain the different aspects of this class. 
    /// </para>
    /// <para>
    /// Overriding messages
    /// </para>
    /// <para>
    /// Assuming the message of type <typeparamref name="T1"/> was collected and <typeparamref name="T2"/> was not. When another message of type <typeparamref name="T1"/> is collected it replaces the old <typeparamref name="T1"/> message with the new message. No action is executed.
    /// </para>
    /// <para>
    /// Considering message domains
    /// </para>
    /// <para>
    /// Assuming the following <see cref="MessageDomain"/>s and the collected messages:
    /// <code>
    ///  ---------------         ------------
    /// | DefaultDomain | ----> | SubDomain1 |
    ///  ---------------  |      ------------
    /// T1 Message1       |      T1 Message2
    ///                   |      T2 Message4
    ///                   |
    ///                   |      ------------
    ///                   ----> | SubDomain2 |
    ///                          ------------
    ///                          T2 Message3
    ///                          T2 Message5
    /// </code>
    /// Here is what happens in this scenario. Message1 is collected. Message2 is collected. This does not override Message1 because it is from a different MessageDomain. It is stored parallel. Message3 is collected. Now SubDomain2 has a complete set of Message1 + Message3. Message 4 is collected. Now there is another completed set in SubDomain1 of Message2 + Message4. Message5 is collected. This overrides Message3 as it is in the same MessageDomain. A new set is executed with Message1 + Message5. In the end the following sets were executed inorder:
    /// <list type="number">
    /// <item><description>Message1 + Message3</description></item>
    /// <item><description>Message2 + Message4</description></item>
    /// <item><description>Message1 + Message5</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// Consumed vs not consumed messages
    /// </para>
    /// <para>
    /// Messages can be consumed during execution with the method <see cref="MessageCollection.MarkAsConsumed"/>. The consumed message is removed from the message collector immediately. Looking at the example above the Message1 is used twice. Assuming during the first execution of Message1 + Message3 the agent executed <c>collection.MarkAsConsumed(Message1)</c>. In this case the third execution Message1 + Message5 would not have happened, as Message1 would have been cleared from the collector instance. 
    /// </para>
    /// </remarks>
    /// <example>
    /// Here a typical example how to setup and use the collector in a class:
    /// <code>
    /// [Consumes(typeof(Message1))]
    /// [Consumes(typeof(Message2))]
    /// public class MessageCollectorAgent : Agent
    /// {
    ///     private readonly MessageCollector&lt;Message1, Message2&gt; collector;
    /// 
    ///     public MessageCollectorAgent(IMessageBoard messageBoard) : base(messageBoard)
    ///     {
    ///         collector = new MessageCollector&lt;Message1, Message2&gt;(OnMessagesCollected);
    ///     }
    /// 
    ///     private void OnMessagesCollected(MessageCollection&lt;Message1, Message2&gt; set)
    ///     {
    ///         //execute set
    ///     }
    /// 
    ///     protected override void ExecuteCore(Message messageData)
    ///     {
    ///         collector.Push(messageData);
    ///     }
    /// }
    /// </code>
    /// </example>
    public class MessageCollector<T1, T2>
        where T1 : Message
        where T2 : Message
    {
        /// <summary>
        /// Store for messages of type <typeparamref name="T1"/>.
        /// </summary>
        protected Dictionary<MessageDomain, MessageStore<T1>> Messages1 { get; } = new Dictionary<MessageDomain, MessageStore<T1>>();
        /// <summary>
        /// Store for messages of type <typeparamref name="T2"/>.
        /// </summary>
        protected Dictionary<MessageDomain, MessageStore<T2>> Messages2 { get; } = new Dictionary<MessageDomain, MessageStore<T2>>();
        
        private readonly Action<MessageCollection<T1, T2>> onMessagesCollected;
        private readonly object dictionaryLock = new object();

        private readonly Dictionary<Message, Action<MessageCollection>> oneShotActions =
            new Dictionary<Message, Action<MessageCollection>>(); 

        /// <summary>
        /// Initialized a new instance of the class <see cref="MessageCollector{T1,T2}"/>.
        /// </summary>
        /// <param name="onMessagesCollected">The action which is executed when all messages were collected.</param>
        public MessageCollector(Action<MessageCollection<T1, T2>> onMessagesCollected = null)
        {
            this.onMessagesCollected = onMessagesCollected;
        }

        /// <summary>
        /// Add a message to the collector.
        /// </summary>
        /// <param name="message">The message object to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if the message is <c>null</c>.</exception>
        public void Push(Message message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            TryPush(message, true);
        }

        /// <summary>
        /// Tries to add the message to the collector.
        /// </summary>
        /// <param name="message">The message object to add.</param>
        /// <returns><c>true</c> if the message was added; otherwise <c>false</c></returns>
        /// <exception cref="ArgumentNullException">Thrown if the message is <c>null</c>.</exception>
        public bool TryPush(Message message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            return TryPush(message, false);
        }

        private bool TryPush(Message message, bool throwError)
        {
            bool result = true;
            IEnumerable<MessageCollection> completedSets;
            lock (dictionaryLock)
            {
                result &= Aggregate(message, throwError);
                completedSets = GetCompleteSets(message.MessageDomain);
            }
            ExecuteCompleteSets(completedSets);
            return result;
        }

        internal void Remove(Message message)
        {
            lock (dictionaryLock)
            {
                RemoveMessage(message);
            }
        }

        /// <summary>
        /// Overriden by inheriting classes to remove the message passed to the method.
        /// </summary>
        /// <param name="message">The message to remove from the collector.</param>
        /// <exception cref="ArgumentException">When the type of the message is not a message type of the collecotr.</exception>
        protected virtual void RemoveMessage(Message message)
        {
            if (message is T1 message1)
            {
                Messages1.Remove(message1.MessageDomain);
            }
            else if (message is T2 message2)
            {
                Messages2.Remove(message2.MessageDomain);
            }
            else
            {
                throw new ArgumentException($"There is no message type {message} in this collection.", nameof(message));
            }
        }

        /// <summary>
        /// Add a message to the collector and wait of the complete set to execute the specified action.
        /// </summary>
        /// <param name="message">The message which is added to the collector.</param>
        /// <param name="onCollected">The action which is executed when the complete set is found.</param>
        /// <remarks>
        /// <para>This method is helpful for <see cref="InterceptorAgent"/>s where the agent in the <see cref="InterceptorAgent.InterceptCore"/> method must wait for a set of message before returning the <see cref="InterceptionAction"/>.</para>
        /// <para>Another example is when the <see cref="InterceptorAgent"/> wants to wait on a single message. In this case the first message is the message that is intercepted. The second message is the message the agent needs. The advantage is, that the collector respects message domains.</para>
        /// </remarks>
        public void PushAndExecute(Message message, Action<MessageCollection<T1, T2>> onCollected)
        {
            ExecutePushAndExecute(message, collection => onCollected((MessageCollection<T1, T2>) collection));
        }

        /// <summary>
        /// Execute the routine for <see cref="PushAndExecute"/>.
        /// </summary>
        /// <param name="message">The message which is pushed.</param>
        /// <param name="executeAction">The action which should execute the action which was passed to the <see cref="PushAndExecute"/> method.</param>
        protected void ExecutePushAndExecute(Message message, Action<MessageCollection> executeAction)
        {
            using (ManualResetEventSlim resetEvent = new ManualResetEventSlim(false))
            {
                MessageCollection collection = null;
                lock (oneShotActions)
                {
                    oneShotActions.Add(message, set =>
                    {
                        collection = set;
                        lock (oneShotActions)
                        {
                            oneShotActions.Remove(message);
                        }
                        resetEvent.Set();
                    });
                }
                
                Push(message);
                resetEvent.Wait();
                
                executeAction?.Invoke(collection);
            }
        }

        /// <summary>
        /// Overridden by inheriting classes to get all sets of message for a specific domain without specific type.
        /// </summary>
        /// <param name="domain">The domain for which sets should be found.</param>
        /// <returns>An enumeration of all completed sets for the domain.</returns>
        /// <exception cref="ArgumentNullException">If the domain is null.</exception>
        protected IEnumerable<MessageCollection> GetCompleteSets(MessageDomain domain)
        {
            if (domain == null)
            {
                throw new ArgumentNullException(nameof(domain));
            }
            HashSet<MessageCollection> sets = new HashSet<MessageCollection>(new MessageSetIdComparer());
            foreach (MessageDomain messageDomain in ThisAndFlattenedChildren(domain).Where(d => !d.IsTerminated))
            {
                if (IsCompleted(messageDomain, out MessageCollection set))
                {
                    sets.Add(set);
                }
            }

            return sets;
        }

        private void ExecuteCompleteSets(IEnumerable<MessageCollection> sets)
        {
            foreach (MessageCollection messageSet in sets)
            {
                if (oneShotActions.Count > 0)
                {
                    foreach (Message message in messageSet)
                    {
                        if (oneShotActions.TryGetValue(message, out Action<MessageCollection> action))
                        {
                            action(messageSet);
                        }
                    }
                }
                Execute(messageSet);
                messageSet.Dispose();
            }
        }

        /// <summary>
        /// Overridden by inheriting classes to see if there is a completed set for the specified domain.
        /// </summary>
        /// <param name="domain">The domain which should be completed.</param>
        /// <param name="messageCollection">The message collection with the complete set.</param>
        /// <returns><c>true</c> if there is a completed set; otherwise <c>false</c></returns>
        protected virtual bool IsCompleted(MessageDomain domain, out MessageCollection messageCollection)
        {
            if (TryGetMessageFittingDomain(domain, Messages1, out MessageStore<T1> message1) &&
                TryGetMessageFittingDomain(domain, Messages2, out MessageStore<T2> message2))
            {
                messageCollection = new MessageCollection<T1, T2>(message1, message2, this);
                return true;
            }

            messageCollection = null;
            return false;
        }

        /// <summary>
        /// Used to get a message from the message dictionaries.
        /// </summary>
        /// <param name="domain">The message domain to get the message for.</param>
        /// <param name="messagePool">The dictionary for the specific message type.</param>
        /// <param name="message">The message for the domain.</param>
        /// <typeparam name="T">The type of the message.</typeparam>
        /// <returns><c>true</c> if the dictionary contains a message for the specific domain; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">If the dictionary is null.</exception>
        protected bool TryGetMessageFittingDomain<T>(MessageDomain domain, Dictionary<MessageDomain, MessageStore<T>> messagePool, out MessageStore<T> message)
            where T : Message
        {
            if (messagePool == null)
            {
                throw new ArgumentNullException(nameof(messagePool));
            }
            MessageDomain current = domain;
            while (current != null)
            {
                if (messagePool.TryGetValue(current, out message))
                {
                    return true;
                }
                current = current.Parent;
            }

            message = null;
            return false;
        }

        /// <summary>
        /// Overridden by inheriting classes to execute the typeless message collection cast to the specific collector type..
        /// </summary>
        /// <param name="messageCollection">The typeless collection.</param>
        protected virtual void Execute(MessageCollection messageCollection)
        {
            onMessagesCollected?.Invoke((MessageCollection<T1, T2>) messageCollection);
        }

        /// <summary>
        /// Overridden by inheriting classes to try to add the message to the collector dictionaries. 
        /// </summary>
        /// <param name="message">The message to add</param>
        /// <param name="throwError">If set to <c>true</c>, throws an error if the message could not be added.</param>
        /// <returns><c>true</c> if the message was added to any dictionary; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">If the message if <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">If <paramref name="throwError"/> was set to <c>false</c> and the message could not be added.</exception>
        protected virtual bool Aggregate(Message message, bool throwError)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            if (message.TryGet(out T1 message1))
            {
                UpdateMessagePool(message1, Messages1);
                return true;
            }

            if (message.TryGet(out T2 message2))
            {
                UpdateMessagePool(message2, Messages2);
                return true;
            }

            if (throwError)
            {
                throw new InvalidOperationException($"{message} does not contain any expected message of {GetType()}");
            }

            return false;
        }

        /// <summary>
        /// Add the message to the specified message pool.
        /// </summary>
        /// <param name="message">The message to add.</param>
        /// <param name="messagePool">The message pool to add the message to.</param>
        /// <typeparam name="T">The type of the message.</typeparam>
        /// <exception cref="ArgumentNullException">If either the message or the message pool is <c>null</c>.</exception>
        protected void UpdateMessagePool<T>(T message, Dictionary<MessageDomain, MessageStore<T>> messagePool)
            where T : Message
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            if (messagePool == null)
            {
                throw new ArgumentNullException(nameof(messagePool));
            }

            if (messagePool.ContainsKey(message.MessageDomain))
            {
                messagePool[message.MessageDomain].Dispose();
                messagePool[message.MessageDomain] = message;
            }
            else
            {
                messagePool.Add(message.MessageDomain, message);
                message.MessageDomain.ExecuteOnTerminate(() => messagePool.Remove(message.MessageDomain));
            }
        }

        private static IEnumerable<MessageDomain> ThisAndFlattenedChildren(MessageDomain messageDomain)
        {
            yield return messageDomain;
            foreach (MessageDomain child in messageDomain.Children.SelectMany(ThisAndFlattenedChildren))
            {
                yield return child;
            }
        }

        private class MessageSetIdComparer : IEqualityComparer<MessageCollection>
        {
            public bool Equals(MessageCollection x, MessageCollection y)
            {
                if (ReferenceEquals(x, y))
                {
                    return true;
                }

                if (ReferenceEquals(null, x))
                {
                    return false;
                }

                if (ReferenceEquals(null, y))
                {
                    return false;
                }

                return x.Select(m => m.Id).OrderBy(id => id).SequenceEqual(y.Select(m => m.Id).OrderBy(id => id));
            }

            public int GetHashCode(MessageCollection obj)
            {
                unchecked
                {
                    return obj.Aggregate(397, (current, message) => current ^ (message.Id.GetHashCode() * 397));
                }
            }
        }
    }

    /// <inheritdoc />
    public class MessageCollector<T1, T2, T3> : MessageCollector<T1, T2>
        where T1 : Message
        where T2 : Message
        where T3 : Message
    {
        /// <summary>
        /// Store for messages of type <typeparamref name="T3"/>.
        /// </summary>
        protected Dictionary<MessageDomain, MessageStore<T3>> Messages3 { get; } = new Dictionary<MessageDomain, MessageStore<T3>>();
        private readonly Action<MessageCollection<T1, T2, T3>> onMessagesCollected;

        /// <inheritdoc />
        public MessageCollector(Action<MessageCollection<T1, T2, T3>> onMessagesCollected = null)
        {
            this.onMessagesCollected = onMessagesCollected;
        }

        /// <inheritdoc />
        protected override bool Aggregate(Message message, bool throwError)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            if (message.TryGet(out T3 message3))
            {
                UpdateMessagePool(message3, Messages3);
                return true;
            }

            return base.Aggregate(message, throwError);
        }

        /// <inheritdoc />
        protected override void Execute(MessageCollection messageCollection)
        {
            onMessagesCollected?.Invoke((MessageCollection<T1, T2, T3>) messageCollection);
        }

        /// <inheritdoc />
        protected override bool IsCompleted(MessageDomain domain, out MessageCollection messageCollection)
        {
            if (TryGetMessageFittingDomain(domain, Messages1, out MessageStore<T1> message1) &&
                TryGetMessageFittingDomain(domain, Messages2, out MessageStore<T2> message2) &&
                TryGetMessageFittingDomain(domain, Messages3, out MessageStore<T3> message3))
            {
                messageCollection = new MessageCollection<T1, T2, T3>(message1, message2, message3, this);
                return true;
            }

            messageCollection = null;
            return false;
        }

        /// <inheritdoc />
        protected override void RemoveMessage(Message message)
        {
            if (message is T3 message3)
            {
                Messages3.Remove(message3.MessageDomain);
            }
            else
            {
                base.RemoveMessage(message);
            }
        }

        /// <summary>
        /// Add a message to the collector and wait of the complete set to execute the specified action.
        /// </summary>
        /// <param name="message">The message which is added to the collector.</param>
        /// <param name="onCollected">The action which is executed when the complete set is found.</param>
        /// <remarks>
        /// <para>This method is helpful for <see cref="InterceptorAgent"/>s where the agent in the <see cref="InterceptorAgent.InterceptCore"/> method must wait for a set of message before returning the <see cref="InterceptionAction"/>.</para>
        /// <para>Another example is when the <see cref="InterceptorAgent"/> wants to wait on a single message. In this case the first message is the message that is intercepted. The second message is the message the agent needs. The advantage is, that the collector respects message domains.</para>
        /// </remarks>
        public void PushAndExecute(Message message, Action<MessageCollection<T1, T2, T3>> onCollected)
        {
            ExecutePushAndExecute(message, collection => onCollected((MessageCollection<T1, T2, T3>) collection));
        }
    }

    /// <inheritdoc />
    public class MessageCollector<T1, T2, T3, T4> : MessageCollector<T1, T2, T3>
        where T1 : Message
        where T2 : Message
        where T3 : Message
        where T4 : Message
    {
        /// <summary>
        /// Store for messages of type <typeparamref name="T4"/>.
        /// </summary>
        protected Dictionary<MessageDomain, MessageStore<T4>> Messages4 { get; } = new Dictionary<MessageDomain, MessageStore<T4>>();
        private readonly Action<MessageCollection<T1, T2, T3, T4>> onMessagesCollected;

        /// <inheritdoc />
        public MessageCollector(Action<MessageCollection<T1, T2, T3, T4>> onMessagesCollected = null)
        {
            this.onMessagesCollected = onMessagesCollected;
        }

        /// <inheritdoc />
        protected override bool Aggregate(Message message, bool throwError)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            if (message.TryGet(out T4 message4))
            {
                UpdateMessagePool(message4, Messages4);
                return true;
            }

            return base.Aggregate(message, throwError);
        }

        /// <inheritdoc />
        protected override void Execute(MessageCollection messageCollection)
        {
            onMessagesCollected?.Invoke((MessageCollection<T1, T2, T3, T4>) messageCollection);
        }

        /// <inheritdoc />
        protected override bool IsCompleted(MessageDomain domain, out MessageCollection messageCollection)
        {
            if (TryGetMessageFittingDomain(domain, Messages1, out MessageStore<T1> message1) &&
                TryGetMessageFittingDomain(domain, Messages2, out MessageStore<T2> message2) &&
                TryGetMessageFittingDomain(domain, Messages3, out MessageStore<T3> message3) &&
                TryGetMessageFittingDomain(domain, Messages4, out MessageStore<T4> message4))
            {
                messageCollection = new MessageCollection<T1, T2, T3, T4>(message1, message2, message3, message4, this);
                return true;
            }

            messageCollection = null;
            return false;
        }

        /// <inheritdoc />
        protected override void RemoveMessage(Message message)
        {
            if (message is T4 message4)
            {
                Messages4.Remove(message4.MessageDomain);
            }
            else
            {
                base.RemoveMessage(message);
            }
        }

        /// <summary>
        /// Add a message to the collector and wait of the complete set to execute the specified action.
        /// </summary>
        /// <param name="message">The message which is added to the collector.</param>
        /// <param name="onCollected">The action which is executed when the complete set is found.</param>
        /// <remarks>
        /// <para>This method is helpful for <see cref="InterceptorAgent"/>s where the agent in the <see cref="InterceptorAgent.InterceptCore"/> method must wait for a set of message before returning the <see cref="InterceptionAction"/>.</para>
        /// <para>Another example is when the <see cref="InterceptorAgent"/> wants to wait on a single message. In this case the first message is the message that is intercepted. The second message is the message the agent needs. The advantage is, that the collector respects message domains.</para>
        /// </remarks>
        public void PushAndExecute(Message message, Action<MessageCollection<T1, T2, T3, T4>> onCollected)
        {
            ExecutePushAndExecute(message, collection => onCollected((MessageCollection<T1, T2, T3, T4>) collection));
        }
    }

    /// <inheritdoc />
    public class MessageCollector<T1, T2, T3, T4, T5> : MessageCollector<T1, T2, T3, T4>
        where T1 : Message
        where T2 : Message
        where T3 : Message
        where T4 : Message
        where T5 : Message
    {
        /// <summary>
        /// Store for messages of type <typeparamref name="T5"/>.
        /// </summary>
        protected Dictionary<MessageDomain, MessageStore<T5>> Messages5 { get; } = new Dictionary<MessageDomain, MessageStore<T5>>();
        private readonly Action<MessageCollection<T1, T2, T3, T4, T5>> onMessagesCollected;

        /// <inheritdoc />
        public MessageCollector(Action<MessageCollection<T1, T2, T3, T4, T5>> onMessagesCollected = null)
        {
            this.onMessagesCollected = onMessagesCollected;
        }

        /// <inheritdoc />
        protected override bool Aggregate(Message message, bool throwError)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            if (message.TryGet(out T5 message5))
            {
                UpdateMessagePool(message5, Messages5);
                return true;
            }

            return base.Aggregate(message, throwError);
        }

        /// <inheritdoc />
        protected override void Execute(MessageCollection messageCollection)
        {
            onMessagesCollected?.Invoke((MessageCollection<T1, T2, T3, T4, T5>) messageCollection);
        }

        /// <inheritdoc />
        protected override bool IsCompleted(MessageDomain domain, out MessageCollection messageCollection)
        {
            if (TryGetMessageFittingDomain(domain, Messages1, out MessageStore<T1> message1) &&
                TryGetMessageFittingDomain(domain, Messages2, out MessageStore<T2> message2) &&
                TryGetMessageFittingDomain(domain, Messages3, out MessageStore<T3> message3) &&
                TryGetMessageFittingDomain(domain, Messages4, out MessageStore<T4> message4) &&
                TryGetMessageFittingDomain(domain, Messages5, out MessageStore<T5> message5))
            {
                messageCollection = new MessageCollection<T1, T2, T3, T4, T5>(message1, message2, message3, message4, message5, this);
                return true;
            }

            messageCollection = null;
            return false;
        }

        /// <inheritdoc />
        protected override void RemoveMessage(Message message)
        {
            if (message is T5 message5)
            {
                Messages5.Remove(message5.MessageDomain);
            }
            else
            {
                base.RemoveMessage(message);
            }
        }

        /// <summary>
        /// Add a message to the collector and wait of the complete set to execute the specified action.
        /// </summary>
        /// <param name="message">The message which is added to the collector.</param>
        /// <param name="onCollected">The action which is executed when the complete set is found.</param>
        /// <remarks>
        /// <para>This method is helpful for <see cref="InterceptorAgent"/>s where the agent in the <see cref="InterceptorAgent.InterceptCore"/> method must wait for a set of message before returning the <see cref="InterceptionAction"/>.</para>
        /// <para>Another example is when the <see cref="InterceptorAgent"/> wants to wait on a single message. In this case the first message is the message that is intercepted. The second message is the message the agent needs. The advantage is, that the collector respects message domains.</para>
        /// </remarks>
        public void PushAndExecute(Message message, Action<MessageCollection<T1, T2, T3, T4, T5>> onCollected)
        {
            ExecutePushAndExecute(message, collection => onCollected((MessageCollection<T1, T2, T3, T4, T5>) collection));
        }
    }

    /// <inheritdoc />
    public class MessageCollector<T1, T2, T3, T4, T5, T6> : MessageCollector<T1, T2, T3, T4, T5>
        where T1 : Message
        where T2 : Message
        where T3 : Message
        where T4 : Message
        where T5 : Message
        where T6 : Message
    {
        /// <summary>
        /// Store for messages of type <typeparamref name="T6"/>.
        /// </summary>
        protected Dictionary<MessageDomain, MessageStore<T6>> Messages6 { get; } = new Dictionary<MessageDomain, MessageStore<T6>>();
        private readonly Action<MessageCollection<T1, T2, T3, T4, T5, T6>> onMessagesCollected;

        /// <inheritdoc />
        public MessageCollector(Action<MessageCollection<T1, T2, T3, T4, T5, T6>> onMessagesCollected = null)
        {
            this.onMessagesCollected = onMessagesCollected;
        }

        /// <inheritdoc />
        protected override bool Aggregate(Message message, bool throwError)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            if (message.TryGet(out T6 message6))
            {
                UpdateMessagePool(message6, Messages6);
                return true;
            }

            return base.Aggregate(message, throwError);
        }

        /// <inheritdoc />
        protected override void Execute(MessageCollection messageCollection)
        {
            onMessagesCollected?.Invoke((MessageCollection<T1, T2, T3, T4, T5, T6>) messageCollection);
        }

        /// <inheritdoc />
        protected override bool IsCompleted(MessageDomain domain, out MessageCollection messageCollection)
        {
            if (TryGetMessageFittingDomain(domain, Messages1, out MessageStore<T1> message1) &&
                TryGetMessageFittingDomain(domain, Messages2, out MessageStore<T2> message2) &&
                TryGetMessageFittingDomain(domain, Messages3, out MessageStore<T3> message3) &&
                TryGetMessageFittingDomain(domain, Messages4, out MessageStore<T4> message4) &&
                TryGetMessageFittingDomain(domain, Messages5, out MessageStore<T5> message5) &&
                TryGetMessageFittingDomain(domain, Messages6, out MessageStore<T6> message6))
            {
                messageCollection = new MessageCollection<T1, T2, T3, T4, T5, T6>(message1, message2, message3, message4, message5, message6, this);
                return true;
            }

            messageCollection = null;
            return false;
        }

        /// <inheritdoc />
        protected override void RemoveMessage(Message message)
        {
            if (message is T6 message6)
            {
                Messages6.Remove(message6.MessageDomain);
            }
            else
            {
                base.RemoveMessage(message);
            }
        }

        /// <summary>
        /// Add a message to the collector and wait of the complete set to execute the specified action.
        /// </summary>
        /// <param name="message">The message which is added to the collector.</param>
        /// <param name="onCollected">The action which is executed when the complete set is found.</param>
        /// <remarks>
        /// <para>This method is helpful for <see cref="InterceptorAgent"/>s where the agent in the <see cref="InterceptorAgent.InterceptCore"/> method must wait for a set of message before returning the <see cref="InterceptionAction"/>.</para>
        /// <para>Another example is when the <see cref="InterceptorAgent"/> wants to wait on a single message. In this case the first message is the message that is intercepted. The second message is the message the agent needs. The advantage is, that the collector respects message domains.</para>
        /// </remarks>
        public void PushAndExecute(Message message, Action<MessageCollection<T1, T2, T3, T4, T5, T6>> onCollected)
        {
            ExecutePushAndExecute(message, collection => onCollected((MessageCollection<T1, T2, T3, T4, T5, T6>) collection));
        }
    }

    /// <inheritdoc />
    public class MessageCollector<T1, T2, T3, T4, T5, T6, T7> : MessageCollector<T1, T2, T3, T4, T5, T6>
        where T1 : Message
        where T2 : Message
        where T3 : Message
        where T4 : Message
        where T5 : Message
        where T6 : Message
        where T7 : Message
    {
        /// <summary>
        /// Store for messages of type <typeparamref name="T7"/>.
        /// </summary>
        protected Dictionary<MessageDomain, MessageStore<T7>> Messages7 { get; } = new Dictionary<MessageDomain, MessageStore<T7>>();
        private readonly Action<MessageCollection<T1, T2, T3, T4, T5, T6, T7>> onMessagesCollected;

        /// <inheritdoc />
        public MessageCollector(Action<MessageCollection<T1, T2, T3, T4, T5, T6, T7>> onMessagesCollected = null)
        {
            this.onMessagesCollected = onMessagesCollected;
        }

        /// <inheritdoc />
        protected override bool Aggregate(Message message, bool throwError)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            if (message.TryGet(out T7 message7))
            {
                UpdateMessagePool(message7, Messages7);
                return true;
            }

            return base.Aggregate(message, throwError);
        }

        /// <inheritdoc />
        protected override void Execute(MessageCollection messageCollection)
        {
            onMessagesCollected?.Invoke((MessageCollection<T1, T2, T3, T4, T5, T6, T7>) messageCollection);
        }

        /// <inheritdoc />
        protected override bool IsCompleted(MessageDomain domain, out MessageCollection messageCollection)
        {
            if (TryGetMessageFittingDomain(domain, Messages1, out MessageStore<T1> message1) &&
                TryGetMessageFittingDomain(domain, Messages2, out MessageStore<T2> message2) &&
                TryGetMessageFittingDomain(domain, Messages3, out MessageStore<T3> message3) &&
                TryGetMessageFittingDomain(domain, Messages4, out MessageStore<T4> message4) &&
                TryGetMessageFittingDomain(domain, Messages5, out MessageStore<T5> message5) &&
                TryGetMessageFittingDomain(domain, Messages6, out MessageStore<T6> message6) &&
                TryGetMessageFittingDomain(domain, Messages7, out MessageStore<T7> message7))
            {
                messageCollection = new MessageCollection<T1, T2, T3, T4, T5, T6, T7>(message1, message2, message3, message4, message5, message6, message7, this);
                return true;
            }

            messageCollection = null;
            return false;
        }

        /// <inheritdoc />
        protected override void RemoveMessage(Message message)
        {
            if (message is T7 message7)
            {
                Messages7.Remove(message7.MessageDomain);
            }
            else
            {
                base.RemoveMessage(message);
            }
        }

        /// <summary>
        /// Add a message to the collector and wait of the complete set to execute the specified action.
        /// </summary>
        /// <param name="message">The message which is added to the collector.</param>
        /// <param name="onCollected">The action which is executed when the complete set is found.</param>
        /// <remarks>
        /// <para>This method is helpful for <see cref="InterceptorAgent"/>s where the agent in the <see cref="InterceptorAgent.InterceptCore"/> method must wait for a set of message before returning the <see cref="InterceptionAction"/>.</para>
        /// <para>Another example is when the <see cref="InterceptorAgent"/> wants to wait on a single message. In this case the first message is the message that is intercepted. The second message is the message the agent needs. The advantage is, that the collector respects message domains.</para>
        /// </remarks>
        public void PushAndExecute(Message message, Action<MessageCollection<T1, T2, T3, T4, T5, T6, T7>> onCollected)
        {
            ExecutePushAndExecute(message, collection => onCollected((MessageCollection<T1, T2, T3, T4, T5, T6, T7>) collection));
        }
    }
}
