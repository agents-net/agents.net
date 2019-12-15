#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Agents.Net
{
    public class MessageSet : IEnumerable<Message>
    {
        protected virtual IEnumerable<Message> GetAllMessages()
        {
            return new Message[0];
        }

        public IEnumerator<Message> GetEnumerator()
        {
            return GetAllMessages().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class MessageSet<T1, T2> : MessageSet
        where T1 : Message
        where T2 : Message
    {
        public MessageSet(T1 message1, T2 message2)
        {
            Message1 = message1;
            Message2 = message2;
        }

        public T1 Message1 { get; }
        public T2 Message2 { get; }

        protected override IEnumerable<Message> GetAllMessages()
        {
            return base.GetAllMessages().Concat(new Message[] {Message1, Message2});
        }
    }

    public class MessageSet<T1, T2, T3> : MessageSet<T1, T2>
        where T1 : Message
        where T2 : Message
        where T3 : Message
    {
        public MessageSet(T1 message1, T2 message2, T3 message3) : base(message1, message2)
        {
            Message3 = message3;
        }

        public T3 Message3 { get; }

        protected override IEnumerable<Message> GetAllMessages()
        {
            return base.GetAllMessages().Concat(new[] {Message3});
        }
    }

    public class MessageSet<T1, T2, T3, T4> : MessageSet<T1, T2, T3>
        where T1 : Message
        where T2 : Message
        where T3 : Message
        where T4 : Message
    {
        public MessageSet(T1 message1, T2 message2, T3 message3, T4 message4) : base(message1, message2, message3)
        {
            Message4 = message4;
        }

        public T4 Message4 { get; }

        protected override IEnumerable<Message> GetAllMessages()
        {
            return base.GetAllMessages().Concat(new[] {Message4});
        }
    }

    public class MessageSet<T1, T2, T3, T4, T5> : MessageSet<T1, T2, T3, T4>
        where T1 : Message
        where T2 : Message
        where T3 : Message
        where T4 : Message
        where T5 : Message
    {
        public MessageSet(T1 message1, T2 message2, T3 message3, T4 message4, T5 message5) : base(message1, message2, message3, message4)
        {
            Message5 = message5;
        }

        public T5 Message5 { get; }

        protected override IEnumerable<Message> GetAllMessages()
        {
            return base.GetAllMessages().Concat(new[] {Message5});
        }
    }

    public class MessageSet<T1, T2, T3, T4, T5, T6> : MessageSet<T1, T2, T3, T4, T5>
        where T1 : Message
        where T2 : Message
        where T3 : Message
        where T4 : Message
        where T5 : Message
        where T6 : Message
    {
        public MessageSet(T1 message1, T2 message2, T3 message3, T4 message4, T5 message5, T6 message6) : base(message1, message2, message3, message4, message5)
        {
            Message6 = message6;
        }

        public T6 Message6 { get; }

        protected override IEnumerable<Message> GetAllMessages()
        {
            return base.GetAllMessages().Concat(new[] {Message6});
        }
    }

    public class MessageSet<T1, T2, T3, T4, T5, T6, T7> : MessageSet<T1, T2, T3, T4, T5, T6>
        where T1 : Message
        where T2 : Message
        where T3 : Message
        where T4 : Message
        where T5 : Message
        where T6 : Message
        where T7 : Message
    {
        public MessageSet(T1 message1, T2 message2, T3 message3, T4 message4, T5 message5, T6 message6, T7 message7) : base(message1, message2, message3, message4, message5, message6)
        {
            Message7 = message7;
        }

        public T7 Message7 { get; }

        protected override IEnumerable<Message> GetAllMessages()
        {
            return base.GetAllMessages().Concat(new[] {Message7});
        }
    }
}
