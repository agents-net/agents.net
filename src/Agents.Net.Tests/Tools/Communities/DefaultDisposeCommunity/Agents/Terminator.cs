using System;
using Agents.Net;
using Agents.Net.Tests.Tools.Communities.DefaultDisposeCommunity.Messages;

namespace Agents.Net.Tests.Tools.Communities.DefaultDisposeCommunity.Agents
{
    [Consumes(typeof(AllMessagesConsumed))]
    public class Terminator : Agent
    {
        private readonly Action terminateAction;
        
        public Terminator(IMessageBoard messageBoard, Action terminateAction) : base(messageBoard)
        {
            this.terminateAction = terminateAction;
        }

        protected override void ExecuteCore(Message messageData)
        {
            terminateAction();
        }
    }
}
