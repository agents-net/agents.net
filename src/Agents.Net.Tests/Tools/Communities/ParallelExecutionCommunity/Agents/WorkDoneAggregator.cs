#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Agents.Net;
using Agents.Net.Tests.Tools.Communities.ParallelExecutionCommunity.Messages;

namespace Agents.Net.Tests.Tools.Communities.ParallelExecutionCommunity.Agents
{
    [Consumes(typeof(WorkDone))]
    [Produces(typeof(WorkloadFinished))]
    public class WorkDoneAggregator : Agent
    {
        private readonly MessageAggregator<WorkDone> aggregator;
        
        public WorkDoneAggregator(IMessageBoard messageBoard) : base(messageBoard)
        {
            aggregator = new MessageAggregator<WorkDone>(OnAggregated);
        }

        private void OnAggregated(IReadOnlyCollection<WorkDone> set)
        {
            MessageDomain.TerminateDomainsOf(set);
            OnMessage(new WorkloadFinished(set.Sum(m => m.WorkResult), set));
        }

        protected override void ExecuteCore(Message messageData)
        {
            aggregator.Aggregate(messageData);
        }
    }
}
