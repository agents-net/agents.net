using System.Collections.Generic;
using Agents.Net;

namespace Agents.Net.Benchmarks.ParallelThreadSleep
{
    public class WorkloadExecutedMessage : Message
    {
        #region Definition

        [MessageDefinition]
        public static MessageDefinition WorkloadExecutedMessageDefinition { get; } =
            new MessageDefinition(nameof(WorkloadExecutedMessage));

        #endregion

        public WorkloadExecutedMessage(Message predecessorMessage, params Message[] childMessages)
            : base(predecessorMessage, WorkloadExecutedMessageDefinition, childMessages)
        {
        }

        public WorkloadExecutedMessage(IEnumerable<Message> predecessorMessages, params Message[] childMessages)
            : base(predecessorMessages, WorkloadExecutedMessageDefinition, childMessages)
        {
        }

        protected override string DataToString()
        {
            return string.Empty;
        }
    }
}
