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
        public static MessageDomain GetMessageDomain(this Message[] domainSourceMessages)
        {
            if (domainSourceMessages.Length == 0)
            {
                return MessageDomain.DefaultMessageDomain;
            }

            if (!domainSourceMessages[0].MessageDomain.IsTerminated &&
                (domainSourceMessages.Length == 1 ||
                domainSourceMessages.All(m => m.MessageDomain == domainSourceMessages[0].MessageDomain)))
            {
                return domainSourceMessages[0].MessageDomain;
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

                    yield return current ?? MessageDomain.DefaultMessageDomain;
                }
            }
        }
    }
}
