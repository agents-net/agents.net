using System;
using System.Linq;
using Agents.Net;

namespace Agents.Net.Benchmarks.FileManipulation
{
    public class ParallelFileFilter : InterceptorAgent
    {
        #region Definition

        [AgentDefinition]
        public static InterceptorAgentDefinition ParallelFileFilterDefinition { get; }
            = new InterceptorAgentDefinition(new []
                                             {
                                                 AllFilesFoundMessage.AllFilesFoundMessageDefinition
                                             },
                                             new []
                                             {
                                                 AllRelevantFilesFoundMessage.AllRelevantFilesFoundMessageDefinition
                                             });

        #endregion

        public ParallelFileFilter(IMessageBoard messageBoard) : base(ParallelFileFilterDefinition, messageBoard)
        {
        }

        protected override InterceptionAction InterceptCore(Message messageData)
        {
            AllFilesFoundMessage files = messageData.Get<AllFilesFoundMessage>();
            AllRelevantFilesFoundMessage.Decorate(files, files.Infos.Where(i => i.IsRelevantFile()));
            return InterceptionAction.Continue;
        }
    }
}
