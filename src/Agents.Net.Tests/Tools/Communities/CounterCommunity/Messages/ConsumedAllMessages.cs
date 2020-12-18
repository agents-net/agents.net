#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System.Collections.Generic;
using Agents.Net;

namespace Agents.Net.Tests.Tools.Communities.CounterCommunity.Messages
{
    public class ConsumedAllMessages : Message
    {
        public ConsumedAllMessages(int count, Message predecessorMessage)
			: base(predecessorMessage)
        {
            Count = count;
        }

        public ConsumedAllMessages(int count, IEnumerable<Message> predecessorMessages)
			: base(predecessorMessages)
        {
            Count = count;
        }

        public int Count { get; }

        protected override string DataToString()
        {
            return $"{nameof(Count)}: {Count}";
        }
    }
}
