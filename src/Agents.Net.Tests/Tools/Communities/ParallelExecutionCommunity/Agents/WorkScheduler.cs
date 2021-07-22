#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;
using System.Collections.Generic;
using Agents.Net;
using Agents.Net.Tests.Tools.Communities.ParallelExecutionCommunity.Messages;

namespace Agents.Net.Tests.Tools.Communities.ParallelExecutionCommunity.Agents
{
    [Consumes(typeof(InitializeMessage))]
    [Consumes(typeof(WorkDone))]
    [Consumes(typeof(ExceptionMessage))]
    [Produces(typeof(WorkScheduled))]
    [Produces(typeof(MessagesAggregated<WorkDone>))]
    public class WorkScheduler : Agent
    {
        private readonly MessageGate<WorkScheduled, WorkDone> aggregator = new();
        public WorkScheduler(IMessageBoard messageBoard) : base(messageBoard)
        {
        }

        protected override void ExecuteCore(Message messageData)
        {
            if (aggregator.Check(messageData))
            {
                return;
            }
            List<WorkScheduled> messages = new();
            for (int i = 0; i < 4; i++)
            {
                messages.Add(new WorkScheduled(i, messageData));
            }
            aggregator.SendAndAggregate(messages, OnMessage);
        }
    }
}
