#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
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
                   .Should().BeTrue($"the message \"{message}\" was expected, but none was found.");
            receivedMessages.Should().ContainEquivalentOf(message);
        }
        
        [Then("the message \"(.*)\" was posted after at most (\\d*) ms")]
        public void ThenTheMessageWasPostedToTheConsoleAfterTimeout(string message, int timeout)
        {
            context.Get<WaitingConsole>().WaitForMessages(out IEnumerable<string> receivedMessages, timeout)
                   .Should().BeTrue($"the message \"{message}\" was expected, but none was found.");
            receivedMessages.Should().ContainEquivalentOf(message);
        }

        [Then("the program was terminated")]
        public void ThenTheProgramWasTerminated()
        {
            context.Get<ManualResetEventSlim>(TerminateEventKey).Wait(300)
                   .Should().BeTrue("program should have been terminated.");
        }        
        
        [Then(@"the program was not terminated")]
        public void ThenTheProgramWasNotTerminated()
        {
            context.Get<ManualResetEventSlim>(TerminateEventKey).Wait(300)
                   .Should().BeFalse("program should not have been terminated.");
        }    
        
        [Then(@"all messages are disposed")]
        public void ThenAllMessagesAreDisposed()
        {
            context.Get<WaitingConsole>().WaitForMessages(out _);
            context.Get<DisposeManager>().CheckAllDisposed();
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
