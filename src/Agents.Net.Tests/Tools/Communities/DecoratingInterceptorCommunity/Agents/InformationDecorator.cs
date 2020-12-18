#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;
using Agents.Net;
using Agents.Net.Tests.Tools.Communities.DecoratingInterceptorCommunity.Messages;

namespace Agents.Net.Tests.Tools.Communities.DecoratingInterceptorCommunity.Agents
{
    [Produces(typeof(DetailedInformationGathered))]
    [Intercepts(typeof(InformationGathered))]
    public class InformationDecorator : InterceptorAgent
    {
        public InformationDecorator(IMessageBoard messageBoard) : base(messageBoard)
        {
        }

        protected override InterceptionAction InterceptCore(Message messageData)
        {
            InformationGathered gathered = messageData.Get<InformationGathered>();
            if (gathered.AdditionalDataAvailable)
            {
                DetailedInformationGathered.Decorate(gathered, "Detailed");
            }

            return InterceptionAction.Continue;
        }
    }
}
