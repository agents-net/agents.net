#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;
using System.Threading;
using Agents.Net;

namespace Agents.Net.Benchmarks.SequentialOverhead
{
    [Consumes(typeof(SpinWaitCountedMessage))]
    [Produces(typeof(SpinWaitCountedMessage))]
    public class SpinWaiter : Agent
    {
        private readonly Action finishAction;
        private readonly bool reuseMessage;

        public SpinWaiter(IMessageBoard messageBoard, Action finishAction, bool reuseMessage) : base(messageBoard)
        {
            this.finishAction = finishAction;
            this.reuseMessage = reuseMessage;
        }

        protected override void ExecuteCore(Message messageData)
        {
            SpinWaitCountedMessage counted = messageData.Get<SpinWaitCountedMessage>();
            if (counted.CountDown == 0)
            {
                finishAction();
            }
            else
            {
                if (counted.Duration > 0)
                {
                    Thread.Sleep(counted.Duration);
                }
                else
                {
                    Thread.SpinWait(15);
                }

                if (reuseMessage)
                {
                    counted.CountDown--;
                    OnMessage(counted);
                }
                else
                {
                    OnMessage(new SpinWaitCountedMessage(counted.CountDown-1, counted.Duration, messageData));
                }
            }
        }
    }
}
