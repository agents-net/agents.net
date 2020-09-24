using System;

namespace Agents.Net
{
    public class AgentLog
    {
        public AgentLog(string agent, string type, Guid agentId, MessageLog message)
        {
            Agent = agent;
            Type = type;
            AgentId = agentId;
            Message = message;
        }

        public string Agent { get; }
        public string Type { get; }
        public Guid AgentId { get; }
        public MessageLog Message { get; }
    }
}