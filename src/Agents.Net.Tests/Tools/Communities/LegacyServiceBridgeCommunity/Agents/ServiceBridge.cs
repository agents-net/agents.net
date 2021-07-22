#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;
using Agents.Net;
using Agents.Net.Tests.Tools.Communities.LegacyServiceBridgeCommunity.Messages;

namespace Agents.Net.Tests.Tools.Communities.LegacyServiceBridgeCommunity.Agents
{
    [Consumes(typeof(ExceptionMessage))]
    [Consumes(typeof(ServiceResult))]
    [Produces(typeof(ServiceParameterPassed))]
    public class ServiceBridge : Agent, ILegacyService
    {
        private readonly MessageGate<ServiceParameterPassed, ServiceResult> gate = new MessageGate<ServiceParameterPassed, ServiceResult>();
        public ServiceBridge(IMessageBoard messageBoard) : base(messageBoard)
        {
        }

        protected override void ExecuteCore(Message messageData)
        {
            gate.Check(messageData);
        }

        public string ServiceCall(bool throwException)
        {
            ServiceParameterPassed startMessage = new ServiceParameterPassed(throwException);
            MessageGateResult<ServiceResult> result = gate.SendAndAwait(startMessage, OnMessage);
            return result.Result == MessageGateResultKind.Success ? result.EndMessage.Result : "Exception";
        }
    }
}
