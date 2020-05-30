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
    public class InterceptorAgentDefinition : AgentDefinition
    {
        public IReadOnlyCollection<MessageDefinition> InterceptedMessages { get; }

        public InterceptorAgentDefinition(MessageDefinition[] interceptedMessages,
                                        MessageDefinition[] producingTriggers,
                                        MessageDefinition[] additionalConsumingTriggers = null)
            : base(additionalConsumingTriggers ?? Array.Empty<MessageDefinition>(), producingTriggers)
        {
            InterceptedMessages = interceptedMessages;
        }
    }
}
