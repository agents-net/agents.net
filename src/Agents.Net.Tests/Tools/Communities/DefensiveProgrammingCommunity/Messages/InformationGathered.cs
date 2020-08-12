using System.Collections.Generic;
using Agents.Net;

namespace Agents.Net.Tests.Tools.Communities.DefensiveProgrammingCommunity.Messages
{
    public class InformationGathered : Message
    {
        public InformationGathered(string information, Message predecessorMessage, params Message[] childMessages)
            : base(predecessorMessage, childMessages:childMessages)
        {
            Information = information;
        }

        public InformationGathered(string information, IEnumerable<Message> predecessorMessages,
                                   params Message[] childMessages)
            : base(predecessorMessages, childMessages:childMessages)
        {
            Information = information;
        }

        public string Information { get; }

        protected override string DataToString()
        {
            return $"{nameof(Information)}: {Information}";
        }
    }
}
