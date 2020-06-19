using System;
using System.Collections.Generic;
using System.Linq;
using Agents.Net;

namespace Agents.Net.Benchmarks.ParallelThreadSleep
{
    public class WorkloadAggregator : Agent
    {
        #region Definition

        [AgentDefinition]
        public static AgentDefinition WorkloadAggregatorDefinition { get; }
            = new AgentDefinition(new []
                                  {
                                      WorkloadExecutedMessage.WorkloadExecutedMessageDefinition
                                  },
                                  Array.Empty<MessageDefinition>());

        #endregion

        private readonly MessageAggregator<WorkloadExecutedMessage> aggregator;
        private readonly Action terminateAction;

        public WorkloadAggregator(IMessageBoard messageBoard, Action terminateAction) : base(WorkloadAggregatorDefinition, messageBoard)
        {
            this.terminateAction = terminateAction;
            aggregator = new MessageAggregator<WorkloadExecutedMessage>(OnAggregated);
        }

        private void OnAggregated(ICollection<WorkloadExecutedMessage> aggregate)
        {
            WorkloadExecutedMessage[] messageCollection = aggregate.ToArray();
            OnMessage(new MessageDomainTerminatedMessage(messageCollection));
            terminateAction();
        }

        protected override void ExecuteCore(Message messageData)
        {
            aggregator.Aggregate(messageData);
        }
    }
}
