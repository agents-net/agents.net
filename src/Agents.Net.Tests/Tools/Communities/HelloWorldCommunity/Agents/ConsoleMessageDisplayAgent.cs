using System;
using Agents.Net;
using Agents.Net.Tests.Tools.Communities.HelloWorldCommunity.Messages;

namespace Agents.Net.Tests.Tools.Communities.HelloWorldCommunity.Agents
{
    public class ConsoleMessageDisplayAgent : Agent
    {
        #region Definition

        [AgentDefinition]
        public static AgentDefinition ConsoleMessageDisplayAgentDefinition { get; }
            = new AgentDefinition(new []
                                  {
                                      ConsoleMessageCreated.ConsoleMessageCreatedDefinition
                                  },
                                  Array.Empty<MessageDefinition>());

        #endregion

        public ConsoleMessageDisplayAgent(IMessageBoard messageBoard) : base(ConsoleMessageDisplayAgentDefinition, messageBoard)
        {
        }

        protected override void ExecuteCore(Message messageData)
        {
            throw new NotImplementedException();
        }
    }
}
