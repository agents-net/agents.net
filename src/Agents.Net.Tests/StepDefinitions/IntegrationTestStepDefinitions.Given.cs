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
    [Binding]
    public sealed partial class IntegrationTestStepDefinitions
    {
        // For additional details on SpecFlow step definitions see https://go.specflow.org/doc-stepdef

        private readonly ScenarioContext context;

        public IntegrationTestStepDefinitions(ScenarioContext injectedContext)
        {
            context = injectedContext;
        }

        [Given("I have loaded the community (.*)")]
        public void GivenIHavenLoadedTheCommunity(string communityName)
        {
            Type moduleType = Assembly.GetAssembly(typeof(IntegrationTestStepDefinitions))
                                      .GetType($"Agents.Net.Tests.Tools.Communities.{communityName}.{communityName}Module");
            moduleType.Should().NotBeNull($"communityName {communityName} was expected to exist.");
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterModule((IModule) Activator.CreateInstance(moduleType));
            builder.RegisterType<Community>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<MessageBoard>().As<IMessageBoard>().InstancePerLifetimeScope();
            builder.RegisterType<WaitingConsole>().As<IConsole>().AsSelf().InstancePerLifetimeScope();
            IContainer container = builder.Build();
            IMessageBoard messageBoard = container.Resolve<IMessageBoard>();
            Community community = container.Resolve<Community>();
            Agent[] agents = container.Resolve<IEnumerable<Agent>>().ToArray();
            community.RegisterAgents(agents);
            context.Set(messageBoard);
            context.Set(container);
            context.Set(container.Resolve<WaitingConsole>());
        }
    }
}
