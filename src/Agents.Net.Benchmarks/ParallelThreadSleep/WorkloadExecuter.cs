#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;
using System.Threading;
using Agents.Net;

namespace Agents.Net.Benchmarks.ParallelThreadSleep
{
    [Consumes(typeof(WorkloadDefinedMessage))]
    [Produces(typeof(WorkloadExecutedMessage))]
    public class WorkloadExecuter : Agent
    {
        public WorkloadExecuter(IMessageBoard messageBoard) : base(messageBoard)
        {
        }

        protected override void ExecuteCore(Message messageData)
        {
            Thread.Sleep(messageData.Get<WorkloadDefinedMessage>().Workload);
            OnMessage(new WorkloadExecutedMessage(messageData));
        }
    }
}
