using System;
using Agents.Net;
using Agents.Net.Tests.Tools.Communities.HelloWorldCommunity.Messages;

namespace Agents.Net.Tests.Tools.Communities.HelloWorldCommunity.Agents
{
    public class ConsoleMessageJoiner : Agent
    {
        #region Definition

        [AgentDefinition]
        public static AgentDefinition ConsoleMessageJoinerDefinition { get; }
            = new AgentDefinition(new []
                                  {
                                      WorldConsoleMessage.WorldConsoleMessageDefinition,
                                      HelloConsoleMessage.HelloConsoleMessageDefinition
                                  },
                                  new []
                                  {
                                      ConsoleMessageCreated.ConsoleMessageCreatedDefinition
                                  });

        #endregion

        public ConsoleMessageJoiner(IMessageBoard messageBoard) : base(ConsoleMessageJoinerDefinition, messageBoard)
        {
        }

        protected override void ExecuteCore(Message messageData)
        {
            throw new NotImplementedException();
        }
    }
}
