#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;
using System.Collections.Generic;
using System.Threading;
using Agents.Net.Tests.Tools.Communities.HelloWorldCommunity.Agents;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace Agents.Net.Tests
{
    public class MessageGateTest
    {
        [Test]
        public void ContinueExecutionIfEndMessageWasSend()
        {
            bool executed = false;
            TestMessage startMessage = new();
            using ManualResetEventSlim resetEvent = new(false);
            MessageGate<TestMessage, OtherMessage> gate = new();
            using Timer timer = new(_ =>
                                    {
                                        executed.Should().BeFalse("The gate should not continue before pushing end message.");
                                        bool checkResult = gate.Check(new OtherMessage(startMessage));
                                        checkResult.Should().BeTrue("Message should be excepted.");
                                        resetEvent.Set();
                                    }, null, 200,
                                    Timeout.Infinite);
            gate.SendAndAwait(startMessage, m => {});
            executed = true;
            resetEvent.Wait();
        }
        
        [Test]
        [Description("This is the only method that tests SendAndContinue as internally SendAndExecute is using send and continue.")]
        public void ContinueActionIsExecutedIfEndMessageWasSend()
        {
            bool executed = false;
            TestMessage startMessage = new();
            using ManualResetEventSlim resetEvent = new(false);
            MessageGate<TestMessage, OtherMessage> gate = new();
            using Timer timer = new(state =>
                                    {
                                        bool checkResult = gate.Check(new OtherMessage(startMessage));
                                        checkResult.Should().BeTrue("Message should be excepted.");
                                        resetEvent.Set();
                                    }, null, 200,
                                    Timeout.Infinite);
            gate.SendAndContinue(startMessage, _ => { }, _ => executed = true);
            resetEvent.Wait();
            executed.Should().BeTrue("After timer gate should immediately execute.");
        }
        
        [Test]
        public void ReturnEndMessageIfSuccess()
        {
            TestMessage startMessage = new();
            MessageGate<TestMessage, OtherMessage> gate = new();
            OtherMessage endMessage = null;
            
            using Timer timer = new(state =>
                                    {
                                        endMessage = new OtherMessage(startMessage);
                                        gate.Check(endMessage);
                                    }, null, 200,
                                    Timeout.Infinite);
            MessageGateResult<OtherMessage> result = gate.SendAndAwait(startMessage, m => {});

            result.Result.Should().Be(MessageGateResultKind.Success);
            result.EndMessage.Should().BeSameAs(endMessage);
        }
        
        [Test]
        public void ReturnExceptionMessageIfUnsuccessful()
        {
            TestMessage startMessage = new();
            MessageGate<TestMessage, OtherMessage> gate = new();
            ExceptionMessage exception = null;
            
            using Timer timer = new(state =>
                                    {
                                        exception = new ExceptionMessage("Error", startMessage, new HelloAgent(Substitute.For<IMessageBoard>()));
                                        gate.Check(exception);
                                    }, null, 200,
                                    Timeout.Infinite);
            MessageGateResult<OtherMessage> result = gate.SendAndAwait(startMessage, m => {});

            result.Result.Should().Be(MessageGateResultKind.Exception);
            result.Exceptions.Should().ContainSingle(message => ReferenceEquals(message, exception));
        }
        
        [Test]
        public void TimeoutIfEndMessageIsOutsideOfMessageDomain()
        {
            TestMessage startMessage = new();
            MessageGate<TestMessage, OtherMessage> gate = new();
            bool executed = false;
            
            using Timer timer = new(state =>
                                    {
                                        OtherMessage endMessage = new(Array.Empty<Message>());
                                        MessageDomain.CreateNewDomainsFor(endMessage);
                                        gate.Check(endMessage);
                                        executed = true;
                                    }, null, 200,
                                    Timeout.Infinite);
            MessageGateResult<OtherMessage> result = gate.SendAndAwait(startMessage, m => {}, 500);

            executed.Should().BeTrue("otherwise there is a timing issue.");
            result.Result.Should().Be(MessageGateResultKind.Timeout);
        }
        
        [Test]
        public void TimeoutIfExceptionMessageIsOutsideOfMessageDomain()
        {
            TestMessage startMessage = new();
            MessageGate<TestMessage, OtherMessage> gate = new();
            bool executed = false;
            
            using Timer timer = new(state =>
                                    {
                                        ExceptionMessage exception = new("Error", Array.Empty<Message>(), new HelloAgent(Substitute.For<IMessageBoard>()));
                                        MessageDomain.CreateNewDomainsFor(exception);
                                        gate.Check(exception);
                                        executed = true;
                                    }, null, 200,
                                    Timeout.Infinite);
            MessageGateResult<OtherMessage> result = gate.SendAndAwait(startMessage, m => {}, 500);

            executed.Should().BeTrue("otherwise there is a timing issue.");
            result.Result.Should().Be(MessageGateResultKind.Timeout);
        }
        
        [Test]
        public void CancelGateWithCancelToken()
        {
            using CancellationTokenSource source = new(500);
            TestMessage startMessage = new();
            MessageGate<TestMessage, OtherMessage> gate = new();
            
            MessageGateResult<OtherMessage> result = gate.SendAndAwait(startMessage, m => {}, cancellationToken:source.Token);

            source.IsCancellationRequested.Should().BeTrue("something went wrong otherwise.");
            result.Result.Should().Be(MessageGateResultKind.Canceled);
        }
        
        [Test]
        public void DoNotExceptForeignMessages()
        {
            MessageGate<TestMessage, OtherMessage> gate = new();
            gate.SendAndContinue(new TestMessage(), _=> {}, _=>{});

            bool checkResult = gate.Check(new ForeignMessage());
            checkResult.Should().BeFalse("Foreign messages should not be excepted.");
        }
        
        [Test]
        public void DoNotExceptMessagesWithoutAwaitingExecution()
        {
            MessageGate<TestMessage, OtherMessage> gate = new();

            bool checkResult = gate.Check(new OtherMessage(ArraySegment<Message>.Empty));
            checkResult.Should().BeFalse("Foreign messages should not be excepted.");
        }
        
        private class OtherMessage : Message
        {

            protected override string DataToString()
            {
                return string.Empty;
            }

            public OtherMessage(Message predecessorMessage, string name = null)
                : base(predecessorMessage, name)
            {
            }

            public OtherMessage(IEnumerable<Message> predecessorMessages, string name = null)
                : base(predecessorMessages, name)
            {
            }
        }
        
        public class ForeignMessage : Message
        {

            public ForeignMessage()
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