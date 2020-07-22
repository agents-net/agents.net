using System;
using Agents.Net;

namespace Agents.Net.Benchmarks.FileManipulation
{
    [Consumes(typeof(RootDirectoryDefinedMessage))]
    [Produces(typeof(AllFilesFoundMessage))]
    public class ParallelFileFinder : Agent
    {        public ParallelFileFinder(IMessageBoard messageBoard) : base(messageBoard)
        {
        }

        protected override void ExecuteCore(Message messageData)
        {
            OnMessage(new AllFilesFoundMessage(messageData.Get<RootDirectoryDefinedMessage>().RootDirectory.EnumerateFiles(),messageData));
        }
    }
}
