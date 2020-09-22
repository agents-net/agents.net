using System;
using System.Threading;
using Agents.Net;
using Agents.Net.Tests.Tools.Communities.DefaultDisposeCommunity.Messages;
using FluentAssertions;

namespace Agents.Net.Tests.Tools.Communities.DefaultDisposeCommunity.Agents
{
    [Consumes(typeof(MultiConsumedMessage))]
    public class ConsumingAgent2 : Agent
    {
        public ConsumingAgent2(IMessageBoard messageBoard) : base(messageBoard)
        {
        }

        protected override void ExecuteCore(Message messageData)
        {
            if (messageData is DisposableMessage disposableMessage)
            {
                //This timeout ensures that the issue https://github.com/agents-net/agents.net/issues/68 is checked
                Thread.Sleep(10);
                disposableMessage.IsDisposed.Should().BeFalse("messages should not be disposed before used.");
            }
        }
    }
}
