using System.Collections.Generic;
using Agents.Net;

namespace Agents.Net.Tests
{
    public class TestMessage : Message
    {
        #region Definition

        [MessageDefinition]
        public static MessageDefinition TestMessageDefinition { get; } =
            new MessageDefinition(nameof(TestMessage));

        #endregion

        public TestMessage(Message predecessorMessage, params Message[] childMessages)
            : base(predecessorMessage, TestMessageDefinition, childMessages)
        {
        }

        public TestMessage(IEnumerable<Message> predecessorMessages, params Message[] childMessages)
            : base(predecessorMessages, TestMessageDefinition, childMessages)
        {
        }

        protected override string DataToString()
        {
            return string.Empty;
        }
    }
}
