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