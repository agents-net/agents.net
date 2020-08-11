using System.Collections.Generic;
using Agents.Net;

namespace Agents.Net.Tests.Tools.Communities.DelayCommunity.Messages
{
    public class TransformingInformation : Message
    {
        public TransformingInformation(string information, InformationGathered originalMessage,
                                       Message predecessorMessage, params Message[] childMessages)
			: base(predecessorMessage, childMessages:childMessages)
        {
            Information = information;
            OriginalMessage = originalMessage;
        }

        public TransformingInformation(string information, InformationGathered originalMessage,
                                       IEnumerable<Message> predecessorMessages, params Message[] childMessages)
			: base(predecessorMessages, childMessages:childMessages)
        {
            Information = information;
            OriginalMessage = originalMessage;
        }

        public string Information { get; }

        public InformationGathered OriginalMessage { get; }

        protected override string DataToString()
        {
            return $"{nameof(Information)}: {Information}; {nameof(OriginalMessage)}: {OriginalMessage}";
        }
    }
}
