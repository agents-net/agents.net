#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;
using Agents.Net;
using Agents.Net.Tests.Tools.Communities.ParallelExecutionCommunity.Messages;

namespace Agents.Net.Tests.Tools.Communities.ParallelExecutionCommunity.Agents
{
    [Consumes(typeof(WorkloadFinished))]
    public class ResultReporter : Agent
    {
        private readonly IConsole console;
        private readonly Action terminateAction;
        
        public ResultReporter(IMessageBoard messageBoard, IConsole console, Action terminateAction) : base(messageBoard)
        {
            this.console = console;
            this.terminateAction = terminateAction;
        }

        protected override void ExecuteCore(Message messageData)
        {
            WorkloadFinished workloadFinished = messageData.Get<WorkloadFinished>();
            console.WriteLine(workloadFinished.TotalResult.ToString());
            terminateAction();
        }
    }
}
