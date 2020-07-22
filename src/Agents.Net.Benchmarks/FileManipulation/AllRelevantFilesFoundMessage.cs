using System.Collections.Generic;
using System.IO;
using System.Linq;
using Agents.Net;

namespace Agents.Net.Benchmarks.FileManipulation
{
    public class AllRelevantFilesFoundMessage : MessageDecorator
    {        private AllRelevantFilesFoundMessage(Message decoratedMessage, IEnumerable<FileInfo> relevantInfos, IEnumerable<Message> additionalPredecessors = null) :
            base(decoratedMessage, additionalPredecessors)
        {
            RelevantInfos = relevantInfos;
        }
        
        public static AllRelevantFilesFoundMessage Decorate(AllFilesFoundMessage declaredMessage, IEnumerable<FileInfo> relevantFiles)
        {
            return new AllRelevantFilesFoundMessage(declaredMessage, relevantFiles);
        }

        public IEnumerable<FileInfo> RelevantInfos { get; }

        protected override string DataToString()
        {
            return $"{nameof(RelevantInfos)}: {RelevantInfos.Count()}";
        }
    }
}
