using System;
using Agents.Net;

namespace Agents.Net.Benchmarks.FileManipulation
{
    public class GenericFileCompleter : Agent
    {
        #region Definition

        [AgentDefinition]
        public static AgentDefinition GenericFileCompleterDefinition { get; }
            = new AgentDefinition(new []
                                  {
                                      FileFoundMessage.FileFoundMessageDefinition
                                  },
                                  new []
                                  {
                                      FileCompletedMessage.FileCompletedMessageDefinition
                                  });

        #endregion

        public GenericFileCompleter(IMessageBoard messageBoard) : base(GenericFileCompleterDefinition, messageBoard)
        {
        }

        protected override void ExecuteCore(Message messageData)
        {
            if (!MessageDecorator.IsDecorated(messageData))
            {
                OnMessage(new FileCompletedMessage(messageData));
            }
        }
    }
}
