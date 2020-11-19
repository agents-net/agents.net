#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System.Collections.Generic;

namespace Agents.Net.Tests.Tools.Communities.DefensiveProgrammingCommunity.Messages
{
    public class HandledException : MessageDecorator
    {
        private HandledException(Message decoratedMessage, IEnumerable<Message> additionalPredecessors = null)
            : base(decoratedMessage, additionalPredecessors)
        {
        }

        public static HandledException Decorate(ExceptionMessage decoratedMessage,
                                          IEnumerable<Message> additionalPredecessors = null)
        {
            return new HandledException(decoratedMessage, additionalPredecessors);
        }

        protected override string DataToString()
        {
            return string.Empty;
        }
    }
}
