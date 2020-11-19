#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;
using Agents.Net;
using Agents.Net.Tests.Tools.Communities.CounterCommunity.Messages;

namespace Agents.Net.Tests.Tools.Communities.CounterCommunity.Agents
{
    [Consumes(typeof(ConsumedAllMessages))]
    [Consumes(typeof(InterceptedAllMessages))]
    public class MessageConsoleWriter : Agent
    {
        private readonly IConsole console;
        private readonly Action terminateAction;
        private readonly MessageCollector<ConsumedAllMessages, InterceptedAllMessages> collector;

        public MessageConsoleWriter(IMessageBoard messageBoard, IConsole console, Action terminateAction) : base(messageBoard)
        {
            this.console = console;
            this.terminateAction = terminateAction;
            collector = new MessageCollector<ConsumedAllMessages, InterceptedAllMessages>(OnMessagesCollected);
        }

        private void OnMessagesCollected(MessageCollection<ConsumedAllMessages, InterceptedAllMessages> set)
        {
            console.WriteLine($"Consumed: {set.Message1.Count}; Intercepted: {set.Message2.Count}");
            terminateAction();
        }

        protected override void ExecuteCore(Message messageData)
        {
            collector.Push(messageData);
        }
    }
}
