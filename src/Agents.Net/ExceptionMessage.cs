#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Collections.Generic;
using System.Runtime.ExceptionServices;

namespace Agents.Net
{
    public class ExceptionMessage : Message
    {
        public ExceptionDispatchInfo ExceptionInfo { get; }
        public string CustomMessage { get; }
        public IEnumerable<Message> Messages { get; }
        public Agent Agent { get; }

        public ExceptionMessage(ExceptionDispatchInfo exceptionInfo, Message message, Agent agent) : base(message)
        {
            ExceptionInfo = exceptionInfo;
            Messages = new []{message};
            Agent = agent;
        }

        public ExceptionMessage(string customMessage, Message message, Agent agent) : base(message)
        {
            CustomMessage = customMessage;
            Messages = new []{message};
            Agent = agent;
        }

        public ExceptionMessage(ExceptionDispatchInfo exceptionInfo, IEnumerable<Message> messages, Agent agent) : base(messages)
        {
            ExceptionInfo = exceptionInfo;
            Messages = messages;
            Agent = agent;
        }

        public ExceptionMessage(string customMessage, IEnumerable<Message> messages, Agent agent) : base(messages)
        {
            CustomMessage = customMessage;
            Messages = messages;
            Agent = agent;
        }

        protected override string DataToString()
        {
            return string.Empty;
        }
    }
}