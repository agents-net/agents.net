﻿#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
//
///////////////////////////////////////////////////////////////////////////////
#endregion

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
            AddChild(decoratedMessage.ReplaceHead(this));
        }
    }
}