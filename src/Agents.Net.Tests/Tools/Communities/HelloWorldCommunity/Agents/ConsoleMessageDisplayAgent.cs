using System;
using Agents.Net;
using Agents.Net.Tests.Tools.Communities.HelloWorldCommunity.Messages;

namespace Agents.Net.Tests.Tools.Communities.HelloWorldCommunity.Agents
{
    [Consumes(typeof(ConsoleMessageCreated))]
    public class ConsoleMessageDisplayAgent : Agent
    {        private readonly IConsole console;
        private readonly Action terminateAction;

        public ConsoleMessageDisplayAgent(IMessageBoard messageBoard, IConsole console, Action terminateAction) : base(messageBoard)
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
