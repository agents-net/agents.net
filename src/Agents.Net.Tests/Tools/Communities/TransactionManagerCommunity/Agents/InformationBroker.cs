#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;
using Agents.Net;
using Agents.Net.Tests.Tools.Communities.TransactionManagerCommunity.Messages;

namespace Agents.Net.Tests.Tools.Communities.TransactionManagerCommunity.Agents
{
    [Consumes(typeof(InitializeMessage))]
    [Produces(typeof(InformationGathered))]
    public class InformationBroker : Agent
    {
        private readonly CommandLineArgs args;
        public InformationBroker(IMessageBoard messageBoard, CommandLineArgs args) : base(messageBoard)
        {
            this.args = args;
        }

        protected override void ExecuteCore(Message messageData)
        {
            OnMessage(new InformationGathered(messageData, args.Arguments[0]));
        }
    }
}
