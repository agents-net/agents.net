#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;
using Agents.Net;
using Agents.Net.Tests.Tools.Communities.DecoratingInterceptorCommunity.Messages;

namespace Agents.Net.Tests.Tools.Communities.DecoratingInterceptorCommunity.Agents
{
    [Consumes(typeof(DisplayMessageGenerated))]
    public class MessageConsoleWriter : Agent
    {
        private readonly IConsole console;
        private readonly Action terminateAction;

        public MessageConsoleWriter(IMessageBoard messageBoard, IConsole console, Action terminateAction) : base(messageBoard)
        {
            this.console = console;
            this.terminateAction = terminateAction;
        }

        protected override void ExecuteCore(Message messageData)
        {
            DisplayMessageGenerated messageGenerated = messageData.Get<DisplayMessageGenerated>();
            console.WriteLine(messageGenerated.DisplayMessage);
            terminateAction();
        }
    }
}
