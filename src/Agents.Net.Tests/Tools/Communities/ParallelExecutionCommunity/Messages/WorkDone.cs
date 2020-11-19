#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System.Collections.Generic;
using Agents.Net;

namespace Agents.Net.Tests.Tools.Communities.ParallelExecutionCommunity.Messages
{
    public class WorkDone : Message
    {
        public WorkDone(int workResult, Message predecessorMessage)
			: base(predecessorMessage)
        {
            WorkResult = workResult;
        }

        public WorkDone(int workResult, IEnumerable<Message> predecessorMessages)
			: base(predecessorMessages)
        {
            WorkResult = workResult;
        }
        
        public int WorkResult { get; }

        protected override string DataToString()
        {
            return $"{nameof(WorkResult)}: {WorkResult}";
        }
    }
}
