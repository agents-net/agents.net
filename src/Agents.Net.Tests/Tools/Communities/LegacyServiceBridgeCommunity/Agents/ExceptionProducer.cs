#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;
using Agents.Net;
using Agents.Net.Tests.Tools.Communities.LegacyServiceBridgeCommunity.Messages;

namespace Agents.Net.Tests.Tools.Communities.LegacyServiceBridgeCommunity.Agents
{
    [Produces(typeof(ExceptionMessage))]
    [Intercepts(typeof(ServiceParameterPassed))]
    public class ExceptionProducer : InterceptorAgent
    {
        public ExceptionProducer(IMessageBoard messageBoard) : base(messageBoard)
        {
        }

        protected override InterceptionAction InterceptCore(Message messageData)
        {
            ServiceParameterPassed parameterPassed = messageData.Get<ServiceParameterPassed>();
            if (parameterPassed.ThrowException)
            {
                OnMessage(new ExceptionMessage("Error", messageData, this));
                return InterceptionAction.DoNotPublish;
            }

            return InterceptionAction.Continue;
        }
    }
}
