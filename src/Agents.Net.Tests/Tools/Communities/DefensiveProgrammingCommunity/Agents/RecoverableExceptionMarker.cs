#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;
using Agents.Net;
using Agents.Net.Tests.Tools.Communities.DefensiveProgrammingCommunity.Messages;

namespace Agents.Net.Tests.Tools.Communities.DefensiveProgrammingCommunity.Agents
{
    [Produces(typeof(HandledException))]
    [Intercepts(typeof(ExceptionMessage))]
    public class RecoverableExceptionMarker : InterceptorAgent
    {
        public RecoverableExceptionMarker(IMessageBoard messageBoard) : base(messageBoard)
        {
        }

        protected override InterceptionAction InterceptCore(Message messageData)
        {
            ExceptionMessage exceptionMessage = messageData.Get<ExceptionMessage>();
            if (exceptionMessage.ExceptionInfo?.SourceException is RecoverableException)
            {
                HandledException.Decorate(exceptionMessage);
            }

            return InterceptionAction.Continue;
        }
    }
}
