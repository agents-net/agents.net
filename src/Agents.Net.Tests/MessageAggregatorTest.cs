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
            MessageAggregator<TestMessage> aggregator = new();

            OtherMessage startMessage = new();
            aggregator.SendAndContinue(new []{startMessage},
                message => message.Should().BeSameAs(startMessage),
                result =>
                {
                    result.Should().Be(WaitResultKind.Success);
                    executed = true;
                });
            aggregator.Aggregate(new TestMessage());

            executed.Should().BeTrue("A message in the default domain should be executed immediately.");
        }

        [Test]
        public void AggregateMultipleMessagesInDifferentDomains()
        {
            bool executed = false;
            MessageAggregator<TestMessage> aggregator = new();

            OtherMessage[] startMessages = {new(), new(), new()};
            MessageDomain.CreateNewDomainsFor(startMessages);
            TestMessage[] messages = startMessages.Select(m => new TestMessage(m)).ToArray();
            aggregator.SendAndContinue(startMessages, 
                                       message => startMessages.Should().Contain((OtherMessage) message),
                                       result =>
                                       {
                                           result.Result.Should().Be(WaitResultKind.Success);
                                           result.EndMessages.Should().BeEquivalentTo(messages);
                                           executed = true;
                                       });
            foreach (TestMessage message in messages)
            {
                aggregator.Aggregate(message);
            }

            executed.Should().BeTrue("all messages were added to the aggregator.");
        }

        [Test]
        public void DontExecuteMultipleMessagesIfOneIsMissing()
        {
            bool executed = false;
            MessageAggregator<TestMessage> aggregator = new();

            OtherMessage[] startMessages = {new(), new(), new()};
            MessageDomain.CreateNewDomainsFor(startMessages);
            TestMessage[] messages = startMessages.Select(m => new TestMessage(m)).ToArray();
            aggregator.SendAndContinue(startMessages, 
                                       message => startMessages.Should().Contain((OtherMessage) message),
                                       result =>
                                       {
                                           executed = true;
                                       });
            aggregator.Aggregate(messages[0]);
            aggregator.Aggregate(messages[1]);

            executed.Should().BeFalse("not all messages were added to the aggregator.");
        }

        [Test]
        public void TerminateMessageDomainAutomatically()
        {
            MessageAggregator<TestMessage> aggregator = new();

            OtherMessage[] startMessages = {new(), new(), new()};
            MessageDomain.CreateNewDomainsFor(startMessages);
            TestMessage[] messages = startMessages.Select(m => new TestMessage(m)).ToArray();
            aggregator.SendAndContinue(startMessages, 
                                       message => startMessages.Should().Contain((OtherMessage) message),
                                       _ => { });
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
        public void DoNotTerminateDefaultDomain()
        {
            MessageAggregator<TestMessage> aggregator =new();

            OtherMessage startMessage = new();
            aggregator.SendAndContinue(new []{startMessage},
                                       message => message.Should().BeSameAs(startMessage),
                                       result =>
                                       {
                                           result.Should().Be(WaitResultKind.Success);
                                       });
            aggregator.Aggregate(new TestMessage());

            MessageDomain.DefaultMessageDomain.IsTerminated.Should().BeFalse("the default message domain should never be terminated.");
        }

        [Test]
        public void AggregateExceptionMessages()
        {
            bool executed = false;
            MessageAggregator<TestMessage> aggregator = new();

            OtherMessage[] startMessages = {new(), new(), new()};
            MessageDomain.CreateNewDomainsFor(startMessages);
            TestMessage[] messages = startMessages.Select(m => new TestMessage(m)).ToArray();
            ExceptionMessage exception = new("Whatever", startMessages[2], null);
            aggregator.SendAndContinue(startMessages, 
                                       message => startMessages.Should().Contain((OtherMessage) message),
                                       result =>
                                       {
                                           result.Result.Should().Be(WaitResultKind.Exception);
                                           result.Exceptions.Should().ContainSingle();
                                           result.Exceptions.Should().Contain(exception);
                                           executed = true;
                                       });
            aggregator.Aggregate(messages[0]);
            aggregator.Aggregate(messages[1]);
            aggregator.Aggregate(exception);

            executed.Should().BeTrue("not all messages were added to the aggregator.");
        }
        
        [Test]
        public void TimeoutAggregation()
        {
            bool executed = false;
            MessageAggregator<TestMessage> aggregator = new();

            OtherMessage startMessage = new();
            aggregator.SendAndContinue(new []{startMessage},
                                       _ => {},
                                       result =>
                                       {
                                           result.Should().Be(WaitResultKind.Timeout);
                                           executed = true;
                                       }, 200);
            Thread.Sleep(500);

            executed.Should().BeTrue("A message in the default domain should be executed immediately.");
        }
        
        [Test]
        public void CancelAggregation()
        {
            bool executed = false;
            MessageAggregator<TestMessage> aggregator = new();

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
                                           result.Should().Be(WaitResultKind.Canceled);
                                           executed = true;
                                       }, cancellationToken: tokenSource.Token);
            resetEvent.Wait();

            executed.Should().BeTrue("A message in the default domain should be executed immediately.");
        }

        [Test]
        public void AutoAggregationMessage()
        {
            bool send = false;
            MessageAggregator<TestMessage> aggregator = new();

            OtherMessage[] startMessages = {new(), new(), new()};
            MessageDomain.CreateNewDomainsFor(startMessages);
            TestMessage[] messages = startMessages.Select(m => new TestMessage(m)).ToArray();
            aggregator.SendAndAggregate(startMessages,
                                        message =>
                                        {
                                            if (message is MessagesAggregated<TestMessage> aggregated)
                                            {
                                                send = true;
                                                aggregated.AggregatorResult.EndMessages.Should()
                                                          .BeEquivalentTo(messages);
                                            }
                                        });
            foreach (TestMessage message in messages)
            {
                aggregator.Aggregate(message);
            }

            send.Should().BeTrue("all messages were added to the aggregator.");
        }

        [Test]
        public void AutoExceptionMessage()
        {
            bool send = false;
            MessageAggregator<TestMessage> aggregator = new();

            OtherMessage[] startMessages = {new(), new(), new()};
            MessageDomain.CreateNewDomainsFor(startMessages);
            TestMessage[] messages = startMessages.Select(m => new TestMessage(m)).ToArray();
            ExceptionMessage exception = new("Whatever", startMessages[2], null);
            aggregator.SendAndAggregate(startMessages,
                                        message =>
                                        {
                                            if (message is AggregatedExceptionMessage exceptionMessage)
                                            {
                                                send = true;
                                                exceptionMessage.Exceptions.Should().ContainSingle();
                                                exceptionMessage.Exceptions.Should().Contain(exception);
                                            }
                                        });
            aggregator.Aggregate(messages[0]);
            aggregator.Aggregate(messages[1]);
            aggregator.Aggregate(exception);

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
