using Autofac;
using Agents.Net;

namespace Agents.Net.Tests.Tools.Communities.TransactionManagerCommunity
{
    public class TransactionManagerCommunityModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Agents.ExceptionProducer>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<Agents.InformationBroker>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<Agents.TransactionScopeCreator>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<Agents.TransactionExecuter>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<Agents.Terminator>().As<Agent>().InstancePerLifetimeScope();
        }
    }
}
