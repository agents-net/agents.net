﻿using System;
using System.Collections.Generic;
using System.Text;
using Autofac;

namespace Agents.Net.Benchmarks.FileManipulation
{
    public class ParallelForEachModule : Module
    {
        private readonly Action finishAction;

        public ParallelForEachModule(Action finishAction)
        {
            this.finishAction = finishAction;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(finishAction);
            builder.RegisterType<MessageBoard>().As<IMessageBoard>().InstancePerLifetimeScope();
            builder.RegisterType<Community>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<ParallelFileFinder>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<ParallelForEachAgent>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<ParallelForEachFinisher>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<ParallelFileFilter>().As<Agent>().InstancePerLifetimeScope();
        }
    }
}
