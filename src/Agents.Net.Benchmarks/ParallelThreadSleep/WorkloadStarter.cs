using System;
using System.Collections.Generic;
using Agents.Net;

namespace Agents.Net.Benchmarks.ParallelThreadSleep
{
    public class WorkloadStarter : Agent
    {
        #region Definition

        [AgentDefinition]
        public static AgentDefinition WorkloadStarterDefinition { get; }
            = new AgentDefinition(new []
                                  {
                                      StartingWorkloadsMessage.StartingWorkloadsMessageDefinition
                                  },
                                  new []
                                  {
                                      WorkloadDefinedMessage.WorkloadDefinedMessageDefinition
                                  });

        #endregion

        public WorkloadStarter(IMessageBoard messageBoard) : base(WorkloadStarterDefinition, messageBoard)
        {
        }

        protected override void ExecuteCore(Message messageData)
        {
            StartingWorkloadsMessage workloads = messageData.Get<StartingWorkloadsMessage>();
            List<Message> messages = new List<Message>();
            foreach (int workload in workloads.Workloads)
            {
                messages.Add(new WorkloadDefinedMessage(workload, messageData));
            }
            MessageDomain.CreateNewDomainsFor(messages);
            foreach (Message message in messages)
            {
                OnMessage(message);
            }
        }
    }
}
