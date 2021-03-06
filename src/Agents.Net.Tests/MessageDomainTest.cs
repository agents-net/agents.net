#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace Agents.Net.Tests
{
    public class MessageDomainTest
    {
        [Test]
        public void NewMessagesHaveDefaultDomain()
        {
            TestMessage message = new TestMessage();

            Assert.IsNotNull(message.MessageDomain, "message.MessageDomain != null");
        }

        [Test]
        public void NewMessagesHaveSameDomain()
        {
            TestMessage message1 = new TestMessage();
            TestMessage message2 = new TestMessage();

            Assert.AreEqual(message1.MessageDomain, message2.MessageDomain, "Domains should be equal.");
        }

        [Test]
        public void CreateDomainCreatesNewDomain()
        {
            TestMessage message1 = new TestMessage();
            TestMessage message2 = new TestMessage();
            MessageDomain.CreateNewDomainsFor(message1);

            Assert.IsNotNull(message1.MessageDomain);
            Assert.AreNotEqual(message1.MessageDomain, message2.MessageDomain, "Domains should be equal.");
        }

        [Test]
        public void CreateDomainSwitchesDomainForMessageHierarchy()
        {
            TestMessage message1 = new TestMessage();
            TestMessageDecorator decorator = TestMessageDecorator.Decorate(message1);
            MessageDomain.CreateNewDomainsFor(message1);
            
            Assert.AreEqual(message1.MessageDomain, decorator.MessageDomain, "Whole hierarchy should have the new domain.");
        }

        [Test]
        public void NewDomainIsChildOfParentDomain()
        {
            TestMessage message1 = new TestMessage();
            TestMessage message2 = new TestMessage();
            MessageDomain.CreateNewDomainsFor(message1);
            TestMessage nextMessage = new TestMessage(message1);
            MessageDomain.CreateNewDomainsFor(nextMessage);

            Assert.AreEqual(nextMessage.MessageDomain.Parent, message1.MessageDomain, "Parent should be default in this case.");
            Assert.AreEqual(1, message1.MessageDomain.Children.Count, $"Expected 1 child domain. Actual: {message2.MessageDomain.Children.Select(d => d.Root.Id)}; Default domain: {message2.MessageDomain.Root.Id}");
            Assert.AreEqual(nextMessage.MessageDomain, message1.MessageDomain.Children.First(),"Child domain should be new domain.");
        }

        [Test]
        public void NewDomainsHaveKnowFirstMessageInDomain()
        {
            TestMessage message = new TestMessage();
            MessageDomain.CreateNewDomainsFor(message);

            Assert.AreEqual(message,message.MessageDomain.Root);
        }

        [Test]
        public void NewDomainsKnowTheirSiblings()
        {
            TestMessage message1 = new TestMessage();
            TestMessage message2 = new TestMessage();
            MessageDomain.CreateNewDomainsFor(new[] {message1, message2});

            Assert.AreEqual(2, message2.MessageDomain.SiblingDomainRootMessages.Count, "Domain should know the sibling messages, which includes the message itself.");
        }

        [Test]
        public void PropagateMessageDomain()
        {
            TestMessage message1 = new TestMessage();
            TestMessage message2 = new TestMessage();
            MessageDomain.CreateNewDomainsFor(message1);

            TestMessage nextMessage1 = new TestMessage(message1);
            TestMessage nextMessage2 = new TestMessage(message2);

            Assert.AreEqual(nextMessage1.MessageDomain, message1.MessageDomain, "Domains need to be propagated.");
            Assert.AreEqual(nextMessage2.MessageDomain, message2.MessageDomain, "Domains need to be propagated.");
        }

        [Test]
        public void PropagateMostSpecificDomain()
        {
            TestMessage message1 = new TestMessage();
            TestMessage message2 = new TestMessage();
            MessageDomain.CreateNewDomainsFor(message1);

            TestMessage nextMessage = new TestMessage(new []{message1, message2});

            Assert.AreEqual(nextMessage.MessageDomain, message1.MessageDomain, "Specific domains need to be propagated.");
        }

        [Test]
        public void CannotPropagateDomainForSiblingDomains()
        {
            TestMessage message1 = new TestMessage();
            TestMessage message2 = new TestMessage();
            MessageDomain.CreateNewDomainsFor(new[] {message1, message2});

            Assert.Throws<InvalidOperationException>(() => new TestMessage(new[] {message1, message2}),
                                                     "Exception should be thrown to indicate error.");
        }

        [Test]
        public void PropagateDomainForTerminatedSiblingDomain()
        {
            TestMessage message1 = new TestMessage();
            TestMessage message2 = new TestMessage();
            MessageDomain.CreateNewDomainsFor(new []{message1, message2});
            
            MessageDomain.TerminateDomainsOf(message2);
            TestMessage message3 = new TestMessage(new []{message1, message2});
            
            Assert.AreEqual(message1.MessageDomain, message3.MessageDomain, "Non terminated domain should have been used.");
        }

        [Test]
        public void DontPropagateDomainForTerminatedSiblingDomainIfParentIsStillSibling()
        {
            TestMessage message1 = new TestMessage();
            TestMessage message2 = new TestMessage();
            MessageDomain.CreateNewDomainsFor(new []{message1, message2});
            MessageDomain.CreateNewDomainsFor(message2);
            
            MessageDomain.TerminateDomainsOf(message2);
            Assert.Throws<InvalidOperationException>(() => new TestMessage(new[] {message1, message2}),
                                                     "Exception should be thrown to indicate error.");
        }

        [Test]
        public void TerminateDomainsSetsTheTerminatedFlag()
        {
            TestMessage message1 = new TestMessage();
            TestMessage message2 = new TestMessage();
            MessageDomain.CreateNewDomainsFor(new[] {message1, message2});

            Assert.IsFalse(message1.MessageDomain.IsTerminated, "message1.MessageDomain.IsTerminated");
            Assert.IsFalse(message2.MessageDomain.IsTerminated, "message2.MessageDomain.IsTerminated");

            MessageDomain.TerminateDomainsOf(new[] {message1, message2});

            Assert.IsTrue(message1.MessageDomain.IsTerminated, "message1.MessageDomain.IsTerminated");
            Assert.IsTrue(message2.MessageDomain.IsTerminated, "message2.MessageDomain.IsTerminated");
        }

        [Test]
        public void TerminatedDomainsDoNotPropagate()
        {
            TestMessage message1 = new TestMessage();
            TestMessage message2 = new TestMessage();
            MessageDomain.CreateNewDomainsFor(message1);

            MessageDomain.TerminateDomainsOf(message1);
            TestMessage nextMessage = new TestMessage(message1);

            Assert.AreEqual(nextMessage.MessageDomain, message2.MessageDomain, "Propagate parent message domain of terminated domains.");
        }

        [Test]
        public void CreatingNewDomainsIgnoreTerminatedDomains()
        {
            TestMessage message1 = new TestMessage();
            MessageDomain.CreateNewDomainsFor(message1);
            TestMessage message2 = new TestMessage(message1);
            
            MessageDomain.TerminateDomainsOf(message1);
            MessageDomain.CreateNewDomainsFor(message2);
            
            Assert.AreEqual(message2.MessageDomain.Parent, MessageDomain.DefaultMessageDomain, "Terminated domains should be skipped when creating new domains.");
        }

        [Test]
        public void TerminatedDomainParentsForgetChildren()
        {
            TestMessage message = new TestMessage();
            MessageDomain.CreateNewDomainsFor(message);

            MessageDomain.TerminateDomainsOf(message);
            message.MessageDomain.Parent.Children.Should().NotContain(message.MessageDomain);
        }
    }
}
