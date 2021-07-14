#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

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
        private readonly MessageGate<TransformingInformation, TransformationCompleted> gate = new();
        private readonly IConsole console;
        public InformationDelayInterceptor(IMessageBoard messageBoard, IConsole console) : base(messageBoard)
        {
            this.console = console;
        }

        protected override void ExecuteCore(Message messageData)
        {
            gate.Check(messageData);
        }

        protected override InterceptionAction InterceptCore(Message messageData)
        {
            InformationGathered gathered = messageData.Get<InformationGathered>();
            InterceptionAction result = InterceptionAction.Delay(out InterceptionDelayToken token);
            gate.SendAndContinue(new TransformingInformation(gathered.Information, gathered), OnMessage, _ =>
            {
                if (gathered.Information.Contains("DoNotPublish", StringComparison.OrdinalIgnoreCase))
                {
                    token.Release(DelayTokenReleaseIntention.DoNotPublish);
                    console.WriteLine("Delayed message was not published.");
                }
                else
                {
                    token.Release();
                }
            });
            return result;
        }
    }
}
