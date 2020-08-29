using Autofac;
using Agents.Net;

namespace Agents.Net.Tests.Tools.Communities.CounterCommunity
{
    public class CounterCommunityModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Agents.MessageProducer>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<Agents.InterceptingCounter>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<Agents.MessageConsoleWriter>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<Agents.MessageCounter>().As<Agent>().InstancePerLifetimeScope();
        }
    }
}
