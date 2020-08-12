using System;
using Agents.Net;
using Agents.Net.Tests.Tools.Communities.DelayCommunity.Messages;

namespace Agents.Net.Tests.Tools.Communities.DelayCommunity.Agents
{
    [Consumes(typeof(InformationGathered))]
    public class InformationConsoleWriter : Agent
    {
        private readonly IConsole console;
        private readonly Action terminateAction;

        public InformationConsoleWriter(IMessageBoard messageBoard, IConsole console, Action terminateAction) : base(messageBoard)
        {
            this.console = console;
            this.terminateAction = terminateAction;
        }

        protected override void ExecuteCore(Message messageData)
        {
            InformationGathered gathered = messageData.Get<InformationGathered>();
            console.WriteLine(gathered.Information);
            terminateAction();
        }
    }
}
