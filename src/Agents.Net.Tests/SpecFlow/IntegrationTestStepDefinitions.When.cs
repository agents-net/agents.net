#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using Agents.Net.Tests.Tools.Communities.LegacyServiceBridgeCommunity;
using Autofac;
using TechTalk.SpecFlow;

namespace Agents.Net.Tests.SpecFlow
{
    public sealed partial class IntegrationTestStepDefinitions
    {
        [When("I start the message board")]
        public void WhenIStartTheMessageBoard()
        {
            context.Get<IMessageBoard>().Start();
        }
        
        [When("Call the legacy service with the data \"(true|false)\"")]
        public void WhenICallTheLegacyService(bool data)
        {
            string result = context.Get<IContainer>().Resolve<ILegacyService>().ServiceCall(data);
            context.Set(result, "LegacyServiceResult");
        }
    }
}
