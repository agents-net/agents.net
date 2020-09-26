using System.Collections.Generic;
using Agents.Net;

namespace Agents.Net.Tests.Tools.Communities.DefaultDisposeCommunity.Messages
{
    public class UnconsumedMessage : DisposableMessage
    {
        public UnconsumedMessage(Message predecessorMessage)
			: base(predecessorMessage)
        {
        }

        public UnconsumedMessage(IEnumerable<Message> predecessorMessages)
			: base(predecessorMessages)
        {
        }

        protected override string DataToString()
        {
            return string.Empty;
        }
    }
}
