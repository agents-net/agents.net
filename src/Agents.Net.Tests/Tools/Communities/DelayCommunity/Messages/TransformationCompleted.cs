using System.Collections.Generic;
using Agents.Net;

namespace Agents.Net.Tests.Tools.Communities.DelayCommunity.Messages
{
    public class TransformationCompleted : Message
    {
        public TransformationCompleted(InformationGathered originalMessage, Message predecessorMessage,
                                       params Message[] childMessages)
			: base(predecessorMessage, childMessages:childMessages)
        {
            OriginalMessage = originalMessage;
        }

        public TransformationCompleted(InformationGathered originalMessage, IEnumerable<Message> predecessorMessages,
                                       params Message[] childMessages)
			: base(predecessorMessages, childMessages:childMessages)
        {
            OriginalMessage = originalMessage;
        }

        public InformationGathered OriginalMessage { get; }

        protected override string DataToString()
        {
            return $"{nameof(OriginalMessage)}: {OriginalMessage}";
        }
    }
}
