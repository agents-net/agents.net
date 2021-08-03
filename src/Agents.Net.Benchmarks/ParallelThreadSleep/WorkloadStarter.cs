#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;
using System.Collections.Generic;
using Agents.Net;

namespace Agents.Net.Benchmarks.ParallelThreadSleep
{
    [Consumes(typeof(WorkloadExecutedMessage))]
    [Consumes(typeof(ExceptionMessage))]
    [Consumes(typeof(StartingWorkloadsMessage))]
    [Produces(typeof(WorkloadDefinedMessage))]
    public class WorkloadStarter : Agent
    {
        private readonly MessageGate<WorkloadDefinedMessage, WorkloadExecutedMessage> gate = new();
        private readonly Action terminateAction;
        public WorkloadStarter(IMessageBoard messageBoard, Action terminateAction) : base(messageBoard)
        {
            this.terminateAction = terminateAction;
        }

        protected override void ExecuteCore(Message messageData)
        {
            if (gate.Check(messageData))
            {
                return;
            }
            StartingWorkloadsMessage workloads = messageData.Get<StartingWorkloadsMessage>();
            List<WorkloadDefinedMessage> messages = new();
            foreach (int workload in workloads.Workloads)
            {
                messages.Add(new WorkloadDefinedMessage(workload, messageData));
            }
            gate.SendAndContinue(messages,OnMessage, result =>
            {
                terminateAction();
            });
        }
    }
}
