using System;
using System.Collections.Concurrent;
using System.Threading;
using Agents.Net;
using Agents.Net.Tests.Tools.Communities.DelayCommunity.Messages;

namespace Agents.Net.Tests.Tools.Communities.DelayCommunity.Agents
{
    [Consumes(typeof(TransformationCompleted))]
    [Produces(typeof(TransformingInformation))]
    [Intercepts(typeof(InformationGathered))]
    public class InformationDelayInterceptor : InterceptorAgent
    {
        public InformationDelayInterceptor(IMessageBoard messageBoard) : base(messageBoard)
        {
        }

        private readonly ConcurrentDictionary<InformationGathered, ManualResetEventSlim> waitHandles =
            new ConcurrentDictionary<InformationGathered, ManualResetEventSlim>();

        protected override void ExecuteCore(Message messageData)
        {
            TransformationCompleted completed = messageData.Get<TransformationCompleted>();
            if (waitHandles.TryGetValue(completed.OriginalMessage, out ManualResetEventSlim resetEvent))
            {
                resetEvent.Set();
            }
        }

        protected override InterceptionAction InterceptCore(Message messageData)
        {
            InformationGathered gathered = messageData.Get<InformationGathered>();
            if (gathered.Information.Contains("Special", StringComparison.OrdinalIgnoreCase))
            {
                using ManualResetEventSlim resetEvent = new ManualResetEventSlim(false);
                waitHandles.TryAdd(gathered, resetEvent);
                OnMessage(new TransformingInformation(gathered.Information, gathered, messageData));
                resetEvent.Wait();
            }

            return InterceptionAction.Continue;
        }
    }
}
