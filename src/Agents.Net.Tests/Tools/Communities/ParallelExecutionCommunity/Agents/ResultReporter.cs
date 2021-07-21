#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;
using System.Linq;
using Agents.Net;
using Agents.Net.Tests.Tools.Communities.ParallelExecutionCommunity.Messages;

namespace Agents.Net.Tests.Tools.Communities.ParallelExecutionCommunity.Agents
{
    [Consumes(typeof(MessagesAggregated<WorkDone>))]
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
            MessagesAggregated<WorkDone> workloadFinished = messageData.Get<MessagesAggregated<WorkDone>>();
            console.WriteLine(workloadFinished.Result.EndMessages.Sum(m => m.WorkResult).ToString());
            terminateAction();
        }
    }
}
