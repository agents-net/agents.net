using System;
using System.Collections.Generic;
using System.Linq;
using Agents.Net;

namespace Agents.Net.Benchmarks.ParallelThreadSleep
{
    [Consumes(typeof(WorkloadExecutedMessage))]
    public class WorkloadAggregator : Agent
    {
        private readonly MessageAggregator<WorkloadExecutedMessage> aggregator;
        private readonly Action terminateAction;

        public WorkloadAggregator(IMessageBoard messageBoard, Action terminateAction) : base(messageBoard)
        {
            this.terminateAction = terminateAction;
            aggregator = new MessageAggregator<WorkloadExecutedMessage>(OnAggregated);
        }

        private void OnAggregated(IReadOnlyCollection<WorkloadExecutedMessage> aggregate)
        {
            MessageDomain.TerminateDomainsOf(aggregate);
            terminateAction();
        }

        protected override void ExecuteCore(Message messageData)
        {
            aggregator.Aggregate(messageData);
        }
    }
}
