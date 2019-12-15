#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;

namespace Agents.Net
{
    public class DecoratorAgentDefinition : AgentDefinition
    {
        public MessageDefinition[] DecoratedMessages { get; }

        public DecoratorAgentDefinition(MessageDefinition[] decoratedMessages,
                                      MessageDefinition[] producingTriggers,
                                      MessageDefinition[] additionalConsumingTriggers = null)
            : base(additionalConsumingTriggers ?? Array.Empty<MessageDefinition>(), producingTriggers)
        {
            DecoratedMessages = decoratedMessages;
        }
    }
}
