using System.Collections.Generic;
using Agents.Net;

namespace Agents.Net.Tests.Tools.Communities.DefaultDisposeCommunity.Messages
{
    public class SingleConsumedMessage : DisposableMessage
    {
        public SingleConsumedMessage(Message predecessorMessage)
			: base(predecessorMessage)
        {
        }

        public SingleConsumedMessage(IEnumerable<Message> predecessorMessages)
			: base(predecessorMessages)
        {
        }

        protected override string DataToString()
        {
            return string.Empty;
        }
    }
}
