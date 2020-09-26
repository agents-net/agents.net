using System.Collections.Generic;

namespace Agents.Net.Tests.Tools
{
    public abstract class DisposableMessage : Message
    {
        protected DisposableMessage(Message predecessorMessage, string name = null) : base(predecessorMessage, name)
        {
        }

        protected DisposableMessage(IEnumerable<Message> predecessorMessages, string name = null) : base(predecessorMessages, name)
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