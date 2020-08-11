using System;
using System.Collections.Generic;
using System.Threading;
using Agents.Net.Tests.Tools;
using FluentAssertions;
using TechTalk.SpecFlow;

namespace Agents.Net.Tests.SpecFlow
{
    public sealed partial class IntegrationTestStepDefinitions
    {
        [Then("the message \"(.*)\" was posted after a while")]
        public void ThenTheMessageWasPostedToTheConsoleAfterAWhile(string message)
        {
            context.Get<WaitingConsole>().WaitForMessages(out IEnumerable<string> receivedMessages)
                   .Should().BeTrue("a message was expected, but none was found.");
            receivedMessages.Should().ContainEquivalentOf(message);
        }

        [Then("the program was terminated")]
        public void ThenTheProgramWasTerminated()
        {
            context.Get<ManualResetEventSlim>(TerminateEventKey).Wait(100)
                   .Should().BeTrue("program should have been terminated.");
        }

        [Then("the agents (.*) were executed parallel")]
        public void ThenTheProgramWasTerminated(string agentsString)
        {
            string[] agents = agentsString.Split(", ",StringSplitOptions.RemoveEmptyEntries);
            context.Get<WaitingConsole>().WaitForMessages(out _);
            context.Get<ExecutionOrder>().CheckParallelExecution(agents);
        }

        [Then("the agent (.*) executed (\\d+) messages parallel")]
        public void TheAgentExecutedMessagesParallel(string agent, int messageCount)
        {
            context.Get<WaitingConsole>().WaitForMessages(out _);
            context.Get<ExecutionOrder>().CheckParallelMessages(agent, messageCount);
        }
    }
}
