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
        #region Definition

        [MessageDefinition]
        public static MessageDefinition MessageDomainTerminatedMessageDefinition { get; } =
            new MessageDefinition(nameof(MessageDomainTerminatedMessage));

        #endregion

        public IEnumerable<MessageDomain> TerminatedDomains { get; }

        public MessageDomainTerminatedMessage(Message messageToTerminate) 
            : this(new []{messageToTerminate})
        {
        }

        public MessageDomainTerminatedMessage(ICollection<Message> messagesToTerminate) 
            : base(messagesToTerminate.TerminateDomainsOfMessages(), MessageDomainTerminatedMessageDefinition)
        {
            TerminatedDomains = messagesToTerminate.Select(m => m.MessageDomain).Distinct().ToArray();
        }

        protected override string DataToString()
        {
            return $"{nameof(TerminatedDomains)}: {string.Join(", ", TerminatedDomains)}";
        }
    }
}
