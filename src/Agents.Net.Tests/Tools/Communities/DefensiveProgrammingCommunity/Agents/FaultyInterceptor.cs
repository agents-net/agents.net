using System;
using Agents.Net;
using Agents.Net.Tests.Tools.Communities.DefensiveProgrammingCommunity.Messages;

namespace Agents.Net.Tests.Tools.Communities.DefensiveProgrammingCommunity.Agents
{
    [Intercepts(typeof(InformationGathered))]
    public class FaultyInterceptor : InterceptorAgent
    {
        public FaultyInterceptor(IMessageBoard messageBoard) : base(messageBoard)
        {
        }

        protected override InterceptionAction InterceptCore(Message messageData)
        {
            if (messageData.Get<InformationGathered>().Information == "Terminate")
            {
                throw new InvalidOperationException();
            }

            return InterceptionAction.Continue;
        }
    }
}
