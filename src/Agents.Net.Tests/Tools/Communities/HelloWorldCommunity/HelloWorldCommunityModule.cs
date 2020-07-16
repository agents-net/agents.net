using Autofac;
using Agents.Net;

namespace Agents.Net.Tests.Tools.Communities.HelloWorldCommunity
{
    public class HelloWorldCommunityModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Agents.WorldAgent>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<Agents.ConsoleMessageJoiner>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<Agents.HelloAgent>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<Agents.ConsoleMessageDisplayAgent>().As<Agent>().InstancePerLifetimeScope();
        }
    }
}
