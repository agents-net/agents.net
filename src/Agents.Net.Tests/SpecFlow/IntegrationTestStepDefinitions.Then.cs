using System;
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
            context.Get<WaitingConsole>().WaitForMessage(out string receivedMessage)
                   .Should().BeTrue("a message was expected, but none was found.");
            receivedMessage.Should().BeEquivalentTo(message);
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
            context.Get<WaitingConsole>().WaitForMessage(out _);
            context.Get<ExecutionOrder>().CheckParallelExecution(agents);
        }
    }
}
