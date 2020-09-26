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
    public abstract class MessageDecorator : Message
    {
        protected MessageDecorator(Message decoratedMessage, IEnumerable<Message> additionalPredecessors = null) 
            : base(decoratedMessage?.Predecessors.Concat(additionalPredecessors ?? Enumerable.Empty<Message>()).Distinct()
            ??Enumerable.Empty<Message>())
        {
            SwitchDomain(decoratedMessage?.MessageDomain);
            SetChild(decoratedMessage?.ReplaceHead(this));
        }

        public static bool IsDecorated(Message message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            return message.Is<MessageDecorator>();
        }
    }
}
