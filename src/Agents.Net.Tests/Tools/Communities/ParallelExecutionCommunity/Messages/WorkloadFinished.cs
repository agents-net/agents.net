using System.Collections.Generic;
using Agents.Net;

namespace Agents.Net.Tests.Tools.Communities.ParallelExecutionCommunity.Messages
{
    public class WorkloadFinished : Message
    {
        public WorkloadFinished(int totalResult, Message predecessorMessage, params Message[] childMessages)
			: base(predecessorMessage, childMessages:childMessages)
        {
            TotalResult = totalResult;
        }

        public WorkloadFinished(int totalResult, IEnumerable<Message> predecessorMessages,
            params Message[] childMessages)
			: base(predecessorMessages, childMessages:childMessages)
        {
            TotalResult = totalResult;
        }
        
        public int TotalResult { get; }

        protected override string DataToString()
        {
            return $"{nameof(TotalResult)}: {TotalResult}";
        }
    }
}
