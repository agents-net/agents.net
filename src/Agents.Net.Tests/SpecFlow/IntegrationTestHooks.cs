#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Agents.Net.Tests.Tools;
using Newtonsoft.Json.Linq;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using TechTalk.SpecFlow;

namespace Agents.Net.Tests.SpecFlow
{
    [Binding]
    public sealed class IntegrationTestHooks
    {
        // For additional details on SpecFlow hooks see http://go.specflow.org/doc-hooks
        private readonly ScenarioContext context;

        public IntegrationTestHooks(ScenarioContext injectedContext)
        {
            context = injectedContext;
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            ExecutionOrder executionOrder = new ExecutionOrder();
            context.Set(executionOrder);
            Log.Logger = new DelegateLogger(AddToExecutionOrder);
            
            void AddToExecutionOrder(LogEventLevel level, Exception exception, string messageTemplate, object[] propertyValues)
            {
                if (level == LogEventLevel.Verbose && 
                    propertyValues.OfType<AgentLog>().Count() == 1)
                {
                    executionOrder.Add(propertyValues.OfType<AgentLog>().Single());
                }
            }
        }

        private class DelegateLogger : ILogger
        {
            private readonly Action<LogEventLevel, Exception, string, object[]> delegateAction;

            public DelegateLogger(Action<LogEventLevel, Exception, string, object[]> delegateAction)
            {
                this.delegateAction = delegateAction;
            }

            public void Write(LogEvent logEvent)
            {
                throw new NotSupportedException("Not supported");
            }

            public void Write(LogEventLevel level, Exception exception, string messageTemplate, params object[] propertyValues)
            {
                delegateAction(level, exception, messageTemplate, propertyValues);
            }
        }
    }
}
