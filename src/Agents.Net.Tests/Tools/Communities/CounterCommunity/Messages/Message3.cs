using System.Collections.Generic;
using Agents.Net;

namespace Agents.Net.Tests.Tools.Communities.CounterCommunity.Messages
{
    public class Message3 : Message
    {
        public Message3(Message predecessorMessage, params Message[] childMessages)
			: base(predecessorMessage, childMessages:childMessages)
        {
        }

        public Message3(IEnumerable<Message> predecessorMessages, params Message[] childMessages)
			: base(predecessorMessages, childMessages:childMessages)
        {
        }

        protected override string DataToString()
        {
            return string.Empty;
        }
    }
}
