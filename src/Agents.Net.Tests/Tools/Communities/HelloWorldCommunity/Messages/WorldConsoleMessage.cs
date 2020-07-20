using System.Collections.Generic;
using Agents.Net;

namespace Agents.Net.Tests.Tools.Communities.HelloWorldCommunity.Messages
{
    public class WorldConsoleMessage : Message
    {
        #region Definition

        [MessageDefinition]
        public static MessageDefinition WorldConsoleMessageDefinition { get; } =
            new MessageDefinition(nameof(WorldConsoleMessage));

        #endregion

        public WorldConsoleMessage(string message, Message predecessorMessage, params Message[] childMessages)
			: base(predecessorMessage, WorldConsoleMessageDefinition, childMessages)
        {
            Message = message;
        }

        public WorldConsoleMessage(string message, IEnumerable<Message> predecessorMessages,
                                   params Message[] childMessages)
			: base(predecessorMessages, WorldConsoleMessageDefinition, childMessages)
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
