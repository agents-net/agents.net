﻿#region Copyright
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
using System.Text;
using System.Threading;
#pragma warning disable CA1801
namespace Agents.Net
{
    public abstract class Message : IEquatable<Message>, IDisposable
    {
        private Message parent;
        private Message[] predecessorMessages;
        public Guid Id { get; } = Guid.NewGuid();

        protected Message(Message predecessorMessage, MessageDefinition messageDefinition,
                          params Message[] childMessages)
            : this(new[] {predecessorMessage}, messageDefinition, childMessages)
        {
        }

        protected Message(IEnumerable<Message> predecessorMessages, MessageDefinition messageDefinition, params Message[] childMessages)
        {
            Definition = messageDefinition;
            this.Children = childMessages.Concat(childMessages.SelectMany(e => e.Children));
            this.predecessorMessages = predecessorMessages.ToArray();
            foreach (Message childMessage in childMessages)
            {
                childMessage.parent = this;
            }
            MessageDomain = this.predecessorMessages.GetMessageDomain();
        }
        
        /// <summary>
        /// Replace this message with the given message.
        /// </summary>
        /// <param name="message">The message which replaces this message.</param>
        /// <remarks>
        /// This method is intended of the use case, that an <see cref="InterceptorAgent"/> wants to replace a,
        /// message with a different message. How to do this see the example.
        /// </remarks>
        /// <example>
        /// This example shows the use case how to replace a message using an <see cref="InterceptorAgent"/>
        /// <code>
        /// protected override InterceptionAction InterceptCore(Message messageData)
        /// {
        ///     Message replacingMessage = GenerateNewMessage();
        ///     messageData.ReplaceWith(replacingMessage);
        ///     OnMessage(replacingMessage);
        ///     return InterceptionAction.DoNotPublish;
        /// }
        /// </code>
        /// </example>
        public void ReplaceWith(Message message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            message.Children = Children;
            message.predecessorMessages = predecessorMessages;
            message.SwitchDomain(MessageDomain);
            message.parent = parent;
            parent.RemoveChild(this);
            parent.AddChild(message);
        }

        internal IEnumerable<Message> Predecessors => predecessorMessages;

        internal Message HeadMessage
        {
            get
            {
                Message head = this;
                while (head.parent != null)
                {
                    head = head.parent;
                }

                return head;
            }
        }

        internal Message ReplaceHead(Message newHead)
        {
            Message head = HeadMessage;
            Message replaced;
            while ((replaced = Interlocked.Exchange(ref head.parent,newHead)) != null)
            {
                head.parent = replaced;
                head = head.HeadMessage;
            }

            return head;
        }

        protected void AddChild(Message childMessage)
        {
            if (childMessage == null)
            {
                throw new ArgumentNullException(nameof(childMessage));
            }

            Children = Children.Concat(childMessage.Children).Concat(new[] {childMessage});
        }

        private void RemoveChild(Message message)
        {
            IEnumerable<Message> directChildren = Children.Where(c => c.parent == this && c != message).ToArray();
            Children = directChildren.SelectMany(c => c.Children).Concat(directChildren);
        }

        public bool Is<T>() where T : Message
        {
            return TryGet(out T _);
        }

        public T Get<T>() where T : Message
        {
            if (!(this is T result))
            {
                result = this != HeadMessage ? HeadMessage.Get<T>() : Children.OfType<T>().First();
            }

            return result;
        }

        public bool TryGet<T>(out T result) where T : Message
        {
            result = this as T;
            if (result == null)
            {
                if (this != HeadMessage)
                {
                    HeadMessage.TryGet(out result);
                }
                else
                {
                    result = Children.OfType<T>().FirstOrDefault();
                }
            }

            return result != null;
        }

        public bool TryGetPredecessor<T>(out T result) where T : Message
        {
            foreach (Message predecessorMessage in predecessorMessages)
            {
                if (predecessorMessage.TryGet(out result))
                {
                    return true;
                }
            }

            result = null;
            return false;
        }

        public MessageDomain MessageDomain { get; private set; }

        internal void SwitchDomain(MessageDomain newDomain)
        {
            MessageDomain = newDomain;
            foreach (Message child in Children)
            {
                child.MessageDomain = newDomain;
            }
        }

        public IEnumerable<Message> Children { get; set; } = Enumerable.Empty<Message>();

        public override string ToString()
        {
            StringBuilder jsonFormat = ToStringBuilder();
            return jsonFormat.ToString();
        }

        public StringBuilder ToStringBuilder()
        {
            StringBuilder jsonFormat = new StringBuilder("{\"Id\": \"");
            jsonFormat.Append(Id);
            jsonFormat.Append("\", \"Definition\": \"");
            jsonFormat.Append(Definition);
            jsonFormat.Append("\", \"Predecessors\": \"");
            jsonFormat.Append(string.Join(", ", predecessorMessages.Select(m => m.Id)));
            jsonFormat.Append("\", \"MessageDomain\": \"");
            jsonFormat.Append(MessageDomain.Root.Id);
            jsonFormat.Append("\", \"Data\": \"");
            jsonFormat.Append(DataToString());
            jsonFormat.Append("\", \"Children\": [");
            jsonFormat.Append(string.Join(", ", Children));
            jsonFormat.Append("]}");
            return jsonFormat;
        }

        protected abstract string DataToString();

        public bool Equals(Message other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((Message) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(Message left, Message right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Message left, Message right)
        {
            return !Equals(left, right);
        }

        public MessageDefinition Definition { get; }

        internal void Used()
        {
            if (Interlocked.Decrement(ref remainingUses) == 0)
            {
                Dispose();
            }
        }

        public IDisposable DelayDispose()
        {
            Interlocked.Increment(ref remainingUses);
            return new DisposableUse(this);
        }

        private int remainingUses;

        internal void SetUserCount(int userCount)
        {
            remainingUses = userCount;
            if (userCount == 0)
            {
                Dispose();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private class DisposableUse : IDisposable
        {
            private readonly Message message;

            public DisposableUse(Message message)
            {
                this.message = message;
            }

            public void Dispose()
            {
                message.Used();
            }
        }
    }
}
