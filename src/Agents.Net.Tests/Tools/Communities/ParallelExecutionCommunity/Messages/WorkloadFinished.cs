#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System.Collections.Generic;
using Agents.Net;

namespace Agents.Net.Tests.Tools.Communities.ParallelExecutionCommunity.Messages
{
    public class WorkloadFinished : Message
    {
        public WorkloadFinished(int totalResult, Message predecessorMessage)
			: base(predecessorMessage)
        {
            TotalResult = totalResult;
        }

        public WorkloadFinished(int totalResult, IEnumerable<Message> predecessorMessages)
			: base(predecessorMessages)
        {
            TotalResult = totalResult;
        }
        
        public int TotalResult { get; }

        protected override string DataToString()
        {
            return $"{nameof(TotalResult)}: {TotalResult}";
        }
    }
}
