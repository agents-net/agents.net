using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;

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
                    if (activeStep == null && steps.LastOrDefault()?.Contains(activeAgents[log.Agent]) != true)
                    {
                        activeStep = new ExecutionOrderStep(activeAgents.Values);
                    }

                    activeStep?.Collect(activeAgents[log.Agent]);
                }
                else if(activeAgents.ContainsKey(log.Agent))
                {
                    AgentCollector remainingCollector = activeAgents[log.Agent].Finish(log.Message);
                    if (remainingCollector == null)
                    {
                        activeAgents.Remove(log.Agent);
                    }

                    if (activeStep != null)
                    {
                        steps.Add(activeStep);
                        activeStep = null;
                    }
                }
            }
        }

        public void CheckParallelExecution(string[] agents)
        {
            steps.Any(s => agents.All(a => s.Agents.Any(stepAgent => stepAgent.EndsWith(a))))
                .Should().BeTrue("agents should have been executed parallel. Following steps were recorded: " +
                                 $"{string.Join(", ", steps.Select(s => $"[{string.Join(", ", s.Agents)}]"))}");
        }

        public void CheckParallelMessages(string agent, int messageCount)
        {
            steps.Any(s => s.Agents.FirstOrDefault(a => a.EndsWith(agent)) != null &&
                           s.AgentMessages[s.Agents.First(a => a.EndsWith(agent))]
                               .GroupBy(m => m.Name)
                               .Any(g => g.Count() == messageCount))
                .Should().BeTrue($"agent {agent} should have executed {messageCount} messages parallel. The following steps were recorded: " +
                                 string.Join(", ", steps.Select(s => $"[{string.Join(", ", s.Agents.Select(a => $"{a}: {s.AgentMessages[a].Count()}"))}]")));
        }

        private class AgentCollector
        {
            private readonly Dictionary<Guid, MessageLog> incomingMessages = new Dictionary<Guid, MessageLog>();

            public AgentCollector(string agent)
            {
                this.Agent = agent;
            }

            public string Agent { get; }

            public IEnumerable<MessageLog> Messages => incomingMessages.Values;

            private AgentCollector(string agent, IEnumerable<MessageLog> remaining) : this(agent)
            {
                incomingMessages = remaining.ToDictionary(l => l.Id, l => l);
            }

            public void Collect(MessageLog message)
            {
                incomingMessages.Add(message.Id, message);
            }

            public AgentCollector Finish(MessageLog publishedMessage)
            {
                List<MessageLog> remaining = incomingMessages
                                                .Where(incomingMessage => !publishedMessage.Predecessors.Contains(incomingMessage.Key))
                                                .Select(incomingMessage => incomingMessage.Value).ToList();

                return remaining.Any() ? new AgentCollector(Agent, remaining) : null;
            }
        }

        private class ExecutionOrderStep
        {
            private readonly HashSet<AgentCollector> activeAgents;

            public ExecutionOrderStep(IEnumerable<AgentCollector> activeAgents)
            {
                this.activeAgents = new HashSet<AgentCollector>(activeAgents);
            }

            public IEnumerable<string> Agents => activeAgents.Select(a => a.Agent);

            public IDictionary<string, IEnumerable<MessageLog>> AgentMessages =>
                activeAgents.ToDictionary(a => a.Agent, a => a.Messages);

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
