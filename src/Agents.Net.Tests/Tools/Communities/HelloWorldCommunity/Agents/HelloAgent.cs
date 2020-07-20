﻿using System;
using Agents.Net;
using Agents.Net.Tests.Tools.Communities.HelloWorldCommunity.Messages;

namespace Agents.Net.Tests.Tools.Communities.HelloWorldCommunity.Agents
{
    public class HelloAgent : Agent
    {
        #region Definition

        [AgentDefinition]
        public static AgentDefinition HelloAgentDefinition { get; }
            = new AgentDefinition(new []
                                  {
                                      InitializeMessage.InitializeMessageDefinition
                                  },
                                  new []
                                  {
                                      HelloConsoleMessage.HelloConsoleMessageDefinition
                                  });

        #endregion

        public HelloAgent(IMessageBoard messageBoard) : base(HelloAgentDefinition, messageBoard)
        {
        }

        protected override void ExecuteCore(Message messageData)
        {
            OnMessage(new HelloConsoleMessage("Hello", messageData));
        }
    }
}
