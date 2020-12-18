#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;
using System.Collections.Generic;
using Agents.Net;

namespace Agents.Net.Benchmarks.ParallelThreadSleep
{
    [Consumes(typeof(StartingWorkloadsMessage))]
    [Produces(typeof(WorkloadDefinedMessage))]
    public class WorkloadStarter : Agent
    {
        public WorkloadStarter(IMessageBoard messageBoard) : base(messageBoard)
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
            OnMessages(messages);
        }
    }
}
