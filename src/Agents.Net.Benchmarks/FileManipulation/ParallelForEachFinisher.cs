using System;
using Agents.Net;

namespace Agents.Net.Benchmarks.FileManipulation
{
    public class ParallelForEachFinisher : Agent
    {
        #region Definition

        [AgentDefinition]
        public static AgentDefinition ParallelForEachFinisherDefinition { get; }
            = new AgentDefinition(new []
                                  {
                                      FilesCompletedMessage.FilesCompletedMessageDefinition
                                  },
                                  Array.Empty<MessageDefinition>());

        #endregion

        private readonly Action finishAction;

        public ParallelForEachFinisher(IMessageBoard messageBoard, Action finishAction) : base(ParallelForEachFinisherDefinition, messageBoard)
        {
            this.finishAction = finishAction;
        }

        protected override void ExecuteCore(Message messageData)
        {
            finishAction();
        }
    }
}
