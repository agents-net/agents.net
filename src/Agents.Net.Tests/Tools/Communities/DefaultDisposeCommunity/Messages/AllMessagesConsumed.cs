using System.Collections.Generic;
using Agents.Net;

namespace Agents.Net.Tests.Tools.Communities.DefaultDisposeCommunity.Messages
{
    public class AllMessagesConsumed : DisposableMessage
    {
        public AllMessagesConsumed(Message predecessorMessage)
			: base(predecessorMessage)
        {
        }

        public AllMessagesConsumed(IEnumerable<Message> predecessorMessages)
			: base(predecessorMessages)
        {
        }

        protected override string DataToString()
        {
            return string.Empty;
        }
    }
}
