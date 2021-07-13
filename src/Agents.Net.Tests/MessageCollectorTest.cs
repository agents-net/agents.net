#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;
using System.Collections.Generic;
using System.Threading;
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
                new(_ =>
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
                new(_ =>
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
            int executed = 0;
            MessageCollector<TestMessage, OtherMessage> collector = 
                new(_ =>
                {
                    executed++;
                });
            collector.Push(new TestMessage());
            collector.Push(new OtherMessage());
            collector.Push(new OtherMessage());

            executed.Should().Be(2, "the test message should not have been forgotten.");
        }
        
        [Test]
        public void ConsumedMessageIsRemovedFromCollector()
        {
            int executed = 0;
            MessageCollector<TestMessage, OtherMessage> collector = 
                new(set =>
                {
                    executed++;
                    set.MarkAsConsumed(set.Message2);
                });
            collector.Push(new TestMessage());
            collector.Push(new OtherMessage());
            collector.Push(new TestMessage());

            executed.Should().Be(1, "other message should have been eliminated.");
        }
        
        [Test]
        public void PushAndContinueStoresMessages()
        {
            int executed = 0;
            MessageCollector<TestMessage, OtherMessage> collector = 
                new(_ => executed++);
            collector.Push(new TestMessage());
            collector.PushAndContinue(new OtherMessage(), _ =>
            {
            });
            collector.Push(new TestMessage());

            executed.Should().Be(2, "push and execute should store the other message.");
        }
        
        [Test]
        public void CancelPushAndContinueAndAfterwardsPushDoesNotExecuteAction()
        {
            bool executed = false;
            MessageCollector<TestMessage, OtherMessage> collector = new();
            using CancellationTokenSource source = new();
            using ManualResetEventSlim resetEvent = new();
            using Timer timer = new(_ =>
                                    {
                                        source.Cancel();
                                        resetEvent.Set();
                                    }, null, 200,
                                    Timeout.Infinite);
            collector.PushAndContinue(new OtherMessage(), _ =>
            {
                executed = true;
            }, source.Token);
            
            resetEvent.Wait();
            collector.Push(new TestMessage());
            
            executed.Should().BeFalse("execution was canceled.");
        }
        
        [Test]
        public void ConsumedMessageIsRemovedFromCollectorUsingPushAndContinue()
        {
            int executed = 0;
            MessageCollector<TestMessage, OtherMessage> collector = 
                new(_ => executed++);
            collector.Push(new TestMessage());
            collector.PushAndContinue(new OtherMessage(), set =>
            {
                set.MarkAsConsumed(set.Message2);
            });
            collector.Push(new TestMessage());

            executed.Should().Be(1, "other message should have been eliminated.");
        }
        
        [Test]
        public void SetIsExecutedEachTimeAMessageChangesWithConsumedMessage()
        {
            int executed = 0;
            MessageCollector<TestMessage, OtherMessage> collector = 
                new(set =>
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
        
        [Test]
        public void ExecutePushedMessageIfSetIsFull()
        {
            bool executed = false;
            MessageCollector<TestMessage, OtherMessage> collector = new();
            collector.Push(new TestMessage());
            collector.PushAndContinue(new OtherMessage(), _ =>
            {
                executed = true;
            });

            executed.Should().BeTrue("this set should have been executed immediately.");
        }
        
        [Test]
        public void ExecutePushedMessageIfSetIsFullWithDecorator()
        {
            bool executed = false;
            MessageCollector<TestMessage, OtherMessage> collector = new();
            collector.Push(new TestMessage());
            collector.PushAndContinue(OtherDecorator.Decorate(new OtherMessage()), _ =>
            {
                executed = true;
            });

            executed.Should().BeTrue("this set should have been executed immediately.");
        }
        
        [Test]
        public void ExecutePushedMessageIfSetIsFilledLater()
        {
            bool executed = false;
            MessageCollector<TestMessage, OtherMessage> collector = new();
            using ManualResetEventSlim resetEvent = new();
            using Timer timer = new(_ =>
                                    {
                                        executed.Should().BeFalse("The set should not execute before pushing second message.");
                                        collector.Push(new TestMessage());
                                        resetEvent.Set();
                                    }, null, 200,
                                    Timeout.Infinite);
            collector.PushAndContinue(new OtherMessage(), _ =>
            {
                executed = true;
            });

            resetEvent.Wait();
            executed.Should().BeTrue("this set should have been executed after the timer goes of.");
        }
        
        [Test]
        public void ExecutePushedMessageOnlyOnce()
        {
            int executed = 0;
            MessageCollector<TestMessage, OtherMessage> collector = new();
            collector.Push(new TestMessage());
            collector.PushAndContinue(new OtherMessage(), _ =>
            {
                executed++;
            });
            collector.Push(new TestMessage());

            executed.Should().Be(1, "the executed action should not have been executed twice.");
        }
        
        [Test]
        public void ExecuteOriginalActionToo()
        {
            bool executed = false;
            MessageCollector<TestMessage, OtherMessage> collector = new(set => executed = true);
            collector.Push(new TestMessage());
            collector.PushAndContinue(new OtherMessage(), _ => { });

            executed.Should().BeTrue("the original action should have been executed.");
        }

        #region Mark as consumable for higher order collectors
        
        
        [Test]
        public void ThirdPushedMessageIsExecuted()
        {
            bool executed = false;
            MessageCollector<TestMessage, OtherMessage, OtherMessage2> collector = 
                new(_ => executed = true);
            collector.Push(new TestMessage());
            collector.Push(new OtherMessage());
            collector.Push(new OtherMessage2());

            executed.Should().BeTrue("the set was completed");
        }
        
        
        [Test]
        public void SetIsExecutedEachTimeAMessageChangesWithThirdMessageConsumed()
        {
            int executed = 0;
            MessageCollector<TestMessage, OtherMessage, OtherMessage2> collector = 
                new(set =>
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
            bool executed = false;
            MessageCollector<TestMessage, OtherMessage, OtherMessage2, OtherMessage3> collector = 
                new(_ => executed = true);
            collector.Push(new TestMessage());
            collector.Push(new OtherMessage());
            collector.Push(new OtherMessage2());
            collector.Push(new OtherMessage3());

            executed.Should().BeTrue("the set was completed");
        }
        
        
        [Test]
        public void SetIsExecutedEachTimeAMessageChangesWithFourthMessageConsumed()
        {
            int executed = 0;
            MessageCollector<TestMessage, OtherMessage, OtherMessage2, OtherMessage3> collector = 
                new(set =>
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
            bool executed = false;
            MessageCollector<TestMessage, OtherMessage, OtherMessage2, OtherMessage3, OtherMessage4> collector = 
                new(_ => executed = true);
            collector.Push(new TestMessage());
            collector.Push(new OtherMessage());
            collector.Push(new OtherMessage2());
            collector.Push(new OtherMessage3());
            collector.Push(new OtherMessage4());

            executed.Should().BeTrue("the set was completed");
        }
        
        
        [Test]
        public void SetIsExecutedEachTimeAMessageChangesWithFifthMessageConsumed()
        {
            int executed = 0;
            MessageCollector<TestMessage, OtherMessage, OtherMessage2, OtherMessage3, OtherMessage4> collector = 
                new(set =>
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
            bool executed = false;
            MessageCollector<TestMessage, OtherMessage, OtherMessage2, OtherMessage3, OtherMessage4, OtherMessage5> collector = 
                new(_ => executed = true);
            collector.Push(new TestMessage());
            collector.Push(new OtherMessage());
            collector.Push(new OtherMessage2());
            collector.Push(new OtherMessage3());
            collector.Push(new OtherMessage4());
            collector.Push(new OtherMessage5());

            executed.Should().BeTrue("the set was completed");
        }
        
        
        [Test]
        public void SetIsExecutedEachTimeAMessageChangesWithSixthMessageConsumed()
        {
            int executed = 0;
            MessageCollector<TestMessage, OtherMessage, OtherMessage2, OtherMessage3, OtherMessage4, OtherMessage5> collector = 
                new(set =>
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
            bool executed = false;
            MessageCollector<TestMessage, OtherMessage, OtherMessage2, OtherMessage3, OtherMessage4, OtherMessage5, OtherMessage6> collector = 
                new(_ => executed = true);
            collector.Push(new TestMessage());
            collector.Push(new OtherMessage());
            collector.Push(new OtherMessage2());
            collector.Push(new OtherMessage3());
            collector.Push(new OtherMessage4());
            collector.Push(new OtherMessage5());
            collector.Push(new OtherMessage6());

            executed.Should().BeTrue("the set was completed");
        }
        
        
        [Test]
        public void SetIsExecutedEachTimeAMessageChangesWithSeventhMessageConsumed()
        {
            int executed = 0;
            MessageCollector<TestMessage, OtherMessage, OtherMessage2, OtherMessage3, OtherMessage4, OtherMessage5, OtherMessage6> collector = 
                new(set =>
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

        private class OtherDecorator : MessageDecorator
        {
            private OtherDecorator(Message decoratedMessage, IEnumerable<Message> additionalPredecessors = null)
                : base(decoratedMessage, additionalPredecessors)
            {
            }

            public static OtherDecorator Decorate(OtherMessage message)
            {
                return new(message);
            }

            protected override string DataToString()
            {
                return string.Empty;
            }
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