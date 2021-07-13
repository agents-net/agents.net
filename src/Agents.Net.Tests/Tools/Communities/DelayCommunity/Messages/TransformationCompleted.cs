#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;
using System.Collections.Generic;
using Agents.Net;

namespace Agents.Net.Tests.Tools.Communities.DelayCommunity.Messages
{
    public class TransformationCompleted : Message
    {
        public TransformationCompleted(Message predecessorMessage)
			: base(predecessorMessage)
        {
        }

        public TransformationCompleted(IEnumerable<Message> predecessorMessages)
			: base(predecessorMessages)
        {
        }

        protected override string DataToString()
        {
            return string.Empty;
        }
    }
}
