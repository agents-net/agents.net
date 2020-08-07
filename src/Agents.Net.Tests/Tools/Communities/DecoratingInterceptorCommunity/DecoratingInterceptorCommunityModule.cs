using Autofac;
using Agents.Net;

namespace Agents.Net.Tests.Tools.Communities.DecoratingInterceptorCommunity
{
    public class DecoratingInterceptorCommunityModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Agents.InformationMessageGenerator>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<Agents.InformationBroker>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<Agents.MessageConsoleWriter>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<Agents.InformationDecorator>().As<Agent>().InstancePerLifetimeScope();
        }
    }
}
