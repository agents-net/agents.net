using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Agents.Net.Tests
{
    public class MessageCollectorTest
    {
        [Test]
        public void SetIsExecutedWhenItHasACompleteSet()
        {
            bool executed = false;
            MessageCollector<TestMessage, OtherMessage> collector = 
                new MessageCollector<TestMessage, OtherMessage>(set =>
                {
                    executed = true;
                });
            collector.Push(new TestMessage());
            collector.Push(new OtherMessage());

            executed.Should().BeTrue("set should have been executed.");
        }
        
        [Test]
        public void SetIsExecutedEachTimeAMessageChanges()
        {
            int executed = 0;
            MessageCollector<TestMessage, OtherMessage> collector = 
                new MessageCollector<TestMessage, OtherMessage>(set =>
                {
                    executed++;
                });
            collector.Push(new TestMessage());
            collector.Push(new OtherMessage());
            collector.Push(new TestMessage());
            collector.Push(new OtherMessage());

            executed.Should().Be(3, "each new message should have triggered the collector");
        }
        
        [Test]
        public void MessagesAreNotRemovedFromCollector()
        {
            MessageCollector<TestMessage, OtherMessage> collector = 
                new MessageCollector<TestMessage, OtherMessage>();
            collector.Push(new TestMessage());
            collector.Push(new OtherMessage());

            collector.FindSetsForDomain(MessageDomain.DefaultMessageDomain)
                .Should().NotBeEmpty("no message was consumed so all messages should still exist.");
        }
        
        [Test]
        public void ConsumedMessageIsRemovedFromCollector()
        {
            MessageCollector<TestMessage, OtherMessage> collector = 
                new MessageCollector<TestMessage, OtherMessage>(set =>
                {
                    set.MarkAsConsumed(set.Message2);
                });
            collector.Push(new TestMessage());
            collector.Push(new OtherMessage());

            collector.FindSetsForDomain(MessageDomain.DefaultMessageDomain)
                .Should().BeEmpty("no complete set should exist after a message was consumed.");
        }
        
        [Test]
        public void ConsumedMessageIsRemovedFromCollectorUsingFindSets()
        {
            MessageCollector<TestMessage, OtherMessage> collector = 
                new MessageCollector<TestMessage, OtherMessage>();
            collector.Push(new TestMessage());
            collector.Push(new OtherMessage());

            MessageCollection<TestMessage, OtherMessage> set = collector.FindSetsForDomain(MessageDomain.DefaultMessageDomain)
                .Single();
            set.MarkAsConsumed(set.Message2);

            collector.FindSetsForDomain(MessageDomain.DefaultMessageDomain)
                .Should().BeEmpty("no complete set should exist after a message was consumed.");
        }
        
        [Test]
        public void SetIsExecutedEachTimeAMessageChangesWithConsumedMessage()
        {
            int executed = 0;
            MessageCollector<TestMessage, OtherMessage> collector = 
                new MessageCollector<TestMessage, OtherMessage>(set =>
                {
                    executed++;
                    set.MarkAsConsumed(set.Message2);
                });
            collector.Push(new TestMessage());
            collector.Push(new OtherMessage());
            collector.Push(new TestMessage());
            collector.Push(new OtherMessage());

            executed.Should().Be(2, "only when consumed message is added again, the collector is executed again.");
        }

        private class OtherMessage : Message
        {

            public OtherMessage()
                : base(Array.Empty<Message>())
            {
            }

            protected override string DataToString()
            {
                return string.Empty;
            }
        }
    }
}