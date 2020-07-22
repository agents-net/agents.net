using System.Collections.Generic;
using Agents.Net;

namespace Agents.Net.Benchmarks.FileManipulation
{
    public class RelevantFileFoundMessage : MessageDecorator
    {        private RelevantFileFoundMessage(Message decoratedMessage, IEnumerable<Message> additionalPredecessors = null) :
            base(decoratedMessage, additionalPredecessors)
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
