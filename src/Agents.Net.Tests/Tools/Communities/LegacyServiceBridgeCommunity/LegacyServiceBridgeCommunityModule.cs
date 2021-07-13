#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using Autofac;
using Agents.Net;

namespace Agents.Net.Tests.Tools.Communities.LegacyServiceBridgeCommunity
{
    public class LegacyServiceBridgeCommunityModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Agents.ExceptionProducer>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<Agents.ServiceExecutor>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<Agents.ServiceBridge>().As<Agent>().As<ILegacyService>().InstancePerLifetimeScope();
        }
    }
}
