using System.Collections.Generic;
using Agents.Net;

namespace Agents.Net.Benchmarks.SequentialOverhead
{
    public class SpinWaitCountedMessage : Message
    {        public SpinWaitCountedMessage(int countDown, int duration, Message predecessorMessage,
                                      params Message[] childMessages)
            : base(predecessorMessage, childMessages:childMessages)
        {
            CountDown = countDown;
            Duration = duration;
        }

        public SpinWaitCountedMessage(int countDown, int duration, IEnumerable<Message> predecessorMessages,
                                      params Message[] childMessages)
            : base(predecessorMessages, childMessages:childMessages)
        {
            CountDown = countDown;
            Duration = duration;
        }

        public int CountDown { get; set; }

        public int Duration { get; }

        protected override string DataToString()
        {
            return $"{nameof(CountDown)}: {CountDown}";
        }
    }
}
