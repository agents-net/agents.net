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

        public ConsoleMessageCreated(Message predecessorMessage, params Message[] childMessages)
			: base(predecessorMessage, ConsoleMessageCreatedDefinition, childMessages)
        {
        }

        public ConsoleMessageCreated(IEnumerable<Message> predecessorMessages, params Message[] childMessages)
			: base(predecessorMessages, ConsoleMessageCreatedDefinition, childMessages)
        {
        }

        protected override string DataToString()
        {
            return string.Empty;
        }
    }
}
