#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;
using Agents.Net.Tests.Tools.Communities.DelayCommunity.Messages;

namespace Agents.Net.Tests.Tools.Communities.DelayCommunity.Agents
{
    [Intercepts(typeof(InformationGathered))]
    public class InterceptionConflictProducer : InterceptorAgent
    {
        public InterceptionConflictProducer(IMessageBoard messageBoard)
            : base(messageBoard)
        {
        }

        protected override InterceptionAction InterceptCore(Message messageData)
        {
            return messageData.Get<InformationGathered>().Information
                              .Contains("Conflict", StringComparison.OrdinalIgnoreCase)
                       ? InterceptionAction.DoNotPublish
                       : InterceptionAction.Continue;
        }
    }
}