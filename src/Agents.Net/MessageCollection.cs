#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Agents.Net
{
    public abstract class MessageCollection : IEnumerable<Message>, IDisposable
    {
        protected virtual IEnumerable<Message> GetAllMessages()
        {
            return Array.Empty<Message>();
        }

        public IEnumerator<Message> GetEnumerator()
        {
            return GetAllMessages().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected abstract void Dispose(bool disposing);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

    public class MessageCollection<T1, T2> : MessageCollection
        where T1 : Message
        where T2 : Message
    {
        private readonly MessageStore<T1> message1;
        private readonly MessageStore<T2> message2;

        public MessageCollection(MessageStore<T1> message1, MessageStore<T2> message2)
        {
            this.message1 = message1;
            this.message2 = message2;
        }

        public T1 Message1 => message1;

        public T2 Message2 => message2;

        protected override IEnumerable<Message> GetAllMessages()
        {
            return base.GetAllMessages().Concat(new Message[] {Message1, Message2});
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                message1.Dispose();
                message2.Dispose();
            }
        }
    }

    public class MessageCollection<T1, T2, T3> : MessageCollection<T1, T2>
        where T1 : Message
        where T2 : Message
        where T3 : Message
    {
        private readonly MessageStore<T3> message3;

        public MessageCollection(MessageStore<T1> message1, MessageStore<T2> message2, MessageStore<T3> message3) : base(message1, message2)
        {
            this.message3 = message3;
        }

        public T3 Message3 => message3;

        protected override IEnumerable<Message> GetAllMessages()
        {
            return base.GetAllMessages().Concat(new[] {Message3});
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                message3.Dispose();
            }
        }
    }

    public class MessageCollection<T1, T2, T3, T4> : MessageCollection<T1, T2, T3>
        where T1 : Message
        where T2 : Message
        where T3 : Message
        where T4 : Message
    {
        private readonly MessageStore<T4> message4;

        public MessageCollection(MessageStore<T1> message1, MessageStore<T2> message2, MessageStore<T3> message3, MessageStore<T4> message4) : base(message1, message2, message3)
        {
            this.message4 = message4;
        }

        public T4 Message4 => message4;

        protected override IEnumerable<Message> GetAllMessages()
        {
            return base.GetAllMessages().Concat(new[] {Message4});
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                message4.Dispose();
            }
        }
    }

    public class MessageCollection<T1, T2, T3, T4, T5> : MessageCollection<T1, T2, T3, T4>
        where T1 : Message
        where T2 : Message
        where T3 : Message
        where T4 : Message
        where T5 : Message
    {
        private readonly MessageStore<T5> message5;

        public MessageCollection(MessageStore<T1> message1, MessageStore<T2> message2, MessageStore<T3> message3, MessageStore<T4> message4, MessageStore<T5> message5) : base(message1, message2, message3, message4)
        {
            this.message5 = message5;
        }

        public T5 Message5 => message5;

        protected override IEnumerable<Message> GetAllMessages()
        {
            return base.GetAllMessages().Concat(new[] {Message5});
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                message5.Dispose();
            }
        }
    }

    public class MessageCollection<T1, T2, T3, T4, T5, T6> : MessageCollection<T1, T2, T3, T4, T5>
        where T1 : Message
        where T2 : Message
        where T3 : Message
        where T4 : Message
        where T5 : Message
        where T6 : Message
    {
        private readonly MessageStore<T6> message6;

        public MessageCollection(MessageStore<T1> message1, MessageStore<T2> message2, MessageStore<T3> message3, MessageStore<T4> message4, MessageStore<T5> message5, MessageStore<T6> message6) : base(message1, message2, message3, message4, message5)
        {
            this.message6 = message6;
        }

        public T6 Message6 => message6;

        protected override IEnumerable<Message> GetAllMessages()
        {
            return base.GetAllMessages().Concat(new[] {Message6});
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                message6.Dispose();
            }
        }
    }

    public class MessageCollection<T1, T2, T3, T4, T5, T6, T7> : MessageCollection<T1, T2, T3, T4, T5, T6>
        where T1 : Message
        where T2 : Message
        where T3 : Message
        where T4 : Message
        where T5 : Message
        where T6 : Message
        where T7 : Message
    {
        private readonly MessageStore<T7> message7;

        public MessageCollection(MessageStore<T1> message1, MessageStore<T2> message2, MessageStore<T3> message3, MessageStore<T4> message4, MessageStore<T5> message5, MessageStore<T6> message6, MessageStore<T7> message7) : base(message1, message2, message3, message4, message5, message6)
        {
            this.message7 = message7;
        }

        public T7 Message7 => message7;

        protected override IEnumerable<Message> GetAllMessages()
        {
            return base.GetAllMessages().Concat(new[] {Message7});
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                message7.Dispose();
            }
        }
    }
}
