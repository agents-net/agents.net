using System.Collections.Generic;
using Agents.Net;

namespace Agents.Net.Tests.Tools.Communities.ParallelExecutionCommunity.Messages
{
    public class WorkDone : Message
    {
        public WorkDone(int workResult, Message predecessorMessage, params Message[] childMessages)
			: base(predecessorMessage, childMessages:childMessages)
        {
            WorkResult = workResult;
        }

        public WorkDone(int workResult, IEnumerable<Message> predecessorMessages, params Message[] childMessages)
			: base(predecessorMessages, childMessages:childMessages)
        {
            WorkResult = workResult;
        }
        
        public int WorkResult { get; }

        protected override string DataToString()
        {
            return $"{nameof(WorkResult)}: {WorkResult}";
        }
    }
}
