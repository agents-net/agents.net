using System.Collections.Generic;
using Agents.Net;

namespace Agents.Net.Tests.Tools.Communities.CounterCommunity.Messages
{
    public class ConsumedAllMessages : Message
    {
        public ConsumedAllMessages(int count, Message predecessorMessage, params Message[] childMessages)
			: base(predecessorMessage, childMessages:childMessages)
        {
            Count = count;
        }

        public ConsumedAllMessages(int count, IEnumerable<Message> predecessorMessages, params Message[] childMessages)
			: base(predecessorMessages, childMessages:childMessages)
        {
            Count = count;
        }

        public int Count { get; }

        protected override string DataToString()
        {
            return $"{nameof(Count)}: {Count}";
        }
    }
}
