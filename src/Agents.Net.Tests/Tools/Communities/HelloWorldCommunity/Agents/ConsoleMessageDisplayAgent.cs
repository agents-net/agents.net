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

        private readonly IConsole console;
        private readonly Action terminateAction;

        public ConsoleMessageDisplayAgent(IMessageBoard messageBoard, IConsole console, Action terminateAction) : base(ConsoleMessageDisplayAgentDefinition, messageBoard)
        {
            this.console = console;
            this.terminateAction = terminateAction;
        }

        protected override void ExecuteCore(Message messageData)
        {
            console.WriteLine(messageData.Get<ConsoleMessageCreated>().Message);
            terminateAction();
        }
    }
}
