#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using Autofac;
using Agents.Net;

namespace Agents.Net.Tests.Tools.Communities.DefensiveProgrammingCommunity
{
    public class DefensiveProgrammingCommunityModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Agents.FaultyInterceptor>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<Agents.FaultyAgent>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<Agents.ExceptionTerminator>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<Agents.ExceptionLogger>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<Agents.RecoverableExceptionMarker>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<Agents.InformationBroker>().As<Agent>().InstancePerLifetimeScope();
        }
    }
}
