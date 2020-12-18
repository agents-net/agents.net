﻿#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System.Collections.Generic;
using Agents.Net;

namespace Agents.Net.Benchmarks.ParallelThreadSleep
{
    public class WorkloadExecutedMessage : Message
    {
        public WorkloadExecutedMessage(Message predecessorMessage)
            : base(predecessorMessage)
        {
        }

        public WorkloadExecutedMessage(IEnumerable<Message> predecessorMessages)
            : base(predecessorMessages)
        {
        }

        protected override string DataToString()
        {
            return string.Empty;
        }
    }
}
