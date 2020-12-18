#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;
using Agents.Net;
using Agents.Net.Tests.Tools.Communities.DefensiveProgrammingCommunity.Messages;

namespace Agents.Net.Tests.Tools.Communities.DefensiveProgrammingCommunity.Agents
{
    [Consumes(typeof(InitializeMessage))]
    [Produces(typeof(InformationGathered))]
    public class InformationBroker : Agent
    {
        private readonly CommandLineArgs commandLineArgs;

        public InformationBroker(IMessageBoard messageBoard, CommandLineArgs commandLineArgs) : base(messageBoard)
        {
            this.commandLineArgs = commandLineArgs;
        }

        protected override void ExecuteCore(Message messageData)
        {
            OnMessage(new InformationGathered(commandLineArgs.Arguments[0], messageData));
        }
    }
}
