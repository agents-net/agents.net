using System.Collections.Generic;
using Agents.Net;

namespace Agents.Net.Tests.Tools.Communities.TransactionManagerCommunity.Messages
{
    public class TransactionRollback : Message
    {
        public TransactionRollback(Message predecessorMessage): base(predecessorMessage)
        {
        }

        public TransactionRollback(IEnumerable<Message> predecessorMessages): base(predecessorMessages)
        {
        }

        protected override string DataToString()
        {
            return string.Empty;
        }
    }
}
