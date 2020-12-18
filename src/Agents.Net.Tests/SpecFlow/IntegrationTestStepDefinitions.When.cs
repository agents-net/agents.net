#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

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
    }
}
