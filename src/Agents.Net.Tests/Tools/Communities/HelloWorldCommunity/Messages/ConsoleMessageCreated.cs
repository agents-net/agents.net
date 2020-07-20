using System.Collections.Generic;
using Agents.Net;

namespace Agents.Net.Tests.Tools.Communities.HelloWorldCommunity.Messages
{
    public class ConsoleMessageCreated : Message
    {
        #region Definition

        [MessageDefinition]
        public static MessageDefinition ConsoleMessageCreatedDefinition { get; } =
            new MessageDefinition(nameof(ConsoleMessageCreated));

        #endregion

        public ConsoleMessageCreated(string message, Message predecessorMessage, params Message[] childMessages)
			: base(predecessorMessage, ConsoleMessageCreatedDefinition, childMessages)
        {
            Message = message;
        }

        public ConsoleMessageCreated(string message, IEnumerable<Message> predecessorMessages,
                                     params Message[] childMessages)
			: base(predecessorMessages, ConsoleMessageCreatedDefinition, childMessages)
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
