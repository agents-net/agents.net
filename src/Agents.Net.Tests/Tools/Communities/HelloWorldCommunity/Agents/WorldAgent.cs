using System;
using Agents.Net;
using Agents.Net.Tests.Tools.Communities.HelloWorldCommunity.Messages;

namespace Agents.Net.Tests.Tools.Communities.HelloWorldCommunity.Agents
{
    [Consumes(typeof(InitializeMessage))]
    [Produces(typeof(WorldConsoleMessage))]
    public class WorldAgent : Agent
    {        public WorldAgent(IMessageBoard messageBoard) : base(messageBoard)
        {
        }

        protected override void ExecuteCore(Message messageData)
        {
            OnMessage(new WorldConsoleMessage("World", messageData));
        }
    }
}
