using System;
using System.Collections.Generic;
using Agents.Net;

namespace Agents.Net.Tests
{
    public class TestMessage : Message
    {        public TestMessage(Message predecessorMessage, params Message[] childMessages)
            : base(predecessorMessage, childMessages:childMessages)
        {
        }

        public TestMessage(IEnumerable<Message> predecessorMessages, params Message[] childMessages)
            : base(predecessorMessages, childMessages:childMessages)
        {
        }

        public TestMessage()
            : base(Array.Empty<Message>())
        {
        }

        protected override string DataToString()
        {
            return string.Empty;
        }
    }
}
