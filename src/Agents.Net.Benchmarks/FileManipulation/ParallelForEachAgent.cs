using System;
using System.Threading.Tasks;
using Agents.Net;

namespace Agents.Net.Benchmarks.FileManipulation
{
    public class ParallelForEachAgent : Agent
    {
        #region Definition

        [AgentDefinition]
        public static AgentDefinition ParallelForEachAgentDefinition { get; }
            = new AgentDefinition(new []
                                  {
                                      AllRelevantFilesFoundMessage.AllRelevantFilesFoundMessageDefinition
                                  },
                                  new []
                                  {
                                      FilesCompletedMessage.FilesCompletedMessageDefinition
                                  });

        #endregion

        public ParallelForEachAgent(IMessageBoard messageBoard) : base(ParallelForEachAgentDefinition, messageBoard)
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
