#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Agents.Net.Tests
{
    public class MessageAggregatorTest
    {
        [Test]
        public void AggregateSingleMessage()
        {
            bool executed = false;
            MessageGate<OtherMessage, TestMessage> aggregator = new();

            OtherMessage startMessage = new();
            aggregator.SendAndContinue(new []{startMessage},
                message => message.Should().BeSameAs(startMessage),
                result =>
                {
                    result.Result.Should().Be(MessageGateResultKind.Success);
                    executed = true;
                });
            aggregator.Check(new TestMessage(startMessage));

            executed.Should().BeTrue("A message in the default domain should be executed immediately.");
        }

        [Test]
        public void AggregateMultipleMessagesInDifferentDomains()
        {
            bool executed = false;
            MessageGate<OtherMessage, TestMessage> aggregator = new();

            OtherMessage[] startMessages = {new(), new(), new()};
            TestMessage[] messages = null;
            aggregator.SendAndContinue(startMessages, 
                                       message => startMessages.Should().Contain((OtherMessage) message),
                                       result =>
                                       {
                                           result.Result.Should().Be(MessageGateResultKind.Success);
                                           result.EndMessages.Should().BeEquivalentTo(messages);
                                           executed = true;
                                       });
            messages = startMessages.Select(m => new TestMessage(m)).ToArray();
            foreach (TestMessage message in messages)
            {
                aggregator.Check(message);
            }

            executed.Should().BeTrue("all messages were added to the aggregator.");
        }

        [Test]
        public void DontExecuteMultipleMessagesIfOneIsMissing()
        {
            bool executed = false;
            MessageGate<OtherMessage, TestMessage> aggregator = new();

            OtherMessage[] startMessages = {new(), new(), new()};
            aggregator.SendAndContinue(startMessages, 
                                       message => startMessages.Should().Contain((OtherMessage) message),
                                       result =>
                                       {
                                           executed = true;
                                       });
            TestMessage[] messages = startMessages.Select(m => new TestMessage(m)).ToArray();
            aggregator.Check(messages[0]);
            aggregator.Check(messages[1]);

            executed.Should().BeFalse("not all messages were added to the aggregator.");
        }

        [Test]
        public void TerminateMessageDomainAutomatically()
        {
            MessageGate<OtherMessage, TestMessage> aggregator = new();

            OtherMessage[] startMessages = {new(), new(), new()};
            aggregator.SendAndContinue(startMessages, 
                                       message => startMessages.Should().Contain((OtherMessage) message),
                                       _ => { });
            TestMessage[] messages = startMessages.Select(m => new TestMessage(m)).ToArray();
            foreach (TestMessage message in messages)
            {
                aggregator.Check(message);
            }

            foreach (TestMessage message in messages)
            {
                message.MessageDomain.IsTerminated.Should().BeTrue("message domain should have been terminated.");
            }
        }

        [Test]
        public void DoNotTerminateDefaultDomain()
        {
            MessageGate<OtherMessage, TestMessage> aggregator =new();

            OtherMessage startMessage = new();
            aggregator.SendAndContinue(new []{startMessage},
                                       message => message.Should().BeSameAs(startMessage),
                                       result =>
                                       {
                                           result.Result.Should().Be(MessageGateResultKind.Success);
                                       });
            aggregator.Check(new TestMessage(startMessage));

            MessageDomain.DefaultMessageDomain.IsTerminated.Should().BeFalse("the default message domain should never be terminated.");
        }

        [Test]
        public void AggregateExceptionMessages()
        {
            bool executed = false;
            MessageGate<OtherMessage, TestMessage> aggregator = new();

            OtherMessage[] startMessages = {new(), new(), new()};
            ExceptionMessage exception = null;
            aggregator.SendAndContinue(startMessages, 
                                       message => startMessages.Should().Contain((OtherMessage) message),
                                       result =>
                                       {
                                           result.Result.Should().Be(MessageGateResultKind.Exception);
                                           result.Exceptions.Should().ContainSingle();
                                           result.Exceptions.Should().Contain(exception);
                                           executed = true;
                                       });
            TestMessage[] messages = startMessages.Select(m => new TestMessage(m)).ToArray();
            exception = new("Whatever", startMessages[2], null);
            aggregator.Check(messages[0]);
            aggregator.Check(messages[1]);
            aggregator.Check(exception);

            executed.Should().BeTrue("not all messages were added to the aggregator.");
        }
        
        [Test]
        public void TimeoutAggregation()
        {
            bool executed = false;
            MessageGate<OtherMessage, TestMessage> aggregator = new();

            OtherMessage startMessage = new();
            aggregator.SendAndContinue(new []{startMessage},
                                       _ => {},
                                       result =>
                                       {
                                           result.Result.Should().Be(MessageGateResultKind.Timeout);
                                           executed = true;
                                       }, 200);
            Thread.Sleep(500);

            executed.Should().BeTrue("A message in the default domain should be executed immediately.");
        }
        
        [Test]
        public void CancelAggregation()
        {
            bool executed = false;
            MessageGate<OtherMessage, TestMessage> aggregator = new();

            using CancellationTokenSource tokenSource = new();
            using ManualResetEventSlim resetEvent = new();
            using Timer timer = new(_ =>
            {
                tokenSource.Cancel();
                resetEvent.Set();
            }, null, 200,Timeout.Infinite);
            OtherMessage startMessage = new();
            aggregator.SendAndContinue(new []{startMessage},
                                       _ => {},
                                       result =>
                                       {
                                           result.Result.Should().Be(MessageGateResultKind.Canceled);
                                           executed = true;
                                       }, cancellationToken: tokenSource.Token);
            resetEvent.Wait();

            executed.Should().BeTrue("A message in the default domain should be executed immediately.");
        }

        [Test]
        public void AutoAggregationMessage()
        {
            bool send = false;
            MessageGate<OtherMessage, TestMessage> aggregator = new();

            OtherMessage[] startMessages = {new(), new(), new()};
            TestMessage[] messages = null;
            aggregator.SendAndAggregate(startMessages,
                                        message =>
                                        {
                                            if (message is MessagesAggregated<TestMessage> aggregated)
                                            {
                                                send = true;
                                                aggregated.Result.Result.Should().Be(MessageGateResultKind.Success);
                                                aggregated.Result.EndMessages.Should()
                                                          .BeEquivalentTo(messages);
                                            }
                                        });
            messages = startMessages.Select(m => new TestMessage(m)).ToArray();
            foreach (TestMessage message in messages)
            {
                aggregator.Check(message);
            }

            send.Should().BeTrue("all messages were added to the aggregator.");
        }

        [Test]
        public void AutoExceptionMessage()
        {
            bool send = false;
            MessageGate<OtherMessage, TestMessage> aggregator = new();

            OtherMessage[] startMessages = {new(), new(), new()};
            ExceptionMessage exception = null;
            aggregator.SendAndAggregate(startMessages,
                                        message =>
                                        {
                                            if (message is MessagesAggregated<TestMessage> aggregated)
                                            {
                                                send = true;
                                                aggregated.Result.Result.Should().Be(MessageGateResultKind.Exception);
                                                aggregated.Result.Exceptions.Should().ContainSingle();
                                                aggregated.Result.Exceptions.Should().Contain(exception);
                                            }
                                        });
            TestMessage[] messages = startMessages.Select(m => new TestMessage(m)).ToArray();
            exception = new("Whatever", startMessages[2], null);
            aggregator.Check(messages[0]);
            aggregator.Check(messages[1]);
            aggregator.Check(exception);

            send.Should().BeTrue("not all messages were added to the aggregator.");
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
