#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace Agents.Net.Tests
{
    public class AgentTest
    {
        private TestAgent testAgent;
        private IMessageBoard messageBoard;

        [SetUp]
        public void Setup()
        {
            messageBoard = Substitute.For<IMessageBoard>();
            testAgent = new TestAgent(messageBoard);
        }

        [Test]
        public void ExecuteExecutesExecuteCore()
        {
            TestMessage message = new TestMessage();
            testAgent.Execute(message);

            Assert.AreEqual(message, testAgent.LastExecutedMessage, "Message should have been executed.");
        }

        [Test]
        public void OnMessagePublishesMessageToMessageBoar()
        {
            TestMessage message = new TestMessage();
            testAgent.OnMessage(message);

            messageBoard.Received().Publish(message);
        }

        [Test]
        public void ExceptionThrowsExceptionMessage()
        {
            ArgumentException someException = new ArgumentException("foo");
            messageBoard.When(m => m.Publish(Arg.Any<ExceptionMessage>()))
                        .Do(info =>
                         {
                             Assert.AreEqual(someException, ((ExceptionMessage)info.Arg<Message>()).ExceptionInfo.SourceException);
                         });
            testAgent.ExecuteException = someException;

            TestMessage message = new TestMessage();
            testAgent.Execute(message);

            messageBoard.Received().Publish(Arg.Any<ExceptionMessage>());
        }

        [Test]
        public void DisposeAllDisposables()
        {
            TestDisposable disposable1 = new TestDisposable();
            TestDisposable disposable2 = new TestDisposable();
            using (TestAgent agent = new TestAgent(Substitute.For<IMessageBoard>()))
            {
                agent.MarkForDispose(disposable1);
                agent.MarkForDispose(disposable2);
            }

            disposable1.IsDisposed.Should().BeTrue("all disposables should be disposed on dispose.");
            disposable2.IsDisposed.Should().BeTrue("all disposables should be disposed on dispose.");
        }

        private class TestDisposable : IDisposable
        {
            public bool IsDisposed { get; private set; }
            public void Dispose()
            {
                IsDisposed = true;
            }
        } 

        private class TestAgent : Agent
        {
            public TestAgent(IMessageBoard messageBoard) : base(messageBoard)
            {
            }

            public void MarkForDispose(IDisposable disposable)
            {
                AddDisposable(disposable);
            }

            public Message LastExecutedMessage { get; private set; }

            public Exception ExecuteException { get; set; }

            protected override void ExecuteCore(Message messageData)
            {
                LastExecutedMessage = messageData;
                if (ExecuteException != null)
                {
                    throw ExecuteException;
                }
            }

            public new void OnMessage(Message message)
            {
                base.OnMessage(message);
            }
        }
    }
}
