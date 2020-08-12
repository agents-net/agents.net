using Autofac;
using Agents.Net;

namespace Agents.Net.Tests.Tools.Communities.ReplaceMessageCommunity
{
    public class ReplaceMessageCommunityModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Agents.InformationBroker>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<Agents.InformationConsoleWriter>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<Agents.InformationReplacer>().As<Agent>().InstancePerLifetimeScope();
        }
    }
}
