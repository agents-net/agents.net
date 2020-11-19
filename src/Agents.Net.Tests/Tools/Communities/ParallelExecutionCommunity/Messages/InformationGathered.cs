#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System.Collections.Generic;
using Agents.Net;

namespace Agents.Net.Tests.Tools.Communities.ParallelExecutionCommunity.Messages
{
    public class InformationGathered : Message
    {
        public InformationGathered(bool proceedWithCaution, Message predecessorMessage)
			: base(predecessorMessage)
        {
            ProceedWithCaution = proceedWithCaution;
        }

        public InformationGathered(bool proceedWithCaution, IEnumerable<Message> predecessorMessages)
			: base(predecessorMessages)
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
