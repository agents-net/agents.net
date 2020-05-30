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

            Assert.IsTrue(MessageDecorator.IsDecorator(decorator),"TestDecorator.IsDecorator(decorator)");
        }

        [Test]
        public void IsDecoratorReturnsTrueForDecoratorIfMessagePassed()
        {
            TestMessage message = new TestMessage(Array.Empty<Message>());
            TestDecorator decorator = new TestDecorator(message);

            Assert.IsTrue(MessageDecorator.IsDecorator(message),"TestDecorator.IsDecorator(message)");
        }

        [Test]
        public void IsDecoratorReturnsFalseForNormalMessage()
        {
            TestMessage message = new TestMessage(Array.Empty<Message>());
            
            Assert.IsFalse(MessageDecorator.IsDecorator(message),"MessageDecorator.IsDecorator(message)");
        }

        [Test]
        public void IsDecoratorReturnsThrowsExceptionForNull()
        {
            Assert.Throws<ArgumentNullException>(() => MessageDecorator.IsDecorator(null));
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
            public TestDecorator(Message decoratedMessage, IEnumerable<Message> additionalPredecessors = null) : base(decoratedMessage, new MessageDefinition("TestDecorator"), additionalPredecessors)
            {
            }

            protected override string DataToString()
            {
                return string.Empty;
            }
        }
    }
}