#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System.Collections.Generic;

namespace Agents.Net.Tests.Tools
{
    public abstract class DisposableMessage : Message
    {
        protected DisposableMessage(Message predecessorMessage, string name = null) : base(predecessorMessage)
        {
        }

        protected DisposableMessage(IEnumerable<Message> predecessorMessages, string name = null) : base(predecessorMessages)
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