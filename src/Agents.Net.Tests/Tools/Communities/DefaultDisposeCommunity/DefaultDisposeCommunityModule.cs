using Autofac;
using Agents.Net;

namespace Agents.Net.Tests.Tools.Communities.DefaultDisposeCommunity
{
    public class DefaultDisposeCommunityModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Agents.ConsumingAgent2>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<Agents.ConsumingAgent1>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<Agents.InterceptorDecorator>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<Agents.MessageGenerator>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<Agents.Terminator>().As<Agent>().InstancePerLifetimeScope();
        }
    }
}
