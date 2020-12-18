#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System.Collections.Generic;
using Agents.Net;

namespace Agents.Net.Tests.Tools.Communities.DelayCommunity.Messages
{
    public class TransformationCompleted : Message
    {
        public TransformationCompleted(InformationGathered originalMessage, Message predecessorMessage)
			: base(predecessorMessage)
        {
            OriginalMessage = originalMessage;
        }

        public TransformationCompleted(InformationGathered originalMessage, IEnumerable<Message> predecessorMessages)
			: base(predecessorMessages)
        {
            OriginalMessage = originalMessage;
        }

        public InformationGathered OriginalMessage { get; }

        protected override string DataToString()
        {
            return $"{nameof(OriginalMessage)}: {OriginalMessage}";
        }
    }
}
