#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
#pragma warning disable CA1801
namespace Agents.Net
{
    /// <summary>
    /// Base class for all messages that are executed by the agents.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Each message knows its predecessors. Predecessors are the messages, that led to this message.
    /// </para>
    /// <para>
    /// Self disposing: The message is self disposing. The message board sets the number of uses of the message equal to the number of consuming agents. Whenever an agent executed the message the <see cref="Agent"/> base class marks the message as used. Once the message is completely used up it calls the <see cref="Dispose()"/> method. The message containers <see cref="MessageCollector{T1,T2}"/> and <see cref="MessageAggregator{T}"/> do delay the disposal until the container is executed.
    /// </para>
    /// <para>
    /// Currently there is a memory leak because the predecessors are strongly referenced. Therefore all messages produced by the system are kept in memory. See https://github.com/agents-net/agents.net/issues/75 for more information and timeline.
    /// </para>
    /// </remarks>
    public abstract class Message : IEquatable<Message>, IDisposable
    {
        private Message parent;

        internal Guid[] PredecessorIds { get; private set; }

        private readonly string name;
        
        internal Type MessageType { get; }
        
        /// <summary>
        /// The id of the agent.
        /// </summary>
        /// <remarks>
        /// The id is only used for logging.
        /// </remarks>
        public Guid Id { get; private set; } = Guid.NewGuid();

        /// <summary>
        /// Initializes a new instances of this class with a single predecessor message.
        /// </summary>
        /// <param name="predecessorMessage">The predecessor message that led to this message.</param>
        /// <param name="name">Optional name of the agent. The default is the name of the type.</param>
        /// <remarks>
        /// The <paramref name="name"/> is only used for logging purposes.
        /// </remarks>
        protected Message(Message predecessorMessage, string name = null)
            : this(new[] {predecessorMessage}, name)
        {
        }

        /// <summary>
        /// Initializes a new instances of this class with multiple predecessor messages.
        /// </summary>
        /// <param name="predecessorMessages">The predecessor messages that led to this message.</param>
        /// <param name="name">Optional name of the agent. The default is the name of the type.</param>
        /// <remarks>
        /// The <paramref name="name"/> is only used for logging purposes.
        /// </remarks>
        protected Message(IEnumerable<Message> predecessorMessages, string name = null)
        {
            this.name = string.IsNullOrEmpty(name) ? GetType().Name : name;
            MessageType = GetType();
            Message[] predecessorHierarchy = predecessorMessages.SelectMany(m => m.HeadMessage.DescendantsAndSelf)
                                                                .ToArray();
            PredecessorIds = predecessorHierarchy.Select(m => m.Id)
                                                 .ToArray();
            MessageDomain = predecessorHierarchy.GetMessageDomain();
            DescendantsAndSelf = new[] {this};
        }

        internal Message(IEnumerable<Guid> predecessorIds, IEnumerable<Message> predecessorMessages, string name = null)
        {
            this.name = string.IsNullOrEmpty(name) ? GetType().Name : name;
            MessageType = GetType();
            Message[] predecessorHierarchy = predecessorMessages.SelectMany(m => m.HeadMessage.DescendantsAndSelf)
                                                                .ToArray();
            this.PredecessorIds = predecessorIds.Concat(predecessorHierarchy.Select(m => m.Id))
                                                .ToArray();
            DescendantsAndSelf = new[] {this};
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

            message.SetChild(Child);
            message.PredecessorIds = PredecessorIds;
            message.SwitchDomain(MessageDomain);
            message.parent = parent;
            message.Id = Id;
            parent?.SetChild(message);
        }

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

        /// <summary>
        /// this is an internal method used by the <see cref="MessageDecorator"/>. It should not be used outside of that.
        /// </summary>
        /// <param name="childMessage">The new child message.</param>
        protected void SetChild(Message childMessage)
        {
            Child = childMessage;
            if (Child != null)
            {
                Child.parent = this;
            }

            Descendants = Child?.Descendants.Concat(new[] {Child})
                          ?? Array.Empty<Message>();
            parent?.SetChild(this);
        }

        /// <summary>
        /// Checks whether this message is the specified type.
        /// </summary>
        /// <typeparam name="T">The message type that needs to be searched.</typeparam>
        /// <returns><c>true</c>, if this message is of the specified type.</returns>
        /// <remarks>
        /// This method is necessary because there can be a hierarchy of messages. In this case the whole stack needs to be searched for the message.
        /// </remarks>
        public bool Is<T>() where T : Message
        {
            return TryGet(out T _);
        }

        /// <summary>
        /// Looks for the specified message type and returns it.
        /// </summary>
        /// <typeparam name="T">The message type that needs to be searched.</typeparam>
        /// <returns>The message of the specified type or null if it was not found.</returns>
        /// <remarks>
        /// This method is necessary because there can be a hierarchy of messages. In this case the whole stack needs to be searched for the message.
        /// </remarks>
        /// <example>
        /// <code>
        /// protected override void ExecuteCore(Message messageData)
        /// {
        ///     //This will not work if the message is decorated
        ///     SpecificMessage message = (SpecificMessage)messageData;
        ///
        ///     //This will work either way
        ///     SpecificMessage message = messageData.Get&lt;SpecificMessage&gt;();
        /// }
        /// </code>
        /// </example>
        public T Get<T>() where T : Message
        {
            TryGet(out T result);
            return result;
        }

        /// <summary>
        /// Tries to looks for the specified message type and returns it.
        /// </summary>
        /// <param name="result">The message of the specified type.</param>
        /// <typeparam name="T">The message type that needs to be searched.</typeparam>
        /// <returns><c>true</c>, if the message was found; otherwise <c>false</c>.</returns>
        /// <remarks>
        /// This method is necessary because there can be a hierarchy of messages. In this case the whole stack needs to be searched for the message.
        /// </remarks>
        /// <example>
        /// <code>
        /// protected override void ExecuteCore(Message messageData)
        /// {
        ///     //This will not execute if the message was decorated
        ///     if(messageData is SpecificMessage message)
        ///     {
        ///         //...
        ///     }
        ///
        ///     //This will work either way
        ///     if(messageData.TryGet(out SpecificMessage message))
        ///     {
        ///         //...
        ///     }
        /// }
        /// </code>
        /// </example>
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
                    result = Descendants.OfType<T>().FirstOrDefault();
                }
            }

            return result != null;
        }

        /// <summary>
        /// The message domain in which the message was created.
        /// </summary>
        /// <remarks>
        /// For more information about message domains see <see cref="MessageDomain"/>.
        /// </remarks>
        public MessageDomain MessageDomain { get; private set; }

        internal void SwitchDomain(MessageDomain newDomain)
        {
            if (this != HeadMessage)
            {
                HeadMessage.SwitchDomain(newDomain);
            }
            MessageDomain = newDomain;
            foreach (Message descendant in Descendants)
            {
                descendant.MessageDomain = newDomain;
            }
        }

        private Message Child
        {
            get => child;
            set
            {
                child = value;
                Descendants = Child?.Descendants.Concat(new[] {Child})
                              ?? Array.Empty<Message>();
            }
        }

        private IEnumerable<Message> Descendants
        {
            get => descendants;
            set
            {
                descendants = value;
                DescendantsAndSelf = Descendants.Concat(new[] {this});
            }
        }

        internal IEnumerable<Message> DescendantsAndSelf { get; private set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return ToStringBuilder().ToString();
        }

        private StringBuilder ToStringBuilder()
        {
            StringBuilder jsonFormat = new StringBuilder("{\"Id\": \"");
            jsonFormat.Append(Id);
            jsonFormat.Append("\", \"Name\": \"");
            jsonFormat.Append(name);
            jsonFormat.Append("\", \"Predecessors\": [");
            jsonFormat.Append(string.Join(", ", PredecessorIds.Select(id => $"\"{id}\"")));
            jsonFormat.Append("], \"MessageDomain\": \"");
            jsonFormat.Append(MessageDomain?.Root.Id);
            jsonFormat.Append("\", \"Data\": \"");
            jsonFormat.Append(DataToString());
            jsonFormat.Append("\", \"Child\": ");
            jsonFormat.Append(Child?.ToStringBuilder());
            jsonFormat.Append('}');
            return jsonFormat;
        }

        /// <summary>
        /// The method which should be overriden to provide a string representation of the carried data.
        /// </summary>
        /// <returns>The string representation.</returns>
        /// <remarks>
        /// If used, it should provide a short string as it is logged multiple times. To much data would slow down the application.
        /// </remarks>
        protected abstract string DataToString();

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        /// <summary>
        /// Determines whether both messages are equal or not.
        /// </summary>
        /// <param name="left">The left message</param>
        /// <param name="right">The right message</param>
        /// <returns><c>true</c>, if both messages are equal; otherwise <c>false</c>.</returns>
        public static bool operator ==(Message left, Message right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Determines whether both messages are not equal or not.
        /// </summary>
        /// <param name="left">The left message</param>
        /// <param name="right">The right message</param>
        /// <returns><c>true</c>, if both messages are not equal; otherwise <c>false</c>.</returns>
        public static bool operator !=(Message left, Message right)
        {
            return !Equals(left, right);
        }

        internal void Used(bool propagate = false)
        {
            if (Interlocked.Decrement(ref remainingUses) == 0)
            {
                Dispose();
            }

            if (!propagate)
            {
                return;
            }

            foreach (Message descendant in Descendants)
            {
                descendant.Used();
            }

        }

        /// <summary>
        /// This delays the self disposal of the message until the returned object is disposed.
        /// </summary>
        /// <returns>The object which will release the message on dispose.</returns>
        /// <remarks>
        /// <para>
        /// This method should only be used, if there is a custom message container that stores this message.
        /// </para>
        /// <para>
        /// This regards the self disposing mechanism of the message. See the description for the type <see cref="Message"/>.
        /// </para>
        /// </remarks>
        public IDisposable DelayDispose()
        {
            Interlocked.Increment(ref remainingUses);
            return new DisposableUse(this);
        }

        private int remainingUses;
        private Message child;
        private IEnumerable<Message> descendants = Array.Empty<Message>();

        internal void SetUserCount(int userCount)
        {
            remainingUses = userCount;
            if (userCount == 0)
            {
                Dispose();
            }
        }

        /// <summary>
        /// Dispose any resources that are stored in the message.
        /// </summary>
        /// <param name="disposing">If <c>true</c> it was called from the <see cref="Dispose()"/> method.</param>
        protected virtual void Dispose(bool disposing)
        {
            remainingUses = 0;
        }

        /// <inheritdoc />
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

        /// <summary>
        /// This is used to log the message. It creates the serializable class <see cref="MessageLog"/>.
        /// </summary>
        /// <returns>The <see cref="MessageLog"/> instance.</returns>
        /// <remarks>
        /// This method is mostly used internally.
        /// </remarks>
        public MessageLog ToMessageLog()
        {
            return new MessageLog(name, Id, PredecessorIds, MessageDomain.Root.Id,
                                  DataToString(), Child?.ToMessageLog());
        }
    }
}
