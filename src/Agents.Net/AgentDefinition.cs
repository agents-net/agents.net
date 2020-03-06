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
    public class AgentDefinition
    {
        public AgentDefinition(MessageDefinition[] consumingTriggers, MessageDefinition[] producingTriggers)
        {
            ConsumingTriggers = consumingTriggers;
            ProducingTriggers = producingTriggers;
        }

        public IReadOnlyCollection<MessageDefinition> ConsumingTriggers { get; }

        public IReadOnlyCollection<MessageDefinition> ProducingTriggers { get; }
    }
}
