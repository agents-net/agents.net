using System;
using Agents.Net;
using Agents.Net.Tests.Tools.Communities.HelloWorldCommunity.Messages;

namespace Agents.Net.Tests.Tools.Communities.HelloWorldCommunity.Agents
{
    public class ConsoleMessageJoiner : Agent
    {
        #region Definition

        [AgentDefinition]
        public static AgentDefinition ConsoleMessageJoinerDefinition { get; }
            = new AgentDefinition(new []
                                  {
                                      WorldConsoleMessage.WorldConsoleMessageDefinition,
                                      HelloConsoleMessage.HelloConsoleMessageDefinition
                                  },
                                  new []
                                  {
                                      ConsoleMessageCreated.ConsoleMessageCreatedDefinition
                                  });

        #endregion

        private readonly MessageCollector<HelloConsoleMessage, WorldConsoleMessage> collector;

        public ConsoleMessageJoiner(IMessageBoard messageBoard) : base(ConsoleMessageJoinerDefinition, messageBoard)
        {
            collector = new MessageCollector<HelloConsoleMessage, WorldConsoleMessage>(OnMessagesCollected);
        }

        private void OnMessagesCollected(MessageCollection<HelloConsoleMessage, WorldConsoleMessage> set)
        {
            OnMessage(new ConsoleMessageCreated($"{set.Message1.Message} {set.Message2.Message}", set));
        }

        protected override void ExecuteCore(Message messageData)
        {
            collector.Push(messageData);
        }
    }
}
