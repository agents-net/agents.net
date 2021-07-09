using System.Collections.Generic;
using Agents.Net;

namespace Agents.Net.Tests.Tools.Communities.LegacyServiceBridgeCommunity.Messages
{
    public class ServiceResult : Message
    {
        public ServiceResult(Message predecessorMessage, string result): base(predecessorMessage)
        {
            Result = result;
        }

        public ServiceResult(IEnumerable<Message> predecessorMessages, string result): base(predecessorMessages)
        {
            Result = result;
        }
        
        public string Result { get; }

        protected override string DataToString()
        {
            return $"{nameof(Result)}: {Result}";
        }
    }
}
