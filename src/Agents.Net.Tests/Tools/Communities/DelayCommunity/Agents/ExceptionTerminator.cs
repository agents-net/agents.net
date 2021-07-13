#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;

namespace Agents.Net.Tests.Tools.Communities.DelayCommunity.Agents
{
    [Consumes(typeof(ExceptionMessage))]
    public class ExceptionTerminator : Agent
    {
        private readonly IConsole console;
        private readonly Action terminateAction;
        public ExceptionTerminator(IMessageBoard messageBoard, IConsole console, Action terminateAction)
            : base(messageBoard)
        {
            this.console = console;
            this.terminateAction = terminateAction;
        }

        protected override void ExecuteCore(Message messageData)
        {
            console.WriteLine("Interception conflict detected.");
            terminateAction();
        }
    }
}