#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;
using Agents.Net;
using Agents.Net.Tests.Tools.Communities.DefensiveProgrammingCommunity.Messages;


namespace Agents.Net.Tests.Tools.Communities.DefensiveProgrammingCommunity.Agents
{
    [Consumes(typeof(ExceptionMessage))]
    [Consumes(typeof(HandledException), Implicitly = true)]
    public class ExceptionLogger : Agent
    {
        private readonly IConsole console;

        public ExceptionLogger(IMessageBoard messageBoard, IConsole console) : base(messageBoard)
        {
            this.console = console;
        }

        protected override void ExecuteCore(Message messageData)
        {
            ExceptionMessage exceptionMessage = messageData.Get<ExceptionMessage>();
            if (exceptionMessage.Is<HandledException>())
            {
                console.WriteLine("Recoverable Exception");
            }
            else
            {
                console.WriteLine("Unrecoverable Exception");
            }
        }
    }
}
