using System.Collections.Generic;
using Agents.Net;

namespace Agents.Net.Tests.Tools.Communities.ParallelExecutionCommunity.Messages
{
    public class WorkScheduled : Message
    {
        public WorkScheduled(int work, Message predecessorMessage)
			: base(predecessorMessage)
        {
            Work = work;
        }

        public WorkScheduled(int work, IEnumerable<Message> predecessorMessages)
			: base(predecessorMessages)
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
