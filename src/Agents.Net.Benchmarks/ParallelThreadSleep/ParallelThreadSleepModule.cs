﻿#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using Autofac;

namespace Agents.Net.Benchmarks.ParallelThreadSleep
{
    public class ParallelThreadSleepModule : Module
    {
        private readonly Action finishAction;

        public ParallelThreadSleepModule(Action finishAction)
        {
            this.finishAction = finishAction;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(finishAction);
            builder.RegisterType<MessageBoard>().As<IMessageBoard>().InstancePerLifetimeScope();
            builder.RegisterType<WorkloadExecuter>().As<Agent>().InstancePerLifetimeScope();
            builder.RegisterType<WorkloadStarter>().As<Agent>().InstancePerLifetimeScope();
        }
    }
}
