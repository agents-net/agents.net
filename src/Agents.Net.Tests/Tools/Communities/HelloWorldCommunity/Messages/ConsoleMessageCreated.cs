using System.Collections.Generic;
using Agents.Net;

namespace Agents.Net.Tests.Tools.Communities.HelloWorldCommunity.Messages
{
    public class ConsoleMessageCreated : Message
    {
        public ConsoleMessageCreated(string message, Message predecessorMessage)
			: base(predecessorMessage)
        {
            Message = message;
        }

        public ConsoleMessageCreated(string message, IEnumerable<Message> predecessorMessages)
			: base(predecessorMessages)
        {
            Message = message;
        }

        public string Message { get; }

        protected override string DataToString()
        {
            return $"{nameof(Message)}: {Message}";
        }
    }
}
