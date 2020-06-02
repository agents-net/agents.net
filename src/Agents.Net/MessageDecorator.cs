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
        protected MessageDecorator(Message decoratedMessage, MessageDefinition messageDefinition, IEnumerable<Message> additionalPredecessors = null) 
            : base(decoratedMessage.Predecessors.Concat(additionalPredecessors ?? Enumerable.Empty<Message>()).Distinct(), 
                   messageDefinition)
        {
            SwitchDomain(decoratedMessage.MessageDomain);
            AddChild(decoratedMessage.ReplaceHead(this));
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
