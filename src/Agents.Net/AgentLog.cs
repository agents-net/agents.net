#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;

namespace Agents.Net
{
    /// <summary>
    /// This class is used to log an agent interaction in JSON format.
    /// </summary>
    /// <remarks>
    /// It can be used for analysing tests or the log.
    /// </remarks>
    public class AgentLog
    {
        /// <summary>
        /// Initialized a new instance of the class <see cref="AgentLog"/>.
        /// </summary>
        /// <param name="agent">Name of the agent.</param>
        /// <param name="type">The type of the interaction.</param>
        /// <param name="agentId">The agent id.</param>
        /// <param name="message">The message formatted as a <see cref="MessageLog"/></param>
        public AgentLog(string agent, string type, Guid agentId, MessageLog message)
        {
            Agent = agent;
            Type = type;
            AgentId = agentId;
            Message = message;
        }

        /// <summary>
        /// Get the name of the agent.
        /// </summary>
        public string Agent { get; }
        /// <summary>
        /// Get the type o the interaction
        /// </summary>
        /// <remarks>
        /// Currently these are "Executing", "Executed", "Intercepting" and "Publishing" 
        /// </remarks>
        public string Type { get; }
        /// <summary>
        /// Get the id of the agent.
        /// </summary>
        /// <remarks>
        /// Only relevant to find out if there are more than one instance of a specific agent.
        /// </remarks>
        public Guid AgentId { get; }
        /// <summary>
        /// Get the message, formatted as <see cref="MessageLog"/>.
        /// </summary>
        public MessageLog Message { get; }
    }
}