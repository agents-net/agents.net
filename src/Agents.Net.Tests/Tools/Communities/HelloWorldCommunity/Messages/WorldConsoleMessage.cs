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

        public WorldConsoleMessage(Message predecessorMessage, params Message[] childMessages)
			: base(predecessorMessage, WorldConsoleMessageDefinition, childMessages)
        {
        }

        public WorldConsoleMessage(IEnumerable<Message> predecessorMessages, params Message[] childMessages)
			: base(predecessorMessages, WorldConsoleMessageDefinition, childMessages)
        {
        }

        protected override string DataToString()
        {
            return string.Empty;
        }
    }
}
