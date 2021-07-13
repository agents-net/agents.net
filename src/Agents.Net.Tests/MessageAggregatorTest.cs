#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Agents.Net.Tests
{
    public class MessageAggregatorTest
    {
        [Test]
        public void AggregateSingleMessage()
        {
            bool executed = false;
            MessageAggregator<TestMessage> aggregator =
                new MessageAggregator<TestMessage>(set =>
               {
                   executed = true;
               });

            aggregator.Aggregate(new TestMessage());

            executed.Should().BeTrue("A message in the default domain should be executed immediately.");
        }

        [Test]
        public void AggregateMultipleMessagesInDifferentDomains()
        {
            bool executed = false;
            MessageAggregator<TestMessage> aggregator =
                new MessageAggregator<TestMessage>(set =>
                {
                    executed = true;
                });

            TestMessage[] messages = new[] { new TestMessage(), new TestMessage(), new TestMessage() };
            MessageDomain.CreateNewDomainsFor(messages);
            foreach (TestMessage message in messages)
            {
                aggregator.Aggregate(message);
            }

            executed.Should().BeTrue("all messages were added to the aggregator.");
        }

        [Test]
        public void ExecuteMultipleMessagesInDefaultDomainSeperately()
        {
            int count = 0;
            MessageAggregator<TestMessage> aggregator =
                new MessageAggregator<TestMessage>(set =>
                {
                    count++;
                });

            TestMessage[] messages = new[] { new TestMessage(), new TestMessage(), new TestMessage() };
            foreach (TestMessage message in messages)
            {
                aggregator.Aggregate(message);
            }

            count.Should().Be(3, "all messages should execute seperately.");
        }

        [Test]
        public void DontExecuteMultipleMessagesIfOneIsMissing()
        {
            bool executed = false;
            MessageAggregator<TestMessage> aggregator =
                new MessageAggregator<TestMessage>(set =>
                {
                    executed = true;
                });

            TestMessage[] messages = new[] { new TestMessage(), new TestMessage(), new TestMessage() };
            MessageDomain.CreateNewDomainsFor(messages);
            aggregator.Aggregate(messages[0]);
            aggregator.Aggregate(messages[1]);

            executed.Should().BeFalse("not all messages were added to the aggregator.");
        }

        [Test]
        public void TerminateMessageDomainAutomatically()
        {
            MessageAggregator<TestMessage> aggregator =
                new MessageAggregator<TestMessage>(set =>
                {
                });

            TestMessage[] messages = new[] { new TestMessage(), new TestMessage(), new TestMessage() };
            MessageDomain.CreateNewDomainsFor(messages);
            foreach (TestMessage message in messages)
            {
                aggregator.Aggregate(message);
            }

            foreach (TestMessage message in messages)
            {
                message.MessageDomain.IsTerminated.Should().BeTrue("message domain should have been terminated.");
            }
        }

        [Test]
        public void DoNotTerminateMessageDomainWhenParameterIsFalse()
        {
            MessageAggregator<TestMessage> aggregator =
                new MessageAggregator<TestMessage>(set =>
                {
                }, false);

            TestMessage[] messages = new[] { new TestMessage(), new TestMessage(), new TestMessage() };
            MessageDomain.CreateNewDomainsFor(messages);
            foreach (TestMessage message in messages)
            {
                aggregator.Aggregate(message);
            }

            foreach (TestMessage message in messages)
            {
                message.MessageDomain.IsTerminated.Should().BeFalse("message domain should not have been terminated.");
            }
        }

        [Test]
        public void DoNotTerminateDefaultDomain()
        {
            MessageAggregator<TestMessage> aggregator =
                new MessageAggregator<TestMessage>(set =>
                {
                });

            aggregator.Aggregate(new TestMessage());

            MessageDomain.DefaultMessageDomain.IsTerminated.Should().BeFalse("the default message domain should never be terminated.");
        }
    }
}
