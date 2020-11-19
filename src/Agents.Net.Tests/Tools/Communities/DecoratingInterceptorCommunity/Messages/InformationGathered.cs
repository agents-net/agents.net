#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System.Collections.Generic;
using Agents.Net;

namespace Agents.Net.Tests.Tools.Communities.DecoratingInterceptorCommunity.Messages
{
    public class InformationGathered : Message
    {
        public InformationGathered(string information, bool additionalDataAvailable, Message predecessorMessage)
			: base(predecessorMessage)
        {
            Information = information;
            AdditionalDataAvailable = additionalDataAvailable;
        }

        public InformationGathered(string information, bool additionalDataAvailable,
                                   IEnumerable<Message> predecessorMessages)
			: base(predecessorMessages)
        {
            Information = information;
            AdditionalDataAvailable = additionalDataAvailable;
        }

        public string Information { get; }

        public bool AdditionalDataAvailable { get; }

        protected override string DataToString()
        {
            return $"{nameof(Information)}: {Information}; {nameof(AdditionalDataAvailable)}: {AdditionalDataAvailable}";
        }
    }
}
