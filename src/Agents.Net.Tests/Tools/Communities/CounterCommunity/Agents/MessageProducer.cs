using System;
using Agents.Net;
using Agents.Net.Tests.Tools.Communities.CounterCommunity.Messages;

namespace Agents.Net.Tests.Tools.Communities.CounterCommunity.Agents
{
    [Consumes(typeof(InitializeMessage))]
    [Produces(typeof(Message1))]
    [Produces(typeof(Message2))]
    [Produces(typeof(Message3))]
    [Produces(typeof(Message4))]
    [Produces(typeof(Message5))]
    public class MessageProducer : Agent
    {
        public MessageProducer(IMessageBoard messageBoard) : base(messageBoard)
        {
        }

        protected override void ExecuteCore(Message messageData)
        {
            OnMessage(new Message1(messageData));
            OnMessage(new Message2(messageData));
            OnMessage(new Message3(messageData));
            OnMessage(new Message4(messageData));
            OnMessage(new Message5(messageData));
        }
    }
}
