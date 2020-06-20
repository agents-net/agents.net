using System;
using System.Collections.Generic;
using System.Threading;

namespace Agents.Net
{
    public abstract class SelfDisposingMessage : Message, IDisposable
    {
        protected SelfDisposingMessage(Message predecessorMessage, MessageDefinition messageDefinition, params Message[] childMessages) : base(predecessorMessage, messageDefinition, childMessages)
        {
        }

        protected SelfDisposingMessage(IEnumerable<Message> predecessorMessages, MessageDefinition messageDefinition, params Message[] childMessages) : base(predecessorMessages, messageDefinition, childMessages)
        {
        }

        internal void Used()
        {
            if (Interlocked.Decrement(ref remainingUses) == 0)
            {
                Dispose();
            }
        }

        public IDisposable DelayDispose()
        {
            Interlocked.Increment(ref remainingUses);
            return new DisposableUse(this);
        }

        private int remainingUses;

        internal void SetUserCount(int userCount)
        {
            remainingUses = userCount;
            if (userCount == 0)
            {
                Dispose();
            }
        }

        protected abstract void Dispose(bool disposing);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private class DisposableUse : IDisposable
        {
            private readonly SelfDisposingMessage message;

            public DisposableUse(SelfDisposingMessage message)
            {
                this.message = message;
            }

            public void Dispose()
            {
                message.Used();
            }
        }
    }
}
