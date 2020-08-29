using System.Collections.Generic;
using Agents.Net;

namespace Agents.Net.Tests.Tools.Communities.CounterCommunity.Messages
{
    public class InterceptedAllMessages : Message
    {
        public InterceptedAllMessages(int count, Message predecessorMessage, params Message[] childMessages)
			: base(predecessorMessage, childMessages:childMessages)
        {
            Count = count;
        }

        public InterceptedAllMessages(int count, IEnumerable<Message> predecessorMessages,
                                      params Message[] childMessages)
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
