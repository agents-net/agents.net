#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;
using Agents.Net;
using Agents.Net.Tests.Tools.Communities.TransactionManagerCommunity.Messages;

namespace Agents.Net.Tests.Tools.Communities.TransactionManagerCommunity.Agents
{
    [Consumes(typeof(TransactionStarted))]
    [Produces(typeof(TransactionFinished))]
    public class TransactionExecuter : Agent
    {
        public TransactionExecuter(IMessageBoard messageBoard) : base(messageBoard)
        {
        }

        protected override void ExecuteCore(Message messageData)
        {
            OnMessage(new TransactionFinished(messageData));
        }
    }
}
