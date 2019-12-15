#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
//
///////////////////////////////////////////////////////////////////////////////
#endregion

namespace Agents.Net
{
    public class AgentDefinition
    {
        public AgentDefinition(MessageDefinition[] consumingTriggers, MessageDefinition[] producingTriggers)
        {
            ConsumingTriggers = consumingTriggers;
            ProducingTriggers = producingTriggers;
        }

        public MessageDefinition[] ConsumingTriggers { get; }

        public MessageDefinition[] ProducingTriggers { get; }
    }
}
