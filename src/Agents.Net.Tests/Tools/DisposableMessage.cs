using System.Collections.Generic;

namespace Agents.Net.Tests.Tools
{
    public abstract class DisposableMessage : Message
    {
        protected DisposableMessage(Message predecessorMessage, string name = null, params Message[] childMessages) : base(predecessorMessage, name, childMessages)
        {
        }

        protected DisposableMessage(IEnumerable<Message> predecessorMessages, string name = null, params Message[] childMessages) : base(predecessorMessages, name, childMessages)
        {
        }
        
        public bool IsDisposed { get; set; }

        protected override void Dispose(bool disposing)
        {
            IsDisposed = disposing;
            base.Dispose(disposing);
        }
    }
}