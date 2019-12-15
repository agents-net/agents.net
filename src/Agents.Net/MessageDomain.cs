#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Collections.Generic;

namespace Agents.Net
{
    public class MessageDomain
    {
        private readonly List<MessageDomain> children = new List<MessageDomain>();

        public MessageDomain(Message root, MessageDomain parent)
        {
            Root = root;
            Parent = parent;
            parent?.children.Add(this);
        }

        public Message Root { get; }

        public MessageDomain Parent { get; }

        public IEnumerable<MessageDomain> Children => children;

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
