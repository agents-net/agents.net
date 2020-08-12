using System;
using Agents.Net;
using Agents.Net.Tests.Tools.Communities.DefensiveProgrammingCommunity.Messages;

namespace Agents.Net.Tests.Tools.Communities.DefensiveProgrammingCommunity.Agents
{
    [Consumes(typeof(ExceptionMessage))]
    [Consumes(typeof(HandledException), Implicitly = true)]
    public class ExceptionTerminator : Agent
    {
        private readonly Action terminateAction;

        public ExceptionTerminator(IMessageBoard messageBoard, Action terminateAction) : base(messageBoard)
        {
            this.terminateAction = terminateAction;
        }

        protected override void ExecuteCore(Message messageData)
        {
            if (!messageData.Is<HandledException>())
            {
                terminateAction();
            }
        }
    }
}
