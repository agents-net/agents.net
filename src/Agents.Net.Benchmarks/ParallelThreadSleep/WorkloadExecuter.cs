using System;
using System.Threading;
using Agents.Net;

namespace Agents.Net.Benchmarks.ParallelThreadSleep
{
    public class WorkloadExecuter : Agent
    {
        #region Definition

        [AgentDefinition]
        public static AgentDefinition WorkloadExecuterDefinition { get; }
            = new AgentDefinition(new []
                                  {
                                      WorkloadDefinedMessage.WorkloadDefinedMessageDefinition
                                  },
                                  new []
                                  {
                                      WorkloadExecutedMessage.WorkloadExecutedMessageDefinition
                                  });

        #endregion

        public WorkloadExecuter(IMessageBoard messageBoard) : base(WorkloadExecuterDefinition, messageBoard)
        {
        }

        protected override void ExecuteCore(Message messageData)
        {
            Thread.Sleep(messageData.Get<WorkloadDefinedMessage>().Workload);
            OnMessage(new WorkloadExecutedMessage(messageData));
        }
    }
}
