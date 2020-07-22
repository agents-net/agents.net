using System.Collections.Generic;
using Agents.Net;

namespace Agents.Net.Tests.Tools.Communities.HelloWorldCommunity.Messages
{
    public class ConsoleMessageCreated : Message
    {        public ConsoleMessageCreated(string message, Message predecessorMessage, params Message[] childMessages)
			: base(predecessorMessage, childMessages:childMessages)
        {
            Message = message;
        }

        public ConsoleMessageCreated(string message, IEnumerable<Message> predecessorMessages,
                                     params Message[] childMessages)
			: base(predecessorMessages, childMessages:childMessages)
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
