#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System.Collections.Generic;
using Agents.Net;

namespace Agents.Net.Tests.Tools.Communities.DefaultDisposeCommunity.Messages
{
    public class InterceptedMessage : DisposableMessage
    {
        public InterceptedMessage(Message predecessorMessage)
			: base(predecessorMessage)
        {
        }

        public InterceptedMessage(IEnumerable<Message> predecessorMessages)
			: base(predecessorMessages)
        {
        }

        protected override string DataToString()
        {
            return string.Empty;
        }
    }
}
