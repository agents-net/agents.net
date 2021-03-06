﻿#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;
using System.Collections.Generic;
using Agents.Net;

namespace Agents.Net.Benchmarks.ParallelThreadSleep
{
    public class StartingWorkloadsMessage : Message
    {
        public StartingWorkloadsMessage(int[] workloads)
            : base(Array.Empty<Message>())
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
