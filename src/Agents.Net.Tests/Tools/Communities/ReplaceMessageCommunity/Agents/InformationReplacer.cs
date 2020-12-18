#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;
using System.Linq;
using Agents.Net;
using Agents.Net.Tests.Tools.Communities.ReplaceMessageCommunity.Messages;

namespace Agents.Net.Tests.Tools.Communities.ReplaceMessageCommunity.Agents
{
    [Produces(typeof(InformationGathered))]
    [Intercepts(typeof(InformationGathered))]
    public class InformationReplacer : InterceptorAgent
    {
        public InformationReplacer(IMessageBoard messageBoard) : base(messageBoard)
        {
        }

        protected override InterceptionAction InterceptCore(Message messageData)
        {
            InformationGathered originalMessage = messageData.Get<InformationGathered>();
            if (originalMessage.Information.Contains("Special", StringComparison.OrdinalIgnoreCase))
            {
                InformationGathered newMessage = new InformationGathered("Replaced Information", Enumerable.Empty<Message>());
                originalMessage.ReplaceWith(newMessage);
                OnMessage(newMessage);
                return InterceptionAction.DoNotPublish;
            }

            return InterceptionAction.Continue;
        }
    }
}
