using System;
using System.Collections.Generic;
using Agents.Net;

namespace Agents.Net.Tests
{
    public class TestMessage : Message
    {
        public TestMessage(Message predecessorMessage)
            : base(predecessorMessage)
        {
        }

        public TestMessage(IEnumerable<Message> predecessorMessages)
            : base(predecessorMessages)
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
