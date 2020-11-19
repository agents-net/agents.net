#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System.Collections.Generic;
using Agents.Net;

namespace Agents.Net.Tests.Tools.Communities.DecoratingInterceptorCommunity.Messages
{
    public class DetailedInformationGathered : MessageDecorator
    {
        private DetailedInformationGathered(Message decoratedMessage, string detailedInformation, IEnumerable<Message> additionalPredecessors = null)
            : base(decoratedMessage, additionalPredecessors)
        {
            DetailedInformation = detailedInformation;
        }

        public static DetailedInformationGathered Decorate(InformationGathered decoratedMessage, string detailedInformation,
                                          IEnumerable<Message> additionalPredecessors = null)
        {
            return new DetailedInformationGathered(decoratedMessage, detailedInformation, additionalPredecessors);
        }

        public string DetailedInformation { get; }

        protected override string DataToString()
        {
            return $"{nameof(DetailedInformation)}: {DetailedInformation}";
        }
    }
}
