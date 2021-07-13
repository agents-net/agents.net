using System.Collections.Generic;
using Agents.Net;

namespace Agents.Net.Tests.Tools.Communities.TransactionManagerCommunity.Messages
{
    public class InformationGathered : Message
    {
        public InformationGathered(Message predecessorMessage, string information): base(predecessorMessage)
        {
            Information = information;
        }

        public InformationGathered(IEnumerable<Message> predecessorMessages, string information): base(predecessorMessages)
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
