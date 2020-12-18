#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Autofac;

namespace Agents.Net.Benchmarks.FileManipulation
{
    internal class FileManipulationModule : Module
    {
        private readonly Action terminateAction;

        public FileManipulationModule(Action terminateAction)
        {
            this.terminateAction = terminateAction;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(terminateAction);
            builder.RegisterType<MessageBoard>().As<IMessageBoard>().InstancePerLifetimeScope();
            builder.RegisterType<FileCompletedAggregator>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<FileFinder>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<FileManipulator>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<GenericFileCompleter>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<RelevantFileFilter>().As<Agent>().InstancePerLifetimeScope();
            base.Load(builder);
        }
    }
}
