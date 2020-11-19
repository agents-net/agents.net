#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using Autofac;
using Agents.Net;

namespace Agents.Net.Tests.Tools.Communities.PreconditionCheckCommunity
{
    public class PreconditionCheckCommunityModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Agents.ExceptionConsoleWriter>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<Agents.InformationConsoleWriter>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<Agents.InformationBroker>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<Agents.InformationValidator>().As<Agent>().InstancePerLifetimeScope();
        }
    }
}
