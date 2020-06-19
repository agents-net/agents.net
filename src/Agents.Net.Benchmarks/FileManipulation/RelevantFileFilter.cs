using System;
using Agents.Net;

namespace Agents.Net.Benchmarks.FileManipulation
{
    public class RelevantFileFilter : InterceptorAgent
    {
        #region Definition

        [AgentDefinition]
        public static InterceptorAgentDefinition RelevantFileFilterDefinition { get; }
            = new InterceptorAgentDefinition(new []
                                             {
                                                 FileFoundMessage.FileFoundMessageDefinition
                                             },
                                             new []
                                             {
                                                 RelevantFileFoundMessage.RelevantFileFoundMessageDefinition
                                             });

        #endregion

        public RelevantFileFilter(IMessageBoard messageBoard) : base(RelevantFileFilterDefinition, messageBoard)
        {
        }

        protected override InterceptionAction InterceptCore(Message messageData)
        {
            FileFoundMessage fileFoundMessage = messageData.Get<FileFoundMessage>();
            if (fileFoundMessage.File.IsRelevantFile())
            {
                RelevantFileFoundMessage.Decorate(fileFoundMessage);
            }

            return InterceptionAction.Continue;
        }
    }
}
