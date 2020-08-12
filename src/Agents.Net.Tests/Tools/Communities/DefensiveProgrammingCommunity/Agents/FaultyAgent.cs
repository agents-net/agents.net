using System;
using Agents.Net;
using Agents.Net.Tests.Tools.Communities.DefensiveProgrammingCommunity.Messages;

namespace Agents.Net.Tests.Tools.Communities.DefensiveProgrammingCommunity.Agents
{
    [Consumes(typeof(InformationGathered))]
    public class FaultyAgent : Agent
    {
        public FaultyAgent(IMessageBoard messageBoard) : base(messageBoard)
        {
        }

        protected override void ExecuteCore(Message messageData)
        {
            throw new RecoverableException();
        }
    }
}
