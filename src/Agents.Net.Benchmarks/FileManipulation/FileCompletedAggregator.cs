using System;
using System.Collections.Generic;
using System.Linq;
using Agents.Net;

namespace Agents.Net.Benchmarks.FileManipulation
{
    public class FileCompletedAggregator : Agent
    {
        #region Definition

        [AgentDefinition]
        public static AgentDefinition FileCompletedAggregatorDefinition { get; }
            = new AgentDefinition(new []
                                  {
                                      FileCompletedMessage.FileCompletedMessageDefinition
                                  },
                                  Array.Empty<MessageDefinition>());

        #endregion

        private readonly MessageAggregator<FileCompletedMessage> aggregator;
        private readonly Action terminateAction;

        public FileCompletedAggregator(IMessageBoard messageBoard, Action terminateAction) : base(FileCompletedAggregatorDefinition, messageBoard)
        {
            this.terminateAction = terminateAction;
            aggregator = new MessageAggregator<FileCompletedMessage>(OnAggregated);
        }

        private void OnAggregated(ICollection<FileCompletedMessage> aggregate)
        {
            FileCompletedMessage[] messageCollection = aggregate.ToArray();
            MessageDomain.TerminateDomainsOf(messageCollection);
            terminateAction();
        }

        protected override void ExecuteCore(Message messageData)
        {
            aggregator.Aggregate(messageData);
        }
    }
}
