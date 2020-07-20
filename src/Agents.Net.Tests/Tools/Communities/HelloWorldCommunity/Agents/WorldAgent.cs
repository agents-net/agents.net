using System;
using Agents.Net;
using Agents.Net.Tests.Tools.Communities.HelloWorldCommunity.Messages;

namespace Agents.Net.Tests.Tools.Communities.HelloWorldCommunity.Agents
{
    public class WorldAgent : Agent
    {
        #region Definition

        [AgentDefinition]
        public static AgentDefinition WorldAgentDefinition { get; }
            = new AgentDefinition(new []
                                  {
                                      InitializeMessage.InitializeMessageDefinition
                                  },
                                  new []
                                  {
                                      WorldConsoleMessage.WorldConsoleMessageDefinition
                                  });

        #endregion

        public WorldAgent(IMessageBoard messageBoard) : base(WorldAgentDefinition, messageBoard)
        {
        }

        protected override void ExecuteCore(Message messageData)
        {
            OnMessage(new WorldConsoleMessage("World", messageData));
        }
    }
}
