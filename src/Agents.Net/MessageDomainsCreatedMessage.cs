#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System.Collections.Generic;
using System.Linq;

namespace Agents.Net
{
    public class MessageDomainsCreatedMessage : Message
    {
        public IReadOnlyCollection<MessageDomain> CreatedDomains { get; }

        internal MessageDomainsCreatedMessage(IReadOnlyCollection<Message> newDomainRootMessages, IEnumerable<Message> predecessorMessages) 
            : base(predecessorMessages)
        {
            CreatedDomains = newDomainRootMessages.Select(m => m.MessageDomain).Distinct().ToArray();
        }

        protected override string DataToString()
        {
            return $"{nameof(CreatedDomains)}: {string.Join(", ", CreatedDomains.Select(d => d.Root.Id))}";
        }
    }
}
