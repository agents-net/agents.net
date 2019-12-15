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
        protected readonly Dictionary<MessageDomain, T1> Messages1 = new Dictionary<MessageDomain, T1>();
        protected readonly Dictionary<MessageDomain, T2> Messages2 = new Dictionary<MessageDomain, T2>();
        private readonly Action<MessageSet<T1, T2>> executeSet;
        private readonly object dictionaryLock = new object();

        public MessageCollector(Action<MessageSet<T1, T2>> executeSet = null)
        {
            this.executeSet = executeSet;
        }

        public void Push(Message message)
        {
            IEnumerable<MessageSet> completedSets;
            lock (dictionaryLock)
            {
                Aggregate(message);
                completedSets = GetCompleteSets(message.MessageDomain);
            }
            ExecuteCompleteSets(completedSets);
        }

        private IEnumerable<MessageSet> GetCompleteSets(MessageDomain domain)
        {
            HashSet<MessageSet> sets = new HashSet<MessageSet>(new MessageSetIdComparer());
            foreach (MessageDomain messageDomain in ThisAndFlattenedChildren(domain).Where(d => !d.IsTerminated))
            {
                if (IsCompleted(messageDomain, out MessageSet set))
                {
                    sets.Add(set);
                }
            }

            return sets;
        }

        private void ExecuteCompleteSets(IEnumerable<MessageSet> sets)
        {
            foreach (MessageSet messageSet in sets)
            {
                Execute(messageSet);
            }
        }

        protected virtual bool IsCompleted(MessageDomain domain, out MessageSet messageSet)
        {
            if (TryGetMessageFittingDomain(domain, Messages1, out T1 message1) &&
                TryGetMessageFittingDomain(domain, Messages2, out T2 message2))
            {
                messageSet = new MessageSet<T1, T2>(message1, message2);
                return true;
            }

            messageSet = null;
            return false;
        }

        protected bool TryGetMessageFittingDomain<T>(MessageDomain domain, Dictionary<MessageDomain, T> messagePool, out T message)
            where T : Message
        {
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

        protected virtual void Execute(MessageSet messageSet)
        {
            executeSet?.Invoke((MessageSet<T1, T2>) messageSet);
        }

        protected virtual void Aggregate(Message message)
        {
            if (message.TryGet(out T1 message1))
            {
                UpdateMessagePool(message1, Messages1);
            }
            else if (message.TryGet(out T2 message2))
            {
                UpdateMessagePool(message2, Messages2);
            }
            else
            {
                throw new InvalidOperationException($"{message} does not contain any expected message of {GetType()}");
            }
        }

        protected void UpdateMessagePool<T>(T message, Dictionary<MessageDomain, T> messagePool)
            where T : Message
        {
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

        private class MessageSetIdComparer : IEqualityComparer<MessageSet>
        {
            public bool Equals(MessageSet x, MessageSet y)
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

            public int GetHashCode(MessageSet obj)
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
        protected readonly Dictionary<MessageDomain, T3> Messages3 = new Dictionary<MessageDomain, T3>();
        private readonly Action<MessageSet<T1, T2, T3>> executeSet;

        public MessageCollector(Action<MessageSet<T1, T2, T3>> executeSet = null)
        {
            this.executeSet = executeSet;
        }

        protected override void Aggregate(Message message)
        {
            if (message.TryGet(out T3 message3))
            {
                UpdateMessagePool(message3, Messages3);
            }
            else
            {
                base.Aggregate(message);
            }
        }

        protected override void Execute(MessageSet messageSet)
        {
            executeSet?.Invoke((MessageSet<T1, T2, T3>) messageSet);
        }

        protected override bool IsCompleted(MessageDomain domain, out MessageSet messageSet)
        {
            if (TryGetMessageFittingDomain(domain, Messages1, out T1 message1) &&
                TryGetMessageFittingDomain(domain, Messages2, out T2 message2) &&
                TryGetMessageFittingDomain(domain, Messages3, out T3 message3))
            {
                messageSet = new MessageSet<T1, T2, T3>(message1, message2, message3);
                return true;
            }

            messageSet = null;
            return false;
        }
    }

    public class MessageCollector<T1, T2, T3, T4> : MessageCollector<T1, T2, T3>
        where T1 : Message
        where T2 : Message
        where T3 : Message
        where T4 : Message
    {
        protected readonly Dictionary<MessageDomain, T4> Messages4 = new Dictionary<MessageDomain, T4>();
        private readonly Action<MessageSet<T1, T2, T3, T4>> executeSet;

        public MessageCollector(Action<MessageSet<T1, T2, T3, T4>> executeSet = null)
        {
            this.executeSet = executeSet;
        }

        protected override void Aggregate(Message message)
        {
            if (message.TryGet(out T4 message4))
            {
                UpdateMessagePool(message4, Messages4);
            }
            else
            {
                base.Aggregate(message);
            }
        }

        protected override void Execute(MessageSet messageSet)
        {
            executeSet?.Invoke((MessageSet<T1, T2, T3, T4>) messageSet);
        }

        protected override bool IsCompleted(MessageDomain domain, out MessageSet messageSet)
        {
            if (TryGetMessageFittingDomain(domain, Messages1, out T1 message1) &&
                TryGetMessageFittingDomain(domain, Messages2, out T2 message2) &&
                TryGetMessageFittingDomain(domain, Messages3, out T3 message3) &&
                TryGetMessageFittingDomain(domain, Messages4, out T4 message4))
            {
                messageSet = new MessageSet<T1, T2, T3, T4>(message1, message2, message3, message4);
                return true;
            }

            messageSet = null;
            return false;
        }
    }

    public class MessageCollector<T1, T2, T3, T4, T5> : MessageCollector<T1, T2, T3, T4>
        where T1 : Message
        where T2 : Message
        where T3 : Message
        where T4 : Message
        where T5 : Message
    {
        protected readonly Dictionary<MessageDomain, T5> Messages5 = new Dictionary<MessageDomain, T5>();
        private readonly Action<MessageSet<T1, T2, T3, T4, T5>> executeSet;

        public MessageCollector(Action<MessageSet<T1, T2, T3, T4, T5>> executeSet = null)
        {
            this.executeSet = executeSet;
        }

        protected override void Aggregate(Message message)
        {
            if (message.TryGet(out T5 message5))
            {
                UpdateMessagePool(message5, Messages5);
            }
            else
            {
                base.Aggregate(message);
            }
        }

        protected override void Execute(MessageSet messageSet)
        {
            executeSet?.Invoke((MessageSet<T1, T2, T3, T4, T5>) messageSet);
        }

        protected override bool IsCompleted(MessageDomain domain, out MessageSet messageSet)
        {
            if (TryGetMessageFittingDomain(domain, Messages1, out T1 message1) &&
                TryGetMessageFittingDomain(domain, Messages2, out T2 message2) &&
                TryGetMessageFittingDomain(domain, Messages3, out T3 message3) &&
                TryGetMessageFittingDomain(domain, Messages4, out T4 message4) &&
                TryGetMessageFittingDomain(domain, Messages5, out T5 message5))
            {
                messageSet = new MessageSet<T1, T2, T3, T4, T5>(message1, message2, message3, message4, message5);
                return true;
            }

            messageSet = null;
            return false;
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
        protected readonly Dictionary<MessageDomain, T6> Messages6 = new Dictionary<MessageDomain, T6>();
        private readonly Action<MessageSet<T1, T2, T3, T4, T5, T6>> executeSet;

        public MessageCollector(Action<MessageSet<T1, T2, T3, T4, T5, T6>> executeSet = null)
        {
            this.executeSet = executeSet;
        }

        protected override void Aggregate(Message message)
        {
            if (message.TryGet(out T6 message6))
            {
                UpdateMessagePool(message6, Messages6);
            }
            else
            {
                base.Aggregate(message);
            }
        }

        protected override void Execute(MessageSet messageSet)
        {
            executeSet?.Invoke((MessageSet<T1, T2, T3, T4, T5, T6>) messageSet);
        }

        protected override bool IsCompleted(MessageDomain domain, out MessageSet messageSet)
        {
            if (TryGetMessageFittingDomain(domain, Messages1, out T1 message1) &&
                TryGetMessageFittingDomain(domain, Messages2, out T2 message2) &&
                TryGetMessageFittingDomain(domain, Messages3, out T3 message3) &&
                TryGetMessageFittingDomain(domain, Messages4, out T4 message4) &&
                TryGetMessageFittingDomain(domain, Messages5, out T5 message5) &&
                TryGetMessageFittingDomain(domain, Messages6, out T6 message6))
            {
                messageSet = new MessageSet<T1, T2, T3, T4, T5, T6>(message1, message2, message3, message4, message5, message6);
                return true;
            }

            messageSet = null;
            return false;
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
        protected readonly Dictionary<MessageDomain, T7> Messages7 = new Dictionary<MessageDomain, T7>();
        private readonly Action<MessageSet<T1, T2, T3, T4, T5, T6, T7>> executeSet;

        public MessageCollector(Action<MessageSet<T1, T2, T3, T4, T5, T6, T7>> executeSet = null)
        {
            this.executeSet = executeSet;
        }

        protected override void Aggregate(Message message)
        {
            if (message.TryGet(out T7 message7))
            {
                UpdateMessagePool(message7, Messages7);
            }
            else
            {
                base.Aggregate(message);
            }
        }

        protected override void Execute(MessageSet messageSet)
        {
            executeSet?.Invoke((MessageSet<T1, T2, T3, T4, T5, T6, T7>) messageSet);
        }

        protected override bool IsCompleted(MessageDomain domain, out MessageSet messageSet)
        {
            if (TryGetMessageFittingDomain(domain, Messages1, out T1 message1) &&
                TryGetMessageFittingDomain(domain, Messages2, out T2 message2) &&
                TryGetMessageFittingDomain(domain, Messages3, out T3 message3) &&
                TryGetMessageFittingDomain(domain, Messages4, out T4 message4) &&
                TryGetMessageFittingDomain(domain, Messages5, out T5 message5) &&
                TryGetMessageFittingDomain(domain, Messages6, out T6 message6) &&
                TryGetMessageFittingDomain(domain, Messages7, out T7 message7))
            {
                messageSet = new MessageSet<T1, T2, T3, T4, T5, T6, T7>(message1, message2, message3, message4, message5, message6, message7);
                return true;
            }

            messageSet = null;
            return false;
        }
    }
}
