#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System.Collections.Generic;
using Agents.Net;

namespace Agents.Net.Tests.Tools.Communities.DelayCommunity.Messages
{
    public class InformationGathered : Message
    {
        public InformationGathered(string information, Message predecessorMessage)
            : base(predecessorMessage)
        {
            Information = information;
        }

        public InformationGathered(string information, IEnumerable<Message> predecessorMessages)
            : base(predecessorMessages)
        {
            Information = information;
        }

        public string Information { get; }

        protected override string DataToString()
        {
            return $"{nameof(Information)}: {Information}";
        }
    }
}
