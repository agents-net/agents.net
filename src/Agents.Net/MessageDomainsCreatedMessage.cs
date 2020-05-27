#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Agents.Net
{
    public class MessageDomainsCreatedMessage : Message
    {
        #region Definition

        [MessageDefinition]
        public static MessageDefinition MessageDomainsCreatedMessageDefinition { get; } =
            new MessageDefinition(nameof(MessageDomainsCreatedMessage));

        #endregion

        private IReadOnlyCollection<Message> DomainRootMessages { get; }

        public MessageDomainsCreatedMessage(IReadOnlyCollection<Message> newDomainRootMessages, Message predecessorMessage) 
            : base(predecessorMessage, MessageDomainsCreatedMessageDefinition)
        {
            DomainRootMessages = newDomainRootMessages;
        }

        public MessageDomainsCreatedMessage(IReadOnlyCollection<Message> newDomainRootMessages, IEnumerable<Message> predecessorMessages) 
            : base(predecessorMessages, MessageDomainsCreatedMessageDefinition)
        {
            DomainRootMessages = newDomainRootMessages;
        }

        protected override string DataToString()
        {
            return $"{nameof(DomainRootMessages)}: {string.Join(", ", DomainRootMessages.Select(m => m.Id))}";
        }
    }
}
