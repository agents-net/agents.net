using System;
using System.Threading;
using Agents.Net;
using Agents.Net.Tests.Tools.Communities.DefaultDisposeCommunity.Messages;
using FluentAssertions;

namespace Agents.Net.Tests.Tools.Communities.DefaultDisposeCommunity.Agents
{
    [Produces(typeof(DecoratingMessage))]
    [Intercepts(typeof(InterceptedMessage))]
    [Intercepts(typeof(DecoratedMessage))]
    public class InterceptorDecorator : InterceptorAgent
    {
        public InterceptorDecorator(IMessageBoard messageBoard) : base(messageBoard)
        {
        }

        protected override InterceptionAction InterceptCore(Message messageData)
        {
            if (messageData is DisposableMessage disposableMessage)
            {
                //This timeout ensures that the issue https://github.com/agents-net/agents.net/issues/68 is checked
                Thread.Sleep(10);
                disposableMessage.IsDisposed.Should().BeFalse("messages should not be disposed before used.");
            }
            if (messageData.TryGet(out DecoratedMessage decoratedMessage))
            {
                DecoratingMessage.Decorate(decoratedMessage);
                return InterceptionAction.Continue;
            }

            return InterceptionAction.DoNotPublish;
        }
    }
}
