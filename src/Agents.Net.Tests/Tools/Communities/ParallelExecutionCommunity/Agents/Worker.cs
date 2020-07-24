using System;
using Agents.Net;
using Agents.Net.Tests.Tools.Communities.ParallelExecutionCommunity.Messages;

namespace Agents.Net.Tests.Tools.Communities.ParallelExecutionCommunity.Agents
{
    [Consumes(typeof(InformationGathered))]
    [Consumes(typeof(WorkScheduled))]
    [Produces(typeof(WorkDone))]
    public class Worker : Agent
    {
        private readonly MessageCollector<InformationGathered, WorkScheduled> collector;
        
        public Worker(IMessageBoard messageBoard) : base(messageBoard)
        {
            collector = new MessageCollector<InformationGathered, WorkScheduled>(OnMessagesCollected);
        }

        private void OnMessagesCollected(MessageCollection<InformationGathered, WorkScheduled> set)
        {
            if (set.Message1.ProceedWithCaution)
            {
                OnMessage(new WorkDone(set.Message2.Work+1, set));
            }
        }

        protected override void ExecuteCore(Message messageData)
        {
            collector.Push(messageData);
        }
    }
}
