using System;
using System.Collections.Concurrent;
using Agents.Net;
using Agents.Net.Tests.Tools.Communities.CounterCommunity.Messages;

namespace Agents.Net.Tests.Tools.Communities.CounterCommunity.Agents
{
    [Produces(typeof(InterceptedAllMessages))]
    [Intercepts(typeof(Message))]
    public class InterceptingCounter : InterceptorAgent
    {
        public InterceptingCounter(IMessageBoard messageBoard) : base(messageBoard)
        {
        }
        
        private readonly ConcurrentBag<Message> counter = new ConcurrentBag<Message>();
        
        protected override InterceptionAction InterceptCore(Message messageData)
        {
            counter.Add(messageData);
            if (counter.Count == 6)
            {
                OnMessage(new InterceptedAllMessages(6, counter.ToArray()));
            }

            return InterceptionAction.Continue;
        }
    }
}
