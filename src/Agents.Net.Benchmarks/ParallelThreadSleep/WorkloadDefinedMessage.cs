using System.Collections.Generic;
using Agents.Net;

namespace Agents.Net.Benchmarks.ParallelThreadSleep
{
    public class WorkloadDefinedMessage : Message
    {        public WorkloadDefinedMessage(int workload, Message predecessorMessage, params Message[] childMessages)
            : base(predecessorMessage, childMessages:childMessages)
        {
            Workload = workload;
        }

        public WorkloadDefinedMessage(int workload, IEnumerable<Message> predecessorMessages,
                                      params Message[] childMessages)
            : base(predecessorMessages, childMessages:childMessages)
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
