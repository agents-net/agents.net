using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Agents.Net.Tests.Tools;
using Autofac;
using Autofac.Core;
using FluentAssertions;
using TechTalk.SpecFlow;

namespace Agents.Net.Tests.SpecFlow
{
    [Binding]
    public sealed partial class IntegrationTestStepDefinitions
    {
        private const string TerminateEventKey = "TerminateEvent";
        // For additional details on SpecFlow step definitions see https://go.specflow.org/doc-stepdef

        private readonly ScenarioContext context;

        public IntegrationTestStepDefinitions(ScenarioContext injectedContext)
        {
            context = injectedContext;
        }

        [Given("I have loaded the community \"(.*)\"")]
        public void GivenIHavenLoadedTheCommunity(string communityName)
        {
            IContainer container = BuildContainer(communityName, out ManualResetEventSlim terminatedEvent);
            SetScenarioContext(container, terminatedEvent);
            RegisterAgents(container);
        }

        [Given(@"I pass the command line argument ""(.*)"" to the program")]
        public void GivenIPassTheCommandLineArgumentToTheProgram(string argument)
        {
            context.Set(new CommandLineArgs(argument));
        }
        
        private static void RegisterAgents(IContainer container)
        {
            Agent[] agents = container.Resolve<IEnumerable<Agent>>().ToArray();
            container.Resolve<IMessageBoard>().Register(agents);
        }

        private void SetScenarioContext(IContainer container, ManualResetEventSlim terminatedEvent)
        {
            context.Set(container.Resolve<IMessageBoard>());
            context.Set(container);
            context.Set(container.Resolve<WaitingConsole>());
            context.Set(terminatedEvent, TerminateEventKey);
        }

        private IContainer BuildContainer(string communityName, out ManualResetEventSlim terminatedEvent)
        {
            ManualResetEventSlim localEvent = new ManualResetEventSlim(false);
            terminatedEvent = localEvent;

            Type moduleType = Assembly.GetAssembly(typeof(IntegrationTestStepDefinitions))
                                      .GetType($"Agents.Net.Tests.Tools.Communities.{communityName}.{communityName}Module");
            moduleType.Should().NotBeNull($"communityName {communityName} was expected to exist.");
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterModule((IModule) Activator.CreateInstance(moduleType));
            builder.RegisterType<MessageBoard>().As<IMessageBoard>().InstancePerLifetimeScope();
            builder.RegisterType<WaitingConsole>().As<IConsole>().AsSelf().InstancePerLifetimeScope();
            if (context.ScenarioInfo.Tags.Contains("DisposeCheck"))
            {
                builder.RegisterType<DisposeManager>().AsSelf().As<Agent>().InstancePerLifetimeScope();
            }
            builder.RegisterInstance((Action) Terminate);
            if (context.TryGetValue(out CommandLineArgs args))
            {
                builder.RegisterInstance(args);
            }
            IContainer container = builder.Build();
            
            if (context.ScenarioInfo.Tags.Contains("DisposeCheck"))
            {
                context.Set(container.Resolve<DisposeManager>());
            }
            return container;

            void Terminate()
            {
                localEvent.Set();
            }
        }
    }
}
