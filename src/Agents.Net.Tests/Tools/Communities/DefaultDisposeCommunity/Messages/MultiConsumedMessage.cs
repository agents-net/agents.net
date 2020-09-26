using System.Collections.Generic;
using Agents.Net;

namespace Agents.Net.Tests.Tools.Communities.DefaultDisposeCommunity.Messages
{
    public class MultiConsumedMessage : DisposableMessage
    {
        public MultiConsumedMessage(Message predecessorMessage)
			: base(predecessorMessage)
        {
        }

        public MultiConsumedMessage(IEnumerable<Message> predecessorMessages)
			: base(predecessorMessages)
        {
        }

        protected override string DataToString()
        {
            return string.Empty;
        }
    }
}
