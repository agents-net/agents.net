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
    public class MessageCollector<T1, T2>
        where T1 : Message
        where T2 : Message
    {
        protected Dictionary<MessageDomain, MessageStore<T1>> Messages1 { get; } = new Dictionary<MessageDomain, MessageStore<T1>>();
        protected Dictionary<MessageDomain, MessageStore<T2>> Messages2 { get; } = new Dictionary<MessageDomain, MessageStore<T2>>();
        private readonly Action<MessageCollection<T1, T2>> onMessagesCollected;
        private readonly object dictionaryLock = new object();

        public MessageCollector(Action<MessageCollection<T1, T2>> onMessagesCollected = null)
        {
            this.onMessagesCollected = onMessagesCollected;
        }

        public void Push(Message message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            TryPush(message, true);
        }

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

        public IEnumerable<MessageCollection<T1, T2>> FindSetsForDomain(MessageDomain domain)
        {
            IEnumerable<MessageCollection> completedSets = GetCompleteSets(domain);
            return completedSets.Cast<MessageCollection<T1, T2>>();
        }

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
                Execute(messageSet);
                messageSet.Dispose();
            }
        }

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

        protected virtual void Execute(MessageCollection messageCollection)
        {
            onMessagesCollected?.Invoke((MessageCollection<T1, T2>) messageCollection);
        }

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
            messagePool[message.MessageDomain] = message;
        }

        private IEnumerable<MessageDomain> ThisAndFlattenedChildren(MessageDomain messageDomain)
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

    public class MessageCollector<T1, T2, T3> : MessageCollector<T1, T2>
        where T1 : Message
        where T2 : Message
        where T3 : Message
    {
        protected Dictionary<MessageDomain, MessageStore<T3>> Messages3 { get; } = new Dictionary<MessageDomain, MessageStore<T3>>();
        private readonly Action<MessageCollection<T1, T2, T3>> onMessagesCollected;

        public MessageCollector(Action<MessageCollection<T1, T2, T3>> onMessagesCollected = null)
        {
            this.onMessagesCollected = onMessagesCollected;
        }

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

        protected override void Execute(MessageCollection messageCollection)
        {
            onMessagesCollected?.Invoke((MessageCollection<T1, T2, T3>) messageCollection);
        }

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

        public new IEnumerable<MessageCollection<T1, T2, T3>> FindSetsForDomain(MessageDomain domain)
        {
            IEnumerable<MessageCollection> completedSets = GetCompleteSets(domain);
            return completedSets.Cast<MessageCollection<T1, T2, T3>>();
        }
    }

    public class MessageCollector<T1, T2, T3, T4> : MessageCollector<T1, T2, T3>
        where T1 : Message
        where T2 : Message
        where T3 : Message
        where T4 : Message
    {
        protected Dictionary<MessageDomain, MessageStore<T4>> Messages4 { get; } = new Dictionary<MessageDomain, MessageStore<T4>>();
        private readonly Action<MessageCollection<T1, T2, T3, T4>> onMessagesCollected;

        public MessageCollector(Action<MessageCollection<T1, T2, T3, T4>> onMessagesCollected = null)
        {
            this.onMessagesCollected = onMessagesCollected;
        }

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

        protected override void Execute(MessageCollection messageCollection)
        {
            onMessagesCollected?.Invoke((MessageCollection<T1, T2, T3, T4>) messageCollection);
        }

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

        public new IEnumerable<MessageCollection<T1, T2, T3, T4>> FindSetsForDomain(MessageDomain domain)
        {
            IEnumerable<MessageCollection> completedSets = GetCompleteSets(domain);
            return completedSets.Cast<MessageCollection<T1, T2, T3, T4>>();
        }
    }

    public class MessageCollector<T1, T2, T3, T4, T5> : MessageCollector<T1, T2, T3, T4>
        where T1 : Message
        where T2 : Message
        where T3 : Message
        where T4 : Message
        where T5 : Message
    {
        protected Dictionary<MessageDomain, MessageStore<T5>> Messages5 { get; } = new Dictionary<MessageDomain, MessageStore<T5>>();
        private readonly Action<MessageCollection<T1, T2, T3, T4, T5>> onMessagesCollected;

        public MessageCollector(Action<MessageCollection<T1, T2, T3, T4, T5>> onMessagesCollected = null)
        {
            this.onMessagesCollected = onMessagesCollected;
        }

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

        protected override void Execute(MessageCollection messageCollection)
        {
            onMessagesCollected?.Invoke((MessageCollection<T1, T2, T3, T4, T5>) messageCollection);
        }

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

        public new IEnumerable<MessageCollection<T1, T2, T3, T4, T5>> FindSetsForDomain(MessageDomain domain)
        {
            IEnumerable<MessageCollection> completedSets = GetCompleteSets(domain);
            return completedSets.Cast<MessageCollection<T1, T2, T3, T4, T5>>();
        }
    }

    public class MessageCollector<T1, T2, T3, T4, T5, T6> : MessageCollector<T1, T2, T3, T4, T5>
        where T1 : Message
        where T2 : Message
        where T3 : Message
        where T4 : Message
        where T5 : Message
        where T6 : Message
    {
        protected Dictionary<MessageDomain, MessageStore<T6>> Messages6 { get; } = new Dictionary<MessageDomain, MessageStore<T6>>();
        private readonly Action<MessageCollection<T1, T2, T3, T4, T5, T6>> onMessagesCollected;

        public MessageCollector(Action<MessageCollection<T1, T2, T3, T4, T5, T6>> onMessagesCollected = null)
        {
            this.onMessagesCollected = onMessagesCollected;
        }

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

        protected override void Execute(MessageCollection messageCollection)
        {
            onMessagesCollected?.Invoke((MessageCollection<T1, T2, T3, T4, T5, T6>) messageCollection);
        }

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

        protected override void RemoveMessage(Message message)
        {
            if (message is T3 message6)
            {
                Messages6.Remove(message6.MessageDomain);
            }
            else
            {
                base.RemoveMessage(message);
            }
        }

        public new IEnumerable<MessageCollection<T1, T2, T3, T4, T5, T6>> FindSetsForDomain(MessageDomain domain)
        {
            IEnumerable<MessageCollection> completedSets = GetCompleteSets(domain);
            return completedSets.Cast<MessageCollection<T1, T2, T3, T4, T5, T6>>();
        }
    }

    public class MessageCollector<T1, T2, T3, T4, T5, T6, T7> : MessageCollector<T1, T2, T3, T4, T5, T6>
        where T1 : Message
        where T2 : Message
        where T3 : Message
        where T4 : Message
        where T5 : Message
        where T6 : Message
        where T7 : Message
    {
        protected Dictionary<MessageDomain, MessageStore<T7>> Messages7 { get; } = new Dictionary<MessageDomain, MessageStore<T7>>();
        private readonly Action<MessageCollection<T1, T2, T3, T4, T5, T6, T7>> onMessagesCollected;

        public MessageCollector(Action<MessageCollection<T1, T2, T3, T4, T5, T6, T7>> onMessagesCollected = null)
        {
            this.onMessagesCollected = onMessagesCollected;
        }

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

        protected override void Execute(MessageCollection messageCollection)
        {
            onMessagesCollected?.Invoke((MessageCollection<T1, T2, T3, T4, T5, T6, T7>) messageCollection);
        }

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

        protected override void RemoveMessage(Message message)
        {
            if (message is T3 message7)
            {
                Messages7.Remove(message7.MessageDomain);
            }
            else
            {
                base.RemoveMessage(message);
            }
        }

        public new IEnumerable<MessageCollection<T1, T2, T3, T4, T5, T6, T7>> FindSetsForDomain(MessageDomain domain)
        {
            IEnumerable<MessageCollection> completedSets = GetCompleteSets(domain);
            return completedSets.Cast<MessageCollection<T1, T2, T3, T4, T5, T6, T7>>();
        }
    }
}
