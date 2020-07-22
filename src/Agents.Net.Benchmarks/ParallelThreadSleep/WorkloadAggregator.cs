using System;
using System.Collections.Generic;
using System.Linq;
using Agents.Net;

namespace Agents.Net.Benchmarks.ParallelThreadSleep
{
    [Consumes(typeof(WorkloadExecutedMessage))]
    public class WorkloadAggregator : Agent
    {        private readonly MessageAggregator<WorkloadExecutedMessage> aggregator;
        private readonly Action terminateAction;

        public WorkloadAggregator(IMessageBoard messageBoard, Action terminateAction) : base(messageBoard)
        {
            this.terminateAction = terminateAction;
            aggregator = new MessageAggregator<WorkloadExecutedMessage>(OnAggregated);
        }

        private void OnAggregated(ICollection<WorkloadExecutedMessage> aggregate)
        {
            WorkloadExecutedMessage[] messageCollection = aggregate.ToArray();
            MessageDomain.TerminateDomainsOf(messageCollection);
            terminateAction();
        }

        protected override void ExecuteCore(Message messageData)
        {
            aggregator.Aggregate(messageData);
        }
    }
}
