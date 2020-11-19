#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;
using System.Collections.Concurrent;
using System.Threading;
using Agents.Net;
using Agents.Net.Tests.Tools.Communities.CounterCommunity.Messages;

namespace Agents.Net.Tests.Tools.Communities.CounterCommunity.Agents
{
    [Consumes(typeof(Message))]
    [Produces(typeof(ConsumedAllMessages))]
    public class MessageCounter : Agent
    {
        public MessageCounter(IMessageBoard messageBoard) : base(messageBoard)
        {
        }

        private readonly ConcurrentBag<Message> counter = new ConcurrentBag<Message>();

        protected override void ExecuteCore(Message messageData)
        {
            counter.Add(messageData);
            if (counter.Count == 6)
            {
                OnMessage(new ConsumedAllMessages(6, counter.ToArray()));
            }
        }
    }
}
