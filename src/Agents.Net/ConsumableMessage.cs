using System;
using System.Collections.Generic;
using System.Text;

namespace Agents.Net
{
    public abstract class ConsumableMessage : Message, IDisposable
    {
        protected ConsumableMessage(Message predecessorMessage, MessageDefinition messageDefinition, params Message[] childMessages) : base(predecessorMessage, messageDefinition, childMessages)
        {
        }

        protected ConsumableMessage(IEnumerable<Message> predecessorMessages, MessageDefinition messageDefinition, params Message[] childMessages) : base(predecessorMessages, messageDefinition, childMessages)
        {
        }

        public void Consumed()
        {
            ConsumeAction?.Invoke();
        }

        internal Action ConsumeAction { get; set; }

        protected abstract void Dispose(bool disposing);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
