﻿using System.Collections.Generic;
using Agents.Net;

namespace Agents.Net.Tests.Tools.Communities.CounterCommunity.Messages
{
    public class Message2 : Message
    {
        public Message2(Message predecessorMessage, params Message[] childMessages)
			: base(predecessorMessage, childMessages:childMessages)
        {
        }

        public Message2(IEnumerable<Message> predecessorMessages, params Message[] childMessages)
			: base(predecessorMessages, childMessages:childMessages)
        {
        }

        protected override string DataToString()
        {
            return string.Empty;
        }
    }
}
