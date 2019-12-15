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
using System.Linq;

namespace Agents.Net
{
    internal static class MessageDomainHelper
    {
        private static MessageDomain DefaultMessageDomain { get; } = new MessageDomain(new DefaultMessageDomainMessage(), null);

        public static MessageDomain GetMessageDomain(this ICollection<Message> domainSourceMessages)
        {
            if (domainSourceMessages.Count == 0)
            {
                return DefaultMessageDomain;
            }
            HashSet<MessageDomain> remainingDomains = new HashSet<MessageDomain>(GetDomainsFromMessages());
            List<MessageDomain> visitedDomains = new List<MessageDomain>();
            while (remainingDomains.Except(visitedDomains).Any())
            {
                MessageDomain current = remainingDomains.Except(visitedDomains).First();
                visitedDomains.Add(current);
                IEnumerable<MessageDomain> parentTree = FlattenParents(current);
                remainingDomains.ExceptWith(parentTree);
            }

            if (remainingDomains.Count != 1)
            {
                throw new InvalidOperationException("Cannot determine message domain from sibling domains.");
            }

            return remainingDomains.Single();

            IEnumerable<MessageDomain> FlattenParents(MessageDomain current)
            {
                MessageDomain parentDomain = current.Parent;
                while (parentDomain != null)
                {
                    yield return parentDomain;
                    parentDomain = parentDomain.Parent;
                }
            }

            IEnumerable<MessageDomain> GetDomainsFromMessages()
            {
                foreach (MessageDomain messageDomain in domainSourceMessages.Select(m => m.MessageDomain))
                {
                    MessageDomain current = messageDomain;
                    while (current?.IsTerminated == true)
                    {
                        current = messageDomain.Parent;
                    }

                    yield return current ?? DefaultMessageDomain;
                }
            }
        }

        private class DefaultMessageDomainMessage : Message
        {
            public DefaultMessageDomainMessage() : base(Array.Empty<Message>(),new MessageDefinition("DefaultDomain"))
            {
            }

            protected override string DataToString()
            {
                return string.Empty;
            }
        }

        public static void CreateNewDomainsFor(IEnumerable<Message> rootMessages)
        {
            foreach (Message rootMessage in rootMessages)
            {
                rootMessage.SwitchDomain(new MessageDomain(rootMessage, rootMessage.MessageDomain));
            }
        }

        public static IEnumerable<Message> TerminateDomainsOfMessages(this IEnumerable<Message> terminatedMessages)
        {
            foreach (MessageDomain terminatedDomain in terminatedMessages.Select(m => m.MessageDomain).Distinct())
            {
                terminatedDomain.Terminate();
            }

            return terminatedMessages;
        }

        public static void TerminateDomains(IEnumerable<MessageDomain> terminatedDomains)
        {
            foreach (MessageDomain terminatedDomain in terminatedDomains)
            {
                terminatedDomain.Terminate();
            }
        }
    }
}
