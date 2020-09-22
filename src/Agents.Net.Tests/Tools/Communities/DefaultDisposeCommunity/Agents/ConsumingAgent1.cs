using System;
using System.Threading;
using Agents.Net;
using Agents.Net.Tests.Tools.Communities.DefaultDisposeCommunity.Messages;
using FluentAssertions;

namespace Agents.Net.Tests.Tools.Communities.DefaultDisposeCommunity.Agents
{
    [Consumes(typeof(SingleConsumedMessage))]
    [Consumes(typeof(MultiConsumedMessage))]
    [Consumes(typeof(DecoratingMessage))]
    [Produces(typeof(AllMessagesConsumed))]
    public class ConsumingAgent1 : Agent
    {
        public ConsumingAgent1(IMessageBoard messageBoard) : base(messageBoard)
        {
        }

        private int counter;

        protected override void ExecuteCore(Message messageData)
        {
            if (messageData is DisposableMessage disposableMessage)
            {
                //This timeout ensures that the issue https://github.com/agents-net/agents.net/issues/68 is checked
                Thread.Sleep(10);
                disposableMessage.IsDisposed.Should().BeFalse("messages should not be disposed before used.");
            }
            if (Interlocked.Increment(ref counter) == 3)
            {
                OnMessage(new AllMessagesConsumed(messageData));
            }
        }
    }
}
