#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;
using System.Linq;
using Agents.Net;
using Agents.Net.Tests.Tools.Communities.DelayCommunity.Messages;

namespace Agents.Net.Tests.Tools.Communities.DelayCommunity.Agents
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
            if (args.Arguments.FirstOrDefault() == "simulate interception conflict")
            {
                OnMessage(new InformationGathered("Conflict", messageData));
            }
            else if (args.Arguments.FirstOrDefault() == "DoNotPublish intention")
            {
                OnMessage(new InformationGathered("DoNotPublish", messageData));
            }
            else
            {
                OnMessage(new InformationGathered("Special Information", messageData));
            }
        }
    }
}
