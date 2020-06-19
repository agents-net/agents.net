using System;
using System.Threading;
using Agents.Net;

namespace Agents.Net.Benchmarks.SequentialOverhead
{
    public class SpinWaiter : Agent
    {
        #region Definition

        [AgentDefinition]
        public static AgentDefinition SpinWaiterDefinition { get; }
            = new AgentDefinition(new []
                                  {
                                      SpinWaitCountedMessage.SpinWaitCountedMessageDefinition
                                  },
                                  new []
                                  {
                                      SpinWaitCountedMessage.SpinWaitCountedMessageDefinition
                                  });

        #endregion

        private readonly Action finishAction;

        public SpinWaiter(IMessageBoard messageBoard, Action finishAction) : base(SpinWaiterDefinition, messageBoard)
        {
            this.finishAction = finishAction;
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
                OnMessage(new SpinWaitCountedMessage(counted.CountDown-1, counted.Duration, messageData));
            }
        }
    }
}
