#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System.Collections.Generic;
using Agents.Net;

namespace Agents.Net.Tests.Tools.Communities.CounterCommunity.Messages
{
    public class Message4 : Message
    {
        public Message4(Message predecessorMessage)
			: base(predecessorMessage)
        {
        }

        public Message4(IEnumerable<Message> predecessorMessages)
			: base(predecessorMessages)
        {
        }

        protected override string DataToString()
        {
            return string.Empty;
        }
    }
}
