#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;
using Agents.Net;
using Agents.Net.Tests.Tools.Communities.TransactionManagerCommunity.Messages;

namespace Agents.Net.Tests.Tools.Communities.TransactionManagerCommunity.Agents
{
    [Produces(typeof(ExceptionMessage))]
    [Intercepts(typeof(TransactionStarted))]
    public class ExceptionProducer : InterceptorAgent
    {
        public ExceptionProducer(IMessageBoard messageBoard) : base(messageBoard)
        {
        }

        protected override InterceptionAction InterceptCore(Message messageData)
        {
            if (messageData.Get<TransactionStarted>().Data.Equals("error", StringComparison.OrdinalIgnoreCase))
            {
                OnMessage(new ExceptionMessage("Error during execution", messageData, this));
                return InterceptionAction.DoNotPublish;
            }

            return InterceptionAction.Continue;
        }
    }
}
