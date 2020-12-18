#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Agents.Net.Tests
{
    public class MessageDecoratorTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void IsDecoratorReturnsTrueForDecorator()
        {
            TestMessage message = new TestMessage(Array.Empty<Message>());
            TestDecorator decorator = new TestDecorator(message);

            Assert.IsTrue(MessageDecorator.IsDecorated(decorator),"TestDecorator.IsDecorated(decorator)");
        }

        [Test]
        public void IsDecoratorReturnsTrueForDecoratorIfMessagePassed()
        {
            TestMessage message = new TestMessage(Array.Empty<Message>());
            TestDecorator decorator = new TestDecorator(message);

            Assert.IsTrue(MessageDecorator.IsDecorated(message),"TestDecorator.IsDecorated(message)");
        }

        [Test]
        public void IsDecoratorReturnsFalseForNormalMessage()
        {
            TestMessage message = new TestMessage(Array.Empty<Message>());
            
            Assert.IsFalse(MessageDecorator.IsDecorated(message),"MessageDecorator.IsDecorated(message)");
        }

        [Test]
        public void IsDecoratorReturnsThrowsExceptionForNull()
        {
            Assert.Throws<ArgumentNullException>(() => MessageDecorator.IsDecorated(null));
        }

        [Test]
        public void DecoratorAdaptsMessageDomain()
        {
            TestMessage message = new TestMessage(Array.Empty<Message>());
            MessageDomain.CreateNewDomainsFor(message);

            TestDecorator decorator = new TestDecorator(message);

            Assert.AreEqual(message.MessageDomain, decorator.MessageDomain, "Decorator should be in same domain as message.");
        }

        private class TestDecorator : MessageDecorator
        {
            public TestDecorator(Message decoratedMessage, IEnumerable<Message> additionalPredecessors = null) : base(decoratedMessage, additionalPredecessors)
            {
            }

            protected override string DataToString()
            {
                return string.Empty;
            }
        }
    }
}