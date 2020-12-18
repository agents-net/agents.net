#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using Autofac;
using Agents.Net;

namespace Agents.Net.Tests.Tools.Communities.DelayCommunity
{
    public class DelayCommunityModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Agents.InformationBroker>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<Agents.InformationTransformer>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<Agents.InformationDelayInterceptor>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<Agents.InformationConsoleWriter>().As<Agent>().InstancePerLifetimeScope();
        }
    }
}
