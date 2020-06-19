using System;
using Agents.Net;

namespace Agents.Net.Benchmarks.FileManipulation
{
    public class FileManipulator : Agent
    {
        #region Definition

        [AgentDefinition]
        public static AgentDefinition FileManipulatorDefinition { get; }
            = new AgentDefinition(new []
                                  {
                                      RelevantFileFoundMessage.RelevantFileFoundMessageDefinition
                                  },
                                  new []
                                  {
                                      FileCompletedMessage.FileCompletedMessageDefinition
                                  });

        #endregion

        public FileManipulator(IMessageBoard messageBoard) : base(FileManipulatorDefinition, messageBoard)
        {
        }

        protected override void ExecuteCore(Message messageData)
        {
            messageData.Get<FileFoundMessage>().File.ManipulateFile();
            OnMessage(new FileCompletedMessage(messageData));
        }
    }
}
