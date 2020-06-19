using System.Collections.Generic;
using Agents.Net;

namespace Agents.Net.Benchmarks.FileManipulation
{
    public class RelevantFileFoundMessage : MessageDecorator
    {
        #region Definition

        [MessageDefinition]
        public static MessageDefinition RelevantFileFoundMessageDefinition { get; } =
            new MessageDefinition(nameof(RelevantFileFoundMessage));

        #endregion

        private RelevantFileFoundMessage(Message decoratedMessage, IEnumerable<Message> additionalPredecessors = null) :
            base(decoratedMessage, RelevantFileFoundMessageDefinition, additionalPredecessors)
        {
        }
        
        public static RelevantFileFoundMessage Decorate(FileFoundMessage declaredMessage)
        {
            return new RelevantFileFoundMessage(declaredMessage);
        }

        protected override string DataToString()
        {
            return string.Empty;
        }
    }
}
