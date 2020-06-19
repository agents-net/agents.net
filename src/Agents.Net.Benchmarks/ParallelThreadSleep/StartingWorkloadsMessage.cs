using System;
using System.Collections.Generic;
using Agents.Net;

namespace Agents.Net.Benchmarks.ParallelThreadSleep
{
    public class StartingWorkloadsMessage : Message
    {
        #region Definition

        [MessageDefinition]
        public static MessageDefinition StartingWorkloadsMessageDefinition { get; } =
            new MessageDefinition(nameof(StartingWorkloadsMessage));

        #endregion

        public StartingWorkloadsMessage(int[] workloads)
            : base(Array.Empty<Message>(), StartingWorkloadsMessageDefinition)
        {
            Workloads = workloads;
        }

        public int[] Workloads { get; }

        protected override string DataToString()
        {
            return string.Empty;
        }
    }
}
