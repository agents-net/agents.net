using System;
using Agents.Net;
using Agents.Net.Tests.Tools.Communities.LegacyServiceBridgeCommunity.Messages;

namespace Agents.Net.Tests.Tools.Communities.LegacyServiceBridgeCommunity.Agents
{
    [Consumes(typeof(ServiceParameterPassed))]
    [Produces(typeof(ServiceResult))]
    public class ServiceExecutor : Agent
    {
        public ServiceExecutor(IMessageBoard messageBoard) : base(messageBoard)
        {
        }

        protected override void ExecuteCore(Message messageData)
        {
            OnMessage(new ServiceResult(messageData, "ServiceCallResult"));
        }
    }
}
