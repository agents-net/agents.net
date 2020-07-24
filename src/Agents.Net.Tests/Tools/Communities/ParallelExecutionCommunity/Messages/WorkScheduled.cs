using System.Collections.Generic;
using Agents.Net;

namespace Agents.Net.Tests.Tools.Communities.ParallelExecutionCommunity.Messages
{
    public class WorkScheduled : Message
    {
        public WorkScheduled(int work, Message predecessorMessage, params Message[] childMessages)
			: base(predecessorMessage, childMessages:childMessages)
        {
            Work = work;
        }

        public WorkScheduled(int work, IEnumerable<Message> predecessorMessages, params Message[] childMessages)
			: base(predecessorMessages, childMessages:childMessages)
        {
            Work = work;
        }
        
        public int Work { get; }

        protected override string DataToString()
        {
            return $"{nameof(Work)}: {Work}";
        }
    }
}
