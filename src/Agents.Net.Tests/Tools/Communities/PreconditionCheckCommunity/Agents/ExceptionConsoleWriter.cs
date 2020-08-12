using System;
using Agents.Net;


namespace Agents.Net.Tests.Tools.Communities.PreconditionCheckCommunity.Agents
{
    [Consumes(typeof(ExceptionMessage))]
    public class ExceptionConsoleWriter : Agent
    {
        private readonly IConsole console;
        private readonly Action terminateAction;

        public ExceptionConsoleWriter(IMessageBoard messageBoard, IConsole console, Action terminateAction) : base(messageBoard)
        {
            this.console = console;
            this.terminateAction = terminateAction;
        }

        protected override void ExecuteCore(Message messageData)
        {
            ExceptionMessage exceptionMessage = messageData.Get<ExceptionMessage>();
            console.WriteLine(exceptionMessage.CustomMessage);
            terminateAction();
        }
    }
}
