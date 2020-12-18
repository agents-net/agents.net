#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System.Collections.Generic;
using Agents.Net;

namespace Agents.Net.Tests.Tools.Communities.DefaultDisposeCommunity.Messages
{
    public class DecoratingMessage : MessageDecorator
    {
        private DecoratingMessage(Message decoratedMessage, IEnumerable<Message> additionalPredecessors = null)
            : base(decoratedMessage, additionalPredecessors)
        {
        }

        public static DecoratingMessage Decorate(DecoratedMessage decoratedMessage,
                                          IEnumerable<Message> additionalPredecessors = null)
        {
            return new DecoratingMessage(decoratedMessage, additionalPredecessors);
        }

        protected override string DataToString()
        {
            return string.Empty;
        }
    }
}
