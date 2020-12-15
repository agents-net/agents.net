#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Agents.Net
{
    /// <summary>
    /// A collection of messages.
    /// </summary>
    /// <remarks>
    /// This is a helper class for the <see cref="MessageCollector{T1,T2}"/>
    /// </remarks>
    public abstract class MessageCollection : IEnumerable<Message>, IDisposable
    {
        /// <summary>
        /// When overridden needs to return all messages in the collection.
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<Message> GetAllMessages()
        {
            return Array.Empty<Message>();
        }

        /// <inheritdoc />
        public IEnumerator<Message> GetEnumerator()
        {
            return GetAllMessages().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Mark the specified message as consumed.
        /// </summary>
        /// <param name="message">The message that will be marked.</param>
        /// <remarks>
        /// The usual behavior for the <see cref="MessageCollector{T1,T2}"/> is to retain all messages that were collected, until other message of the same type replaces the the old message. This method changes that behavior, so that the marked message is removed from the collector afterwards.
        /// </remarks>
        public abstract void MarkAsConsumed(Message message);

        /// <summary>
        /// Disposes all <see cref="MessageStore{T}"/>s contained in this collection
        /// </summary>
        /// <param name="disposing"></param>
        /// <remarks>
        /// This releases the usage of the message and therefore potentially disposes the contained <see cref="Message"/>. When a message is used more than once in the <see cref="MessageCollector{T1,T2}"/>, the <see cref="Agent"/> that contains the <see cref="MessageCollector{T1,T2}"/> should mark the message with <see cref="Message.DelayDispose"/>. 
        /// </remarks>
        protected abstract void Dispose(bool disposing);

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

    /// <inheritdoc />
    public class MessageCollection<T1, T2> : MessageCollection
        where T1 : Message
        where T2 : Message
    {
        private readonly MessageStore<T1> message1;
        private readonly MessageStore<T2> message2;
        private readonly MessageCollector<T1, T2> collector;

        /// <summary>
        /// Initialized a new instance of the class <see cref="MessageCollection{T1,T2}"/>.
        /// </summary>
        /// <param name="message1">Message of type <typeparamref name="T1"/>.</param>
        /// <param name="message2">Message of type <typeparamref name="T2"/>.</param>
        /// <param name="collector">The <see cref="MessageCollector{T1,T2}"/> that produced this instance.</param>
        public MessageCollection(MessageStore<T1> message1, MessageStore<T2> message2, MessageCollector<T1, T2> collector)
        {
            this.message1 = message1;
            this.message2 = message2;
            this.collector = collector;
        }

        /// <summary>
        /// Message of type <typeparamref name="T1"/>.
        /// </summary>
        public T1 Message1 => message1;

        /// <summary>
        /// Message of type <typeparamref name="T2"/>.
        /// </summary>
        public T2 Message2 => message2;

        /// <inheritdoc />
        protected override IEnumerable<Message> GetAllMessages()
        {
            return base.GetAllMessages().Concat(new Message[] {Message1, Message2});
        }

        /// <inheritdoc />
        public override void MarkAsConsumed(Message message)
        {
            collector.Remove(message);
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                message1.Dispose();
                message2.Dispose();
            }
        }
    }

    /// <inheritdoc />
    public class MessageCollection<T1, T2, T3> : MessageCollection<T1, T2>
        where T1 : Message
        where T2 : Message
        where T3 : Message
    {
        private readonly MessageStore<T3> message3;

        /// <summary>
        /// Initialized a new instance of the class <see cref="MessageCollection{T1,T2,T3}"/>.
        /// </summary>
        /// <param name="message1">Message of type <typeparamref name="T1"/>.</param>
        /// <param name="message2">Message of type <typeparamref name="T2"/>.</param>
        /// <param name="message3">Message of type <typeparamref name="T3"/>.</param>
        /// <param name="collector">The <see cref="MessageCollector{T1,T2}"/> that produced this instance.</param>
        public MessageCollection(MessageStore<T1> message1, MessageStore<T2> message2, MessageStore<T3> message3, MessageCollector<T1, T2> collector) 
            : base(message1, message2, collector)
        {
            this.message3 = message3;
        }

        /// <summary>
        /// Message of type <typeparamref name="T3"/>.
        /// </summary>
        public T3 Message3 => message3;

        /// <inheritdoc />
        protected override IEnumerable<Message> GetAllMessages()
        {
            return base.GetAllMessages().Concat(new[] {Message3});
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                message3.Dispose();
            }
        }
    }

    /// <inheritdoc />
    public class MessageCollection<T1, T2, T3, T4> : MessageCollection<T1, T2, T3>
        where T1 : Message
        where T2 : Message
        where T3 : Message
        where T4 : Message
    {
        private readonly MessageStore<T4> message4;

        /// <summary>
        /// Initialized a new instance of the class <see cref="MessageCollection{T1,T2,T3,T4}"/>.
        /// </summary>
        /// <param name="message1">Message of type <typeparamref name="T1"/>.</param>
        /// <param name="message2">Message of type <typeparamref name="T2"/>.</param>
        /// <param name="message3">Message of type <typeparamref name="T3"/>.</param>
        /// <param name="message4">Message of type <typeparamref name="T4"/>.</param>
        /// <param name="collector">The <see cref="MessageCollector{T1,T2}"/> that produced this instance.</param>
        public MessageCollection(MessageStore<T1> message1, MessageStore<T2> message2, MessageStore<T3> message3, MessageStore<T4> message4, MessageCollector<T1, T2> collector) 
            : base(message1, message2, message3, collector)
        {
            this.message4 = message4;
        }

        /// <summary>
        /// Message of type <typeparamref name="T4"/>.
        /// </summary>
        public T4 Message4 => message4;

        /// <inheritdoc />
        protected override IEnumerable<Message> GetAllMessages()
        {
            return base.GetAllMessages().Concat(new[] {Message4});
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                message4.Dispose();
            }
        }
    }

    /// <inheritdoc />
    public class MessageCollection<T1, T2, T3, T4, T5> : MessageCollection<T1, T2, T3, T4>
        where T1 : Message
        where T2 : Message
        where T3 : Message
        where T4 : Message
        where T5 : Message
    {
        private readonly MessageStore<T5> message5;

        /// <summary>
        /// Initialized a new instance of the class <see cref="MessageCollection{T1,T2,T3,T4,T5}"/>.
        /// </summary>
        /// <param name="message1">Message of type <typeparamref name="T1"/>.</param>
        /// <param name="message2">Message of type <typeparamref name="T2"/>.</param>
        /// <param name="message3">Message of type <typeparamref name="T3"/>.</param>
        /// <param name="message4">Message of type <typeparamref name="T4"/>.</param>
        /// <param name="message5">Message of type <typeparamref name="T5"/>.</param>
        /// <param name="collector">The <see cref="MessageCollector{T1,T2}"/> that produced this instance.</param>
        public MessageCollection(MessageStore<T1> message1, MessageStore<T2> message2, MessageStore<T3> message3, MessageStore<T4> message4, MessageStore<T5> message5, MessageCollector<T1, T2> collector) 
            : base(message1, message2, message3, message4, collector)
        {
            this.message5 = message5;
        }

        /// <summary>
        /// Message of type <typeparamref name="T5"/>.
        /// </summary>
        public T5 Message5 => message5;

        /// <inheritdoc />
        protected override IEnumerable<Message> GetAllMessages()
        {
            return base.GetAllMessages().Concat(new[] {Message5});
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                message5.Dispose();
            }
        }
    }

    /// <inheritdoc />
    public class MessageCollection<T1, T2, T3, T4, T5, T6> : MessageCollection<T1, T2, T3, T4, T5>
        where T1 : Message
        where T2 : Message
        where T3 : Message
        where T4 : Message
        where T5 : Message
        where T6 : Message
    {
        private readonly MessageStore<T6> message6;

        /// <summary>
        /// Initialized a new instance of the class <see cref="MessageCollection{T1,T2,T3,T4,T5,T6}"/>.
        /// </summary>
        /// <param name="message1">Message of type <typeparamref name="T1"/>.</param>
        /// <param name="message2">Message of type <typeparamref name="T2"/>.</param>
        /// <param name="message3">Message of type <typeparamref name="T3"/>.</param>
        /// <param name="message4">Message of type <typeparamref name="T4"/>.</param>
        /// <param name="message5">Message of type <typeparamref name="T5"/>.</param>
        /// <param name="message6">Message of type <typeparamref name="T6"/>.</param>
        /// <param name="collector">The <see cref="MessageCollector{T1,T2}"/> that produced this instance.</param>
        public MessageCollection(MessageStore<T1> message1, MessageStore<T2> message2, MessageStore<T3> message3, MessageStore<T4> message4, MessageStore<T5> message5, MessageStore<T6> message6, MessageCollector<T1, T2> collector) 
            : base(message1, message2, message3, message4, message5, collector)
        {
            this.message6 = message6;
        }

        /// <summary>
        /// Message of type <typeparamref name="T6"/>.
        /// </summary>
        public T6 Message6 => message6;

        /// <inheritdoc />
        protected override IEnumerable<Message> GetAllMessages()
        {
            return base.GetAllMessages().Concat(new[] {Message6});
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                message6.Dispose();
            }
        }
    }

    /// <inheritdoc />
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

        /// <summary>
        /// Initialized a new instance of the class <see cref="MessageCollection{T1,T2,T3,T4,T5,T6,T7}"/>.
        /// </summary>
        /// <param name="message1">Message of type <typeparamref name="T1"/>.</param>
        /// <param name="message2">Message of type <typeparamref name="T2"/>.</param>
        /// <param name="message3">Message of type <typeparamref name="T3"/>.</param>
        /// <param name="message4">Message of type <typeparamref name="T4"/>.</param>
        /// <param name="message5">Message of type <typeparamref name="T5"/>.</param>
        /// <param name="message6">Message of type <typeparamref name="T6"/>.</param>
        /// <param name="message7">Message of type <typeparamref name="T7"/>.</param>
        /// <param name="collector">The <see cref="MessageCollector{T1,T2}"/> that produced this instance.</param>
        public MessageCollection(MessageStore<T1> message1, MessageStore<T2> message2, MessageStore<T3> message3, MessageStore<T4> message4, MessageStore<T5> message5, MessageStore<T6> message6, MessageStore<T7> message7, MessageCollector<T1, T2> collector) 
            : base(message1, message2, message3, message4, message5, message6, collector)
        {
            this.message7 = message7;
        }

        /// <summary>
        /// Message of type <typeparamref name="T7"/>.
        /// </summary>
        public T7 Message7 => message7;

        /// <inheritdoc />
        protected override IEnumerable<Message> GetAllMessages()
        {
            return base.GetAllMessages().Concat(new[] {Message7});
        }

        /// <inheritdoc />
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
