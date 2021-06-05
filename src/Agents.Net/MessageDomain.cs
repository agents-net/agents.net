#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Agents.Net
{
    /// <summary>
    /// The time domain the message is operating in.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Each message operates in a specific message domain. Usually that is the <see cref="DefaultMessageDomain"/>. But there are situations where it is necessary to specify new domains for a message. Some of these situations are:
    /// <list type="bullet">
    /// <item><description>when message are executed explicitly in parallel with <see cref="Agent.OnMessages"/> in order to aggregate them again</description></item>
    /// <item><description>when a sequential cycle of recurring message chains is executed in order to identify the start of the cycle when the next cycle starts</description></item>
    /// <item><description>to separate message produced in a specific chain (e.g. UIEvent) from other messages</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// Given is the following example:
    /// <code>
    ///  ---------------         ------------
    /// | DefaultDomain | ----> | SubDomain1 |
    ///  ---------------  |      ------------
    /// T1 Message1       |      T1 Message2
    ///                   |      T2 Message4
    ///                   |
    ///                   |      ------------
    ///                   ----> | SubDomain2 |
    ///                          ------------
    ///                          T2 Message3
    ///                          T2 Message5
    /// </code>
    /// Message2 and Message4 should never be used in SubDomain2 as well as Message3 and Message5 should never be used in SubDomain1. But Message1 is accessible from all domains. The <see cref="MessageCollector{T1,T2}"/> implements these rules.
    /// </para>
    /// <para>
    /// New messages inherit the domain for their predecessors. That means if there is a new message based on the messages Message1 and Message4 it is automatically in the SubDomain1 as long as that domain is still active. Trying to create a new message based on the messages Message2 and Message3 will result in an exception as these are sibling domains which cannot interfere with each other.
    /// </para>
    /// </remarks>
    public class MessageDomain : IEquatable<MessageDomain>
    {
        /// <summary>
        /// The default message domain for all new messages.
        /// </summary>
        public static MessageDomain DefaultMessageDomain { get; } = new MessageDomain(new DefaultMessageDomainMessage(), null);

        private readonly List<MessageDomain> children = new List<MessageDomain>();
        private readonly ConcurrentBag<Action> terminateActions = new ConcurrentBag<Action>();

        private MessageDomain(Message root, MessageDomain parent, IReadOnlyCollection<Message> siblingDomainRootMessages = null)
        {
            while (parent?.IsTerminated == true)
            {
                parent = parent.Parent;
            }
            Root = root;
            Parent = parent;
            SiblingDomainRootMessages = siblingDomainRootMessages ?? new[] { root };
            lock (parent?.children??new object())
            {
                parent?.children.Add(this);
            }
        }

        /// <summary>
        /// Create a new domain for the <paramref name="newDomainRootMessage"/>.
        /// </summary>
        /// <param name="newDomainRootMessage">The first message in the new domain.</param>
        public static void CreateNewDomainsFor(Message newDomainRootMessage)
        {
            CreateNewDomainsFor(new[] {newDomainRootMessage});
        }

        /// <summary>
        /// Create a new domain for each of the <paramref name="newDomainRootMessages"/>.
        /// </summary>
        /// <param name="newDomainRootMessages">The first messages in the new domains.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="newDomainRootMessages"/> is null.</exception>
        public static void CreateNewDomainsFor(IReadOnlyCollection<Message> newDomainRootMessages)
        {
            if (newDomainRootMessages == null)
            {
                throw new ArgumentNullException(nameof(newDomainRootMessages));
            }
            CreateDomains();

            void CreateDomains()
            {
                foreach (Message rootMessage in newDomainRootMessages)
                {
                    rootMessage.SwitchDomain(new MessageDomain(rootMessage, rootMessage.MessageDomain, newDomainRootMessages));
                }
            }
        }

        /// <summary>
        /// Sets the domain of <paramref name="domainMessage"/> to inactive.
        /// </summary>
        /// <param name="domainMessage">The message which provides the terminated domain.</param>
        public static void TerminateDomainsOf(Message domainMessage)
        {
            TerminateDomainsOf(new[] {domainMessage});
        }


        /// <summary>
        /// Sets the domain of all <paramref name="domainMessages"/> to inactive.
        /// </summary>
        /// <param name="domainMessages">The messages which provides the terminated domains.</param>
        public static void TerminateDomainsOf(IReadOnlyCollection<Message> domainMessages)
        {
            IEnumerable<MessageDomain> terminatingDomains = domainMessages.Select(m => m.MessageDomain).Distinct().ToArray();
            foreach (MessageDomain terminatedDomain in terminatingDomains)
            {
                if(terminatedDomain == DefaultMessageDomain)
                {
                    continue;
                }
                terminatedDomain.Terminate();
            }
        }

        /// <summary>
        /// The first message of the domain.
        /// </summary>
        public Message Root { get; }

        /// <summary>
        /// The parent domain.
        /// </summary>
        public MessageDomain Parent { get; }

        /// <summary>
        /// The root messages of all message domains that were created together with this domain.
        /// </summary>
        /// <remarks>
        /// There can be more sibling domains active. This will only contain all sibling domains that were created with the <see cref="CreateNewDomainsFor(IReadOnlyCollection{Agents.Net.Message})"/> method.
        /// </remarks>
        public IReadOnlyCollection<Message> SiblingDomainRootMessages { get; }

        /// <summary>
        /// The child domains that were created in this domain.
        /// </summary>
        public IReadOnlyCollection<MessageDomain> Children
        {
            get
            {
                lock (children)
                {
                    return children.ToArray();
                }
            }
        }

        private void RemoveChild(MessageDomain child)
        {
            lock (children)
            {
                children.Remove(child);
            }
        }

        /// <inheritdoc />
        public bool Equals(MessageDomain other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Equals(Root, other.Root);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((MessageDomain) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return (Root != null ? Root.GetHashCode() : 0);
        }

        /// <summary>
        /// Checks whether both domains are equal.
        /// </summary>
        /// <param name="left">The first domain.</param>
        /// <param name="right">The second domain.</param>
        /// <returns><c>true</c>, if the domains are equal; otherwise <c>false</c>.</returns>
        public static bool operator ==(MessageDomain left, MessageDomain right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Checks whether both domains are not equal.
        /// </summary>
        /// <param name="left">The first domain.</param>
        /// <param name="right">The second domain.</param>
        /// <returns><c>true</c>, if the domains are not equal; otherwise <c>false</c>.</returns>
        public static bool operator !=(MessageDomain left, MessageDomain right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        /// If set to <c>true</c>, shows that the domain was terminated.
        /// </summary>
        public bool IsTerminated { get; private set; }

        private void Terminate()
        {
            foreach (MessageDomain messageDomain in Children)
            {
                messageDomain.Terminate();
            }
            Parent?.RemoveChild(this);
            IsTerminated = true;
            foreach (Action terminateAction in terminateActions)
            {
                terminateAction();
            }
        }

        internal void ExecuteOnTerminate(Action terminateAction)
        {
            if (this == DefaultMessageDomain)
            {
                //Default message domain is never terminated
                return;
            }

            if (IsTerminated)
            {
                terminateAction();
            }
            terminateActions.Add(terminateAction);
        }

        private class DefaultMessageDomainMessage : Message
        {
            public DefaultMessageDomainMessage() : base(Array.Empty<Message>())
            {
            }

            protected override string DataToString()
            {
                return string.Empty;
            }
        }
    }
}
