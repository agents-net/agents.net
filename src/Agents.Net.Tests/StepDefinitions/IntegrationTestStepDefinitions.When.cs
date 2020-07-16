using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Autofac;
using Autofac.Core;
using FluentAssertions;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace Agents.Net.Tests.StepDefinitions
{
    public sealed partial class IntegrationTestStepDefinitions
    {
        [When("I start the message board")]
        public void WhenIStartTheMessageBoard()
        {
            context.Get<IMessageBoard>().Start();
        }
    }
}
