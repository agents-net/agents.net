#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;
using Agents.Net;
using Agents.Net.Tests.Tools.Communities.TransactionManagerCommunity.Messages;

namespace Agents.Net.Tests.Tools.Communities.TransactionManagerCommunity.Agents
{
    [Consumes(typeof(TransactionSuccessful))]
    [Consumes(typeof(TransactionRollback))]
    public class Terminator : Agent
    {
        private readonly IConsole console;
        private readonly Action terminateAction;
        public Terminator(IMessageBoard messageBoard, IConsole console, Action terminateAction) : base(messageBoard)
        {
            this.console = console;
            this.terminateAction = terminateAction;
        }

        protected override void ExecuteCore(Message messageData)
        {
            console.WriteLine(messageData.Is<TransactionSuccessful>() ? "Transaction Successful" : "Rollback");
            terminateAction();
        }
    }
}
