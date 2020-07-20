using System.Collections.Generic;
using Agents.Net;

namespace Agents.Net.Tests.Tools.Communities.HelloWorldCommunity.Messages
{
    public class HelloConsoleMessage : Message
    {
        #region Definition

        [MessageDefinition]
        public static MessageDefinition HelloConsoleMessageDefinition { get; } =
            new MessageDefinition(nameof(HelloConsoleMessage));

        #endregion

        public HelloConsoleMessage(string message, Message predecessorMessage, params Message[] childMessages)
			: base(predecessorMessage, HelloConsoleMessageDefinition, childMessages)
        {
            Message = message;
        }

        public HelloConsoleMessage(string message, IEnumerable<Message> predecessorMessages,
                                   params Message[] childMessages)
			: base(predecessorMessages, HelloConsoleMessageDefinition, childMessages)
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
