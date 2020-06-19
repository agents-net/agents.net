using System;
using Agents.Net;

namespace Agents.Net.Benchmarks.FileManipulation
{
    public class ParallelFileFinder : Agent
    {
        #region Definition

        [AgentDefinition]
        public static AgentDefinition ParallelFileFinderDefinition { get; }
            = new AgentDefinition(new []
                                  {
                                      RootDirectoryDefinedMessage.RootDirectoryDefinedMessageDefinition
                                  },
                                  new []
                                  {
                                      AllFilesFoundMessage.AllFilesFoundMessageDefinition
                                  });

        #endregion

        public ParallelFileFinder(IMessageBoard messageBoard) : base(ParallelFileFinderDefinition, messageBoard)
        {
        }

        protected override void ExecuteCore(Message messageData)
        {
            OnMessage(new AllFilesFoundMessage(messageData.Get<RootDirectoryDefinedMessage>().RootDirectory.EnumerateFiles(),messageData));
        }
    }
}
