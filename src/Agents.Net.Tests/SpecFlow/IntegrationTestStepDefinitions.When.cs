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
