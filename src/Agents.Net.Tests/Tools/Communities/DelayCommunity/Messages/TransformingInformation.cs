#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System.Collections.Generic;
using Agents.Net;

namespace Agents.Net.Tests.Tools.Communities.DelayCommunity.Messages
{
    public class TransformingInformation : Message
    {
        public TransformingInformation(string information, InformationGathered originalMessage,
                                       Message predecessorMessage)
			: base(predecessorMessage)
        {
            Information = information;
            OriginalMessage = originalMessage;
        }

        public TransformingInformation(string information, InformationGathered originalMessage,
                                       IEnumerable<Message> predecessorMessages)
			: base(predecessorMessages)
        {
            Information = information;
            OriginalMessage = originalMessage;
        }

        public string Information { get; }

        public InformationGathered OriginalMessage { get; }

        protected override string DataToString()
        {
            return $"{nameof(Information)}: {Information}; {nameof(OriginalMessage)}: {OriginalMessage}";
        }
    }
}
