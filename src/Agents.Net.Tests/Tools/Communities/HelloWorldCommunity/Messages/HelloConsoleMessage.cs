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

        public HelloConsoleMessage(Message predecessorMessage, params Message[] childMessages)
			: base(predecessorMessage, HelloConsoleMessageDefinition, childMessages)
        {
        }

        public HelloConsoleMessage(IEnumerable<Message> predecessorMessages, params Message[] childMessages)
			: base(predecessorMessages, HelloConsoleMessageDefinition, childMessages)
        {
        }

        protected override string DataToString()
        {
            return string.Empty;
        }
    }
}
