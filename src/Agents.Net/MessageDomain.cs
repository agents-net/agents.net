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
    public class MessageDomain : IEquatable<MessageDomain>
    {
        public static MessageDomain DefaultMessageDomain { get; } = new MessageDomain(new DefaultMessageDomainMessage(), null);

        private readonly List<MessageDomain> children = new List<MessageDomain>();

        private MessageDomain(Message root, MessageDomain parent, IReadOnlyCollection<Message> siblingDomainRootMessages = null)
        {
            Root = root;
            Parent = parent;
            SiblingDomainRootMessages = siblingDomainRootMessages ?? Array.Empty<Message>();
            lock (parent?.children??new object())
            {
                parent?.children.Add(this);
            }
        }

        public static MessageDomainsCreatedMessage CreateNewDomainsFor(Message newDomainRootMessage)
        {
            return CreateNewDomainsFor(new[] {newDomainRootMessage});
        }

        public static MessageDomainsCreatedMessage CreateNewDomainsFor(IReadOnlyCollection<Message> newDomainRootMessages)
        {
            if (newDomainRootMessages == null)
            {
                throw new ArgumentNullException(nameof(newDomainRootMessages));
            }
            CreateDomains();
            return new MessageDomainsCreatedMessage(newDomainRootMessages, newDomainRootMessages.SelectMany(m => m.Predecessors).Distinct());

            void CreateDomains()
            {
                foreach (Message rootMessage in newDomainRootMessages)
                {
                    rootMessage.SwitchDomain(new MessageDomain(rootMessage, rootMessage.MessageDomain, newDomainRootMessages));
                }
            }
        }

        public Message Root { get; }

        public MessageDomain Parent { get; }

        public IReadOnlyCollection<Message> SiblingDomainRootMessages { get; }

        public IReadOnlyCollection<MessageDomain> Children
        {
            get
            {
                lock (children)
                {
                    return children.ToArray();
                }
            }
        }

        public bool Equals(MessageDomain other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Equals(Root, other.Root);
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

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((MessageDomain) obj);
        }

        public override int GetHashCode()
        {
            return (Root != null ? Root.GetHashCode() : 0);
        }

        public static bool operator ==(MessageDomain left, MessageDomain right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(MessageDomain left, MessageDomain right)
        {
            return !Equals(left, right);
        }

        public bool IsTerminated { get; private set; }

        internal void Terminate()
        {
            foreach (MessageDomain messageDomain in Children)
            {
                messageDomain.Terminate();
            }
            IsTerminated = true;
        }

        private class DefaultMessageDomainMessage : Message
        {
            public DefaultMessageDomainMessage() : base(Array.Empty<Message>(),new MessageDefinition("DefaultDomain"))
            {
            }

            protected override string DataToString()
            {
                return string.Empty;
            }
        }
    }
}
