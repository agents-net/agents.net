#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;
using Agents.Net;
using Agents.Net.Tests.Tools.Communities.PreconditionCheckCommunity.Messages;

namespace Agents.Net.Tests.Tools.Communities.PreconditionCheckCommunity.Agents
{
    [Produces(typeof(ExceptionMessage))]
    [Intercepts(typeof(InformationGathered))]
    public class InformationValidator : InterceptorAgent
    {
        public InformationValidator(IMessageBoard messageBoard) : base(messageBoard)
        {
        }

        protected override InterceptionAction InterceptCore(Message messageData)
        {
            InformationGathered gathered = messageData.Get<InformationGathered>();
            if (gathered.Information.Contains("Invalid", StringComparison.OrdinalIgnoreCase))
            {
                OnMessage(new ExceptionMessage("Validation Failed", messageData, this));
                return InterceptionAction.DoNotPublish;
            }

            return InterceptionAction.Continue;
        }
    }
}
