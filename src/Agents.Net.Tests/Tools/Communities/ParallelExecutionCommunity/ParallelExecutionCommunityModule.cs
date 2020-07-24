using Autofac;
using Agents.Net;

namespace Agents.Net.Tests.Tools.Communities.ParallelExecutionCommunity
{
    public class ParallelExecutionCommunityModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Agents.WorkDoneAggregator>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<Agents.Worker>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<Agents.ResultReporter>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<Agents.InformationBroker>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<Agents.WorkScheduler>().As<Agent>().InstancePerLifetimeScope();
        }
    }
}
