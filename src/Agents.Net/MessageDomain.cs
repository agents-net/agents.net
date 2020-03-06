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

namespace Agents.Net
{
    public class MessageDomain : IEquatable<MessageDomain>
    {
        private readonly List<MessageDomain> children = new List<MessageDomain>();

        public MessageDomain(Message root, MessageDomain parent, MessageDomainsCreatedMessage createdMessage = null)
        {
            Root = root;
            Parent = parent;
            CreatedMessage = createdMessage;
            lock (parent?.children??new object())
            {
                parent?.children.Add(this);
            }
        }

        public Message Root { get; }

        public MessageDomain Parent { get; }

        public MessageDomainsCreatedMessage CreatedMessage { get; }

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
    }
}
