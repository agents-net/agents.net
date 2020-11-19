#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;
using Agents.Net;
using Agents.Net.Tests.Tools.Communities.HelloWorldCommunity.Messages;

namespace Agents.Net.Tests.Tools.Communities.HelloWorldCommunity.Agents
{
    [Consumes(typeof(WorldConsoleMessage))]
    [Consumes(typeof(HelloConsoleMessage))]
    [Produces(typeof(ConsoleMessageCreated))]
    public class ConsoleMessageJoiner : Agent
    {
        private readonly MessageCollector<HelloConsoleMessage, WorldConsoleMessage> collector;

        public ConsoleMessageJoiner(IMessageBoard messageBoard) : base(messageBoard)
        {
            collector = new MessageCollector<HelloConsoleMessage, WorldConsoleMessage>(OnMessagesCollected);
        }

        private void OnMessagesCollected(MessageCollection<HelloConsoleMessage, WorldConsoleMessage> set)
        {
            OnMessage(new ConsoleMessageCreated($"{set.Message1.Message} {set.Message2.Message}", set));
        }

        protected override void ExecuteCore(Message messageData)
        {
            collector.Push(messageData);
        }
    }
}
