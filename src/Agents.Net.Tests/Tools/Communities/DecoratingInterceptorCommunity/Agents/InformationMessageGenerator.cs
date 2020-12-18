#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;
using Agents.Net;
using Agents.Net.Tests.Tools.Communities.DecoratingInterceptorCommunity.Messages;

namespace Agents.Net.Tests.Tools.Communities.DecoratingInterceptorCommunity.Agents
{
    [Consumes(typeof(InformationGathered))]
    [Consumes(typeof(DetailedInformationGathered), Implicitly = true)]
    [Produces(typeof(DisplayMessageGenerated))]
    public class InformationMessageGenerator : Agent
    {
        public InformationMessageGenerator(IMessageBoard messageBoard) : base(messageBoard)
        {
        }

        protected override void ExecuteCore(Message messageData)
        {
            InformationGathered information = messageData.Get<InformationGathered>();
            string informationMessage = information.Information;
            if (messageData.TryGet(out DetailedInformationGathered detailedInformation))
            {
                informationMessage += $" {detailedInformation.DetailedInformation}";
            }
            OnMessage(new DisplayMessageGenerated(informationMessage, messageData));
        }
    }
}
