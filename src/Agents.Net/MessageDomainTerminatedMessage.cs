#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Collections.Generic;
using System.Linq;

namespace Agents.Net
{
    public class MessageDomainTerminatedMessage : Message
    {
        public IEnumerable<MessageDomain> TerminatedDomains { get; }

        internal MessageDomainTerminatedMessage(IEnumerable<Message> lastMessages, IEnumerable<MessageDomain> terminatedDomains) 
            : base(lastMessages)
        {
            TerminatedDomains = terminatedDomains;
        }

        protected override string DataToString()
        {
            return $"{nameof(TerminatedDomains)}: {string.Join(", ", TerminatedDomains.Select(d => d.Root.Id))}";
        }
    }
}
