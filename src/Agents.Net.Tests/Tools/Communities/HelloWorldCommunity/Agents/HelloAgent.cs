using System;
using Agents.Net;
using Agents.Net.Tests.Tools.Communities.HelloWorldCommunity.Messages;

namespace Agents.Net.Tests.Tools.Communities.HelloWorldCommunity.Agents
{
    [Consumes(typeof(InitializeMessage))]
    [Produces(typeof(HelloConsoleMessage))]
    public class HelloAgent : Agent
    {        public HelloAgent(IMessageBoard messageBoard) : base(messageBoard)
        {
        }

        protected override void ExecuteCore(Message messageData)
        {
            OnMessage(new HelloConsoleMessage("Hello", messageData));
        }
    }
}
