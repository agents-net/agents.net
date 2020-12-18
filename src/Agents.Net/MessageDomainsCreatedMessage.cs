#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System.Collections.Generic;
using System.Linq;

namespace Agents.Net
{
    /// <summary>
    /// A message that shows that new message domains were created.
    /// </summary>
    /// <remarks>
    /// This message is not send automatically. It must be send explicitly or using the <see cref="Agent.OnMessages"/> method.
    /// </remarks>
    public class MessageDomainsCreatedMessage : Message
    {
        /// <summary>
        /// gets the created message domains.
        /// </summary>
        public IReadOnlyCollection<MessageDomain> CreatedDomains { get; }

        internal MessageDomainsCreatedMessage(IReadOnlyCollection<Message> newDomainRootMessages, IEnumerable<Message> predecessorMessages) 
            : base(predecessorMessages)
        {
            CreatedDomains = newDomainRootMessages.Select(m => m.MessageDomain).Distinct().ToArray();
        }

        /// <inheritdoc />
        protected override string DataToString()
        {
            return $"{nameof(CreatedDomains)}: {string.Join(", ", CreatedDomains.Select(d => d.Root.Id))}";
        }
    }
}
