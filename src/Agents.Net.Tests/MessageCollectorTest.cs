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

        #region Mark as consumable for higher order collectors
        
        
        [Test]
        public void ThirdConsumedMessageIsRemovedFromCollector()
        {
            MessageCollector<TestMessage, OtherMessage, OtherMessage2> collector = 
                new MessageCollector<TestMessage, OtherMessage, OtherMessage2>(set =>
                {
                    set.MarkAsConsumed(set.Message3);
                });
            collector.Push(new TestMessage());
            collector.Push(new OtherMessage());
            collector.Push(new OtherMessage2());

            collector.FindSetsForDomain(MessageDomain.DefaultMessageDomain)
                     .Should().BeEmpty("no complete set should exist after a message was consumed.");
        }
        
        
        [Test]
        public void SetIsExecutedEachTimeAMessageChangesWithThirdMessageConsumed()
        {
            int executed = 0;
            MessageCollector<TestMessage, OtherMessage, OtherMessage2> collector = 
                new MessageCollector<TestMessage, OtherMessage, OtherMessage2>(set =>
                {
                    executed++;
                    set.MarkAsConsumed(set.Message3);
                });
            collector.Push(new TestMessage());
            collector.Push(new OtherMessage());
            collector.Push(new OtherMessage2());
            collector.Push(new TestMessage());
            collector.Push(new OtherMessage2());

            executed.Should().Be(2, "only when consumed message is added again, the collector is executed again.");
        }
        
        [Test]
        public void FourthConsumedMessageIsRemovedFromCollector()
        {
            MessageCollector<TestMessage, OtherMessage, OtherMessage2, OtherMessage3> collector = 
                new MessageCollector<TestMessage, OtherMessage, OtherMessage2, OtherMessage3>(set =>
                {
                    set.MarkAsConsumed(set.Message4);
                });
            collector.Push(new TestMessage());
            collector.Push(new OtherMessage());
            collector.Push(new OtherMessage2());
            collector.Push(new OtherMessage3());

            collector.FindSetsForDomain(MessageDomain.DefaultMessageDomain)
                     .Should().BeEmpty("no complete set should exist after a message was consumed.");
        }
        
        
        [Test]
        public void SetIsExecutedEachTimeAMessageChangesWithFourthMessageConsumed()
        {
            int executed = 0;
            MessageCollector<TestMessage, OtherMessage, OtherMessage2, OtherMessage3> collector = 
                new MessageCollector<TestMessage, OtherMessage, OtherMessage2, OtherMessage3>(set =>
                {
                    executed++;
                    set.MarkAsConsumed(set.Message4);
                });
            collector.Push(new TestMessage());
            collector.Push(new OtherMessage());
            collector.Push(new OtherMessage2());
            collector.Push(new OtherMessage3());
            collector.Push(new TestMessage());
            collector.Push(new OtherMessage3());

            executed.Should().Be(2, "only when consumed message is added again, the collector is executed again.");
        }
        
        [Test]
        public void FifthConsumedMessageIsRemovedFromCollector()
        {
            MessageCollector<TestMessage, OtherMessage, OtherMessage2, OtherMessage3, OtherMessage4> collector = 
                new MessageCollector<TestMessage, OtherMessage, OtherMessage2, OtherMessage3, OtherMessage4>(set =>
                {
                    set.MarkAsConsumed(set.Message5);
                });
            collector.Push(new TestMessage());
            collector.Push(new OtherMessage());
            collector.Push(new OtherMessage2());
            collector.Push(new OtherMessage3());
            collector.Push(new OtherMessage4());

            collector.FindSetsForDomain(MessageDomain.DefaultMessageDomain)
                     .Should().BeEmpty("no complete set should exist after a message was consumed.");
        }
        
        
        [Test]
        public void SetIsExecutedEachTimeAMessageChangesWithFifthMessageConsumed()
        {
            int executed = 0;
            MessageCollector<TestMessage, OtherMessage, OtherMessage2, OtherMessage3, OtherMessage4> collector = 
                new MessageCollector<TestMessage, OtherMessage, OtherMessage2, OtherMessage3, OtherMessage4>(set =>
                {
                    executed++;
                    set.MarkAsConsumed(set.Message5);
                });
            collector.Push(new TestMessage());
            collector.Push(new OtherMessage());
            collector.Push(new OtherMessage2());
            collector.Push(new OtherMessage3());
            collector.Push(new OtherMessage4());
            collector.Push(new TestMessage());
            collector.Push(new OtherMessage4());

            executed.Should().Be(2, "only when consumed message is added again, the collector is executed again.");
        }
        
        [Test]
        public void SixthConsumedMessageIsRemovedFromCollector()
        {
            MessageCollector<TestMessage, OtherMessage, OtherMessage2, OtherMessage3, OtherMessage4, OtherMessage5> collector = 
                new MessageCollector<TestMessage, OtherMessage, OtherMessage2, OtherMessage3, OtherMessage4, OtherMessage5>(set =>
                {
                    set.MarkAsConsumed(set.Message6);
                });
            collector.Push(new TestMessage());
            collector.Push(new OtherMessage());
            collector.Push(new OtherMessage2());
            collector.Push(new OtherMessage3());
            collector.Push(new OtherMessage4());
            collector.Push(new OtherMessage5());

            collector.FindSetsForDomain(MessageDomain.DefaultMessageDomain)
                     .Should().BeEmpty("no complete set should exist after a message was consumed.");
        }
        
        
        [Test]
        public void SetIsExecutedEachTimeAMessageChangesWithSixthMessageConsumed()
        {
            int executed = 0;
            MessageCollector<TestMessage, OtherMessage, OtherMessage2, OtherMessage3, OtherMessage4, OtherMessage5> collector = 
                new MessageCollector<TestMessage, OtherMessage, OtherMessage2, OtherMessage3, OtherMessage4, OtherMessage5>(set =>
                {
                    executed++;
                    set.MarkAsConsumed(set.Message6);
                });
            collector.Push(new TestMessage());
            collector.Push(new OtherMessage());
            collector.Push(new OtherMessage2());
            collector.Push(new OtherMessage3());
            collector.Push(new OtherMessage4());
            collector.Push(new OtherMessage5());
            collector.Push(new TestMessage());
            collector.Push(new OtherMessage5());

            executed.Should().Be(2, "only when consumed message is added again, the collector is executed again.");
        }
        
        [Test]
        public void SeventhConsumedMessageIsRemovedFromCollector()
        {
            MessageCollector<TestMessage, OtherMessage, OtherMessage2, OtherMessage3, OtherMessage4, OtherMessage5, OtherMessage6> collector = 
                new MessageCollector<TestMessage, OtherMessage, OtherMessage2, OtherMessage3, OtherMessage4, OtherMessage5, OtherMessage6>(set =>
                {
                    set.MarkAsConsumed(set.Message7);
                });
            collector.Push(new TestMessage());
            collector.Push(new OtherMessage());
            collector.Push(new OtherMessage2());
            collector.Push(new OtherMessage3());
            collector.Push(new OtherMessage4());
            collector.Push(new OtherMessage5());
            collector.Push(new OtherMessage6());

            collector.FindSetsForDomain(MessageDomain.DefaultMessageDomain)
                     .Should().BeEmpty("no complete set should exist after a message was consumed.");
        }
        
        
        [Test]
        public void SetIsExecutedEachTimeAMessageChangesWithSeventhMessageConsumed()
        {
            int executed = 0;
            MessageCollector<TestMessage, OtherMessage, OtherMessage2, OtherMessage3, OtherMessage4, OtherMessage5, OtherMessage6> collector = 
                new MessageCollector<TestMessage, OtherMessage, OtherMessage2, OtherMessage3, OtherMessage4, OtherMessage5, OtherMessage6>(set =>
                {
                    executed++;
                    set.MarkAsConsumed(set.Message7);
                });
            collector.Push(new TestMessage());
            collector.Push(new OtherMessage());
            collector.Push(new OtherMessage2());
            collector.Push(new OtherMessage3());
            collector.Push(new OtherMessage4());
            collector.Push(new OtherMessage5());
            collector.Push(new OtherMessage6());
            collector.Push(new TestMessage());
            collector.Push(new OtherMessage6());

            executed.Should().Be(2, "only when consumed message is added again, the collector is executed again.");
        }

        #endregion

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

        private class OtherMessage2 : Message
        {

            public OtherMessage2()
                : base(Array.Empty<Message>())
            {
            }

            protected override string DataToString()
            {
                return string.Empty;
            }
        }

        private class OtherMessage3 : Message
        {

            public OtherMessage3()
                : base(Array.Empty<Message>())
            {
            }

            protected override string DataToString()
            {
                return string.Empty;
            }
        }

        private class OtherMessage4 : Message
        {

            public OtherMessage4()
                : base(Array.Empty<Message>())
            {
            }

            protected override string DataToString()
            {
                return string.Empty;
            }
        }

        private class OtherMessage5 : Message
        {

            public OtherMessage5()
                : base(Array.Empty<Message>())
            {
            }

            protected override string DataToString()
            {
                return string.Empty;
            }
        }

        private class OtherMessage6 : Message
        {

            public OtherMessage6()
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