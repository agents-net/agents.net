using System;
using Agents.Net;
using Agents.Net.Tests.Tools.Communities.ParallelExecutionCommunity.Messages;

namespace Agents.Net.Tests.Tools.Communities.ParallelExecutionCommunity.Agents
{
    [Consumes(typeof(InitializeMessage))]
    [Produces(typeof(InformationGathered))]
    public class InformationBroker : Agent
    {
        public InformationBroker(IMessageBoard messageBoard) : base(messageBoard)
        {
        }

        protected override void ExecuteCore(Message messageData)
        {
            OnMessage(new InformationGathered(true, messageData));
        }
    }
}
