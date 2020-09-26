using System.Collections.Generic;
using Agents.Net;

namespace Agents.Net.Tests.Tools.Communities.DefaultDisposeCommunity.Messages
{
    public class DecoratedMessage : DisposableMessage
    {
        public DecoratedMessage(Message predecessorMessage)
			: base(predecessorMessage)
        {
        }

        public DecoratedMessage(IEnumerable<Message> predecessorMessages)
			: base(predecessorMessages)
        {
        }

        protected override string DataToString()
        {
            return string.Empty;
        }
    }
}
