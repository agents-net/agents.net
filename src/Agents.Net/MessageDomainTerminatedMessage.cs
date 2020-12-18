#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System.Collections.Generic;
using System.Linq;

namespace Agents.Net
{
    /// <summary>
    /// A message that shows that message domains were terminated.
    /// </summary>
    /// <remarks>
    /// This message is not send automatically. It must be send explicitly.
    /// </remarks>
    public class MessageDomainTerminatedMessage : Message
    {
        /// <summary>
        /// Gets the terminated message domains.
        /// </summary>
        public IEnumerable<MessageDomain> TerminatedDomains { get; }

        internal MessageDomainTerminatedMessage(IEnumerable<Message> lastMessages, IEnumerable<MessageDomain> terminatedDomains) 
            : base(lastMessages)
        {
            TerminatedDomains = terminatedDomains;
        }

        /// <inheritdoc />
        protected override string DataToString()
        {
            return $"{nameof(TerminatedDomains)}: {string.Join(", ", TerminatedDomains.Select(d => d.Root.Id))}";
        }
    }
}
