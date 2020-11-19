#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;
using Agents.Net;
using Agents.Net.Tests.Tools.Communities.DefaultDisposeCommunity.Messages;

namespace Agents.Net.Tests.Tools.Communities.DefaultDisposeCommunity.Agents
{
    [Consumes(typeof(InitializeMessage))]
    [Produces(typeof(SingleConsumedMessage))]
    [Produces(typeof(MultiConsumedMessage))]
    [Produces(typeof(UnconsumedMessage))]
    [Produces(typeof(InterceptedMessage))]
    [Produces(typeof(DecoratedMessage))]
    public class MessageGenerator : Agent
    {
        public MessageGenerator(IMessageBoard messageBoard) : base(messageBoard)
        {
        }

        protected override void ExecuteCore(Message messageData)
        {
            OnMessage(new SingleConsumedMessage(messageData));
            OnMessage(new MultiConsumedMessage(messageData));
            OnMessage(new UnconsumedMessage(messageData));
            OnMessage(new InterceptedMessage(messageData));
            OnMessage(new DecoratedMessage(messageData));
        }
    }
}
