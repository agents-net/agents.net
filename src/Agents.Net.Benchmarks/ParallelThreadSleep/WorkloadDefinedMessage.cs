using System.Collections.Generic;
using Agents.Net;

namespace Agents.Net.Benchmarks.ParallelThreadSleep
{
    public class WorkloadDefinedMessage : Message
    {
        #region Definition

        [MessageDefinition]
        public static MessageDefinition WorkloadDefinedMessageDefinition { get; } =
            new MessageDefinition(nameof(WorkloadDefinedMessage));

        #endregion

        public WorkloadDefinedMessage(int workload, Message predecessorMessage, params Message[] childMessages)
            : base(predecessorMessage, WorkloadDefinedMessageDefinition, childMessages)
        {
            Workload = workload;
        }

        public WorkloadDefinedMessage(int workload, IEnumerable<Message> predecessorMessages,
                                      params Message[] childMessages)
            : base(predecessorMessages, WorkloadDefinedMessageDefinition, childMessages)
        {
            Workload = workload;
        }

        public int Workload { get; }

        protected override string DataToString()
        {
            return $"{nameof(Workload)}: {Workload}";
        }
    }
}
