using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Agents.Net.Tests.Tools;
using Agents.Net.Tests.Tools.Log;
using Newtonsoft.Json;
using NLog;
using NLog.Config;
using NLog.Targets;
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

        [BeforeScenario("CollectExecutionOrderInfo")]
        public void BeforeScenario()
        {
            LoggingConfiguration emptyConfiguration = LogManager.Configuration;
            ExecutionOrder executionOrder = new ExecutionOrder();
            context.Set(emptyConfiguration);
            LoggingConfiguration executionOrderConfiguration = new LoggingConfiguration();
            Target executionOrderLogger = new MethodCallTarget("ExecutionOrderLog", (info, objects) =>
            {
                using StringReader textReader = new StringReader(info.FormattedMessage);
                using JsonReader jsonReader = new JsonTextReader(textReader);
                JsonSerializer serializer = new JsonSerializer();
                AgentLog log = serializer.Deserialize<AgentLog>(jsonReader);
                executionOrder.Add(log);
            });
            executionOrderConfiguration.AddRule(LogLevel.Trace, LogLevel.Trace, executionOrderLogger);
            LogManager.Configuration = executionOrderConfiguration;
        }

        [AfterScenario("CollectExecutionOrderInfo")]
        public void AfterScenario()
        {
            LogManager.Configuration = context.Get<LoggingConfiguration>();
        }
    }
}
