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
            TestMessage startMessage = new TestMessage();
            using ManualResetEventSlim resetEvent = new ManualResetEventSlim(false);
            MessageGate<TestMessage, OtherMessage> gate = new MessageGate<TestMessage, OtherMessage>();
            using Timer timer = new Timer(state =>
                                          {
                                              executed.Should().BeFalse("The gate should not continue before pushing end message.");
                                              gate.Check(new OtherMessage(startMessage));
                                              resetEvent.Set();
                                          }, null, 200,
                                          Timeout.Infinite);
            gate.SendAndAwait(startMessage);
            executed = true;
            resetEvent.Wait();
        }
        
        [Test]
        public void ReturnEndMessageIfSuccess()
        {
            TestMessage startMessage = new TestMessage();
            MessageGate<TestMessage, OtherMessage> gate = new MessageGate<TestMessage, OtherMessage>();
            OtherMessage endMessage = null;
            
            using Timer timer = new Timer(state =>
                                          {
                                              endMessage = new OtherMessage(startMessage);
                                              gate.Check(endMessage);
                                          }, null, 200,
                                          Timeout.Infinite);
            MessageGateResult<OtherMessage> result = gate.SendAndAwait(startMessage);

            result.Result.Should().Be(MessageGateResultKind.Success);
            result.EndMessage.Should().BeSameAs(endMessage);
        }
        
        [Test]
        public void ReturnExceptionMessageIfUnsuccessful()
        {
            TestMessage startMessage = new TestMessage();
            MessageGate<TestMessage, OtherMessage> gate = new MessageGate<TestMessage, OtherMessage>();
            ExceptionMessage exception = null;
            
            using Timer timer = new Timer(state =>
                                          {
                                              exception = new ExceptionMessage("Error", startMessage, new HelloAgent(Substitute.For<IMessageBoard>()));
                                              gate.Check(exception);
                                          }, null, 200,
                                          Timeout.Infinite);
            MessageGateResult<OtherMessage> result = gate.SendAndAwait(startMessage);

            result.Result.Should().Be(MessageGateResultKind.Exception);
            result.Exceptions.Should().ContainSingle(message => ReferenceEquals(message, exception));
        }
        
        [Test]
        public void TimeoutIfEndMessageIsOutsideOfMessageDomain()
        {
            TestMessage startMessage = new TestMessage();
            MessageGate<TestMessage, OtherMessage> gate = new MessageGate<TestMessage, OtherMessage>();
            bool executed = false;
            
            using Timer timer = new Timer(state =>
                                          {
                                              OtherMessage endMessage = new OtherMessage(Array.Empty<Message>());
                                              gate.Check(endMessage);
                                              executed = true;
                                          }, null, 200,
                                          Timeout.Infinite);
            MessageGateResult<OtherMessage> result = gate.SendAndAwait(startMessage, 500);

            executed.Should().BeTrue("otherwise there is a timing issue.");
            result.Result.Should().Be(MessageGateResultKind.Timeout);
        }
        
        [Test]
        public void TimeoutIfExceptionMessageIsOutsideOfMessageDomain()
        {
            TestMessage startMessage = new TestMessage();
            MessageGate<TestMessage, OtherMessage> gate = new MessageGate<TestMessage, OtherMessage>();
            bool executed = false;
            
            using Timer timer = new Timer(state =>
                                          {
                                              ExceptionMessage exception = new ExceptionMessage("Error", Array.Empty<Message>(), new HelloAgent(Substitute.For<IMessageBoard>()));
                                              gate.Check(exception);
                                              executed = true;
                                          }, null, 200,
                                          Timeout.Infinite);
            MessageGateResult<OtherMessage> result = gate.SendAndAwait(startMessage, 500);

            executed.Should().BeTrue("otherwise there is a timing issue.");
            result.Result.Should().Be(MessageGateResultKind.Timeout);
        }
        
        [Test]
        public void CancelGateWithCancelToken()
        {
            using CancellationTokenSource source = new CancellationTokenSource(500);
            TestMessage startMessage = new TestMessage();
            MessageGate<TestMessage, OtherMessage> gate = new MessageGate<TestMessage, OtherMessage>();
            
            MessageGateResult<OtherMessage> result = gate.SendAndAwait(startMessage, cancellationToken:source.Token);

            source.IsCancellationRequested.Should().BeTrue("something went wrong otherwise.");
            result.Result.Should().Be(MessageGateResultKind.Canceled);
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
    }
}