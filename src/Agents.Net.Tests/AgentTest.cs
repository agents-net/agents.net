#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;
using System.Collections.Generic;
using System.Text;
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
        public void OnMessagesPublishesAllMessagesAndCreatesDomains()
        {
            TestMessage message1 = new TestMessage();
            TestMessage message2 = new TestMessage();
            testAgent.OnMessages(false, message1, message2);

            messageBoard.Received().Publish(message1);
            messageBoard.Received().Publish(message2);
            Assert.AreNotEqual(message1.MessageDomain,message2.MessageDomain, "MessageDomains not created.");
        }

        [Test]
        public void OnMessagesPublishesCreatedMessageIfRequested()
        {
            TestMessage message1 = new TestMessage();
            TestMessage message2 = new TestMessage();
            testAgent.OnMessages(true, message1, message2);

            messageBoard.Received().Publish(Arg.Is<Message>(m => m is MessageDomainsCreatedMessage));
        }

        [Test]
        public void OnMessagesDoesNotPublishCreatedMessageIfNotRequested()
        {
            TestMessage message1 = new TestMessage();
            TestMessage message2 = new TestMessage();
            testAgent.OnMessages(false, message1, message2);

            messageBoard.DidNotReceive().Publish(Arg.Is<Message>(m => m is MessageDomainsCreatedMessage));
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

        private class TestAgent : Agent
        {
            public TestAgent(IMessageBoard messageBoard) : base(messageBoard)
            {
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

            public void OnMessages(bool publishCreated, params Message[] messages)
            {
                base.OnMessages(messages, publishCreated);
            }
        }
    }
}
