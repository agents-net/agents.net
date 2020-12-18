#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System.Collections.Generic;
using Agents.Net;

namespace Agents.Net.Benchmarks.SequentialOverhead
{
    public class SpinWaitCountedMessage : Message
    {
        public SpinWaitCountedMessage(int countDown, int duration, Message predecessorMessage)
            : base(predecessorMessage)
        {
            CountDown = countDown;
            Duration = duration;
        }

        public SpinWaitCountedMessage(int countDown, int duration, IEnumerable<Message> predecessorMessages)
            : base(predecessorMessages)
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
