using System.Collections.Generic;
using Agents.Net;

namespace Agents.Net.Benchmarks.ParallelThreadSleep
{
    public class WorkloadDefinedMessage : Message
    {
        public WorkloadDefinedMessage(int workload, Message predecessorMessage)
            : base(predecessorMessage)
        {
            Workload = workload;
        }

        public WorkloadDefinedMessage(int workload, IEnumerable<Message> predecessorMessages)
            : base(predecessorMessages)
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
