using System;
using System.Collections.Generic;
using Agents.Net;
using Agents.Net.Tests.Tools.Communities.ParallelExecutionCommunity.Messages;

namespace Agents.Net.Tests.Tools.Communities.ParallelExecutionCommunity.Agents
{
    [Consumes(typeof(InitializeMessage))]
    [Produces(typeof(WorkScheduled))]
    public class WorkScheduler : Agent
    {
        public WorkScheduler(IMessageBoard messageBoard) : base(messageBoard)
        {
        }

        protected override void ExecuteCore(Message messageData)
        {
            List<Message> messages = new List<Message>();
            for (int i = 0; i < 4; i++)
            {
                messages.Add(new WorkScheduled(i, messageData));
            }
            OnMessages(messages);
        }
    }
}
