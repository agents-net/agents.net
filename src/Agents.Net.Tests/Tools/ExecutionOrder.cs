using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Agents.Net.Tests.Tools.Log;

namespace Agents.Net.Tests.Tools
{
    public class ExecutionOrder
    {
        private readonly List<ExecutionOrderStep> steps = new List<ExecutionOrderStep>();
        private readonly Dictionary<string, AgentCollector> activeAgents = new Dictionary<string, AgentCollector>();
        private ExecutionOrderStep activeStep;
        private readonly object syncRoot = new object();

        public void Add(AgentLog log)
        {
            lock (syncRoot)
            {
                if (log.Type == "Executing")
                {
                    if (!activeAgents.ContainsKey(log.Agent))
                    {
                        activeAgents.Add(log.Agent, new AgentCollector(log.Agent));
                    }

                    activeAgents[log.Agent].Collect(log.Message);
                    if (activeStep == null && steps.LastOrDefault()?.Contains(activeAgents[log.Agent]) == false)
                    {
                        activeStep = new ExecutionOrderStep();
                    }

                    activeStep?.Collect(activeAgents[log.Agent]);
                }
                else
                {
                    //Add produced message
                    //First time add collecting agent into steps based on last produced message
                    //Afterward only add produced messages
                }
            }
        }

        private class AgentCollector
        {
            private readonly string agent;
            private readonly Dictionary<Guid, MessageLog> incomingMessages = new Dictionary<Guid, MessageLog>();

            public AgentCollector(string agent)
            {
                this.agent = agent;
            }

            public void Collect(MessageLog message)
            {
                incomingMessages.Add(message.Id, message);
            }
        }

        private class ExecutionOrderStep
        {
            private readonly HashSet<AgentCollector> activeAgents = new HashSet<AgentCollector>();

            public void Collect(AgentCollector agentCollector)
            {
                activeAgents.Add(agentCollector);
            }

            public bool Contains(AgentCollector agentCollector)
            {
                return activeAgents.Contains(agentCollector);
            }
        }
    }
}
