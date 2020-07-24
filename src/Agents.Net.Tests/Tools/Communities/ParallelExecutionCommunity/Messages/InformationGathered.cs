using System.Collections.Generic;
using Agents.Net;

namespace Agents.Net.Tests.Tools.Communities.ParallelExecutionCommunity.Messages
{
    public class InformationGathered : Message
    {
        public InformationGathered(bool proceedWithCaution, Message predecessorMessage, params Message[] childMessages)
			: base(predecessorMessage, childMessages:childMessages)
        {
            ProceedWithCaution = proceedWithCaution;
        }

        public InformationGathered(bool proceedWithCaution, IEnumerable<Message> predecessorMessages,
            params Message[] childMessages)
			: base(predecessorMessages, childMessages:childMessages)
        {
            ProceedWithCaution = proceedWithCaution;
        }
        
        public bool ProceedWithCaution { get; }

        protected override string DataToString()
        {
            return $"{nameof(ProceedWithCaution)}: {ProceedWithCaution}";
        }
    }
}
