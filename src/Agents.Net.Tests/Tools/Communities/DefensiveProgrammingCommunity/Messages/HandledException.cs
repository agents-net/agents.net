using System.Collections.Generic;
using Agents.Net;
using Agents.Net;

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
