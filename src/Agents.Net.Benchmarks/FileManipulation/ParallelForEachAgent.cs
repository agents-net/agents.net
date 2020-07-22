using System;
using System.Threading.Tasks;
using Agents.Net;

namespace Agents.Net.Benchmarks.FileManipulation
{
    [Consumes(typeof(AllRelevantFilesFoundMessage))]
    [Produces(typeof(FilesCompletedMessage))]
    public class ParallelForEachAgent : Agent
    {        public ParallelForEachAgent(IMessageBoard messageBoard) : base(messageBoard)
        {
        }

        protected override void ExecuteCore(Message messageData)
        {
            Parallel.ForEach(messageData.Get<AllRelevantFilesFoundMessage>().RelevantInfos,
                             info => info.ManipulateFile());
            OnMessage(new FilesCompletedMessage(messageData));
        }
    }
}
