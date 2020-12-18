#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;
using Agents.Net;
using Agents.Net.Tests.Tools.Communities.DelayCommunity.Messages;

namespace Agents.Net.Tests.Tools.Communities.DelayCommunity.Agents
{
    [Consumes(typeof(TransformingInformation))]
    [Produces(typeof(TransformationCompleted))]
    public class InformationTransformer : Agent
    {
        private readonly IConsole console;

        public InformationTransformer(IMessageBoard messageBoard, IConsole console) : base(messageBoard)
        {
            this.console = console;
        }

        protected override void ExecuteCore(Message messageData)
        {
            TransformingInformation transformingInformation = messageData.Get<TransformingInformation>();
            console.WriteLine(transformingInformation.Information.Replace("Special", "Transformed",
                                                                          StringComparison.OrdinalIgnoreCase));
            OnMessage(new TransformationCompleted(transformingInformation.OriginalMessage, messageData));
        }
    }
}
