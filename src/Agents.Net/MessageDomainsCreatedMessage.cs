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
    public class MessageDomainsCreatedMessage : Message
    {
        #region Definition

        [MessageDefinition]
        public static MessageDefinition MessageDomainsCreatedMessageDefinition { get; } =
            new MessageDefinition(nameof(MessageDomainsCreatedMessage));

        #endregion

        public Message[] DomainRootMessages { get; }

        public MessageDomainsCreatedMessage(ICollection<Message> newDomainRootMessages, Message predecessorMessage) 
            : base(predecessorMessage, MessageDomainsCreatedMessageDefinition, newDomainRootMessages.ToArray())
        {
            DomainRootMessages = newDomainRootMessages.ToArray();
            MessageDomainHelper.CreateNewDomainsFor(DomainRootMessages);
        }

        public MessageDomainsCreatedMessage(ICollection<Message> newDomainRootMessages, IEnumerable<Message> predecessorMessages) 
            : base(predecessorMessages, MessageDomainsCreatedMessageDefinition, newDomainRootMessages.ToArray())
        {
            DomainRootMessages = newDomainRootMessages.ToArray();
            MessageDomainHelper.CreateNewDomainsFor(DomainRootMessages);
        }

        public MessageDomainsCreatedMessage(Message newDomainRootMessage, Message predecessorMessage) 
            : this(new []{newDomainRootMessage}, predecessorMessage)
        {
        }

        public MessageDomainsCreatedMessage(Message newDomainRootMessage, IEnumerable<Message> predecessorMessages) 
            : this(new []{newDomainRootMessage}, predecessorMessages)
        {
        }

        protected override string DataToString()
        {
            return string.Empty;
        }
    }
}
