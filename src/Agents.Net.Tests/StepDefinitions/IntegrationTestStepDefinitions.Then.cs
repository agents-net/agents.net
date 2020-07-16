using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Agents.Net.Tests.Tools;
using Autofac;
using Autofac.Core;
using FluentAssertions;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace Agents.Net.Tests.StepDefinitions
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
    }
}
