using System;
using Agents.Net;

namespace Agents.Net.Benchmarks.FileManipulation
{
    [Intercepts(typeof(FileFoundMessage))]
    [Produces(typeof(RelevantFileFoundMessage))]
    public class RelevantFileFilter : InterceptorAgent
    {        public RelevantFileFilter(IMessageBoard messageBoard) : base(messageBoard)
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
