#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Agents.Net.Tests
{
    /// <summary>
    /// This class simulates the interactions between the message board and the messages regarding disposing the messages.
    /// </summary>
    /// <remarks>
    /// The message board uses SetUserCount(x) to set the number of agents that use the message.
    /// After each agent execution the message is called with Used() once.
    /// Once no more uses are there, the message is disposed.
    /// </remarks>
    public class DisposeTests
    {
        [Test]
        public void MessageIsDisposedAfterOnlyUse()
        {
            DisposableMessage message = new();
            message.SetUserCount(1);
            message.Used();

            message.IsDisposed.Should().BeTrue("no one blocked the dispose action.");
        }
        
        [Test]
        public void MessageNotDisposedIfHeldByDelay()
        {
            DisposableMessage message = new();
            message.SetUserCount(1);
            message.DelayDispose();
            message.Used();

            message.IsDisposed.Should().BeFalse("the delay blocked the dispose.");
        }
        
        [Test]
        public void MessageIsDisposedIfDelayIsReleased()
        {
            DisposableMessage message = new();
            message.SetUserCount(1);
            IDisposable token = message.DelayDispose();
            message.Used();
            token.Dispose();

            message.IsDisposed.Should().BeTrue("the delay blocked the dispose.");
        }
        
        [Test]
        public void MessageNotDisposedIfHeldByCollector()
        {
            MessageCollector<TestMessage, DisposableMessage> collector = new();
            DisposableMessage message = new();
            message.SetUserCount(1);
            collector.Push(message);
            message.Used();

            message.IsDisposed.Should().BeFalse("the collector blocked the dispose.");
        }
        
        [Test]
        public void MessageIsDisposedIfCollectorIsExecuted()
        {
            MessageCollector<TestMessage, DisposableMessage> collector = new(set =>
            {
                set.Message2.IsDisposed.Should().BeFalse("I am still using it.");
            });
            DisposableMessage message = new();
            message.SetUserCount(1);
            collector.Push(message);
            message.Used();
            collector.Push(new TestMessage());

            message.IsDisposed.Should().BeTrue("the collector is finished.");
        }
        
        [Test]
        public void MessageIsDisposedIfRemovedFromCollectorByDomainTermination()
        {
            MessageCollector<TestMessage, DisposableMessage> collector = new();
            DisposableMessage message = new();
            MessageDomain.CreateNewDomainsFor(message);
            message.SetUserCount(1);
            collector.Push(message);
            message.Used();
            MessageDomain.TerminateDomainsOf(message);

            message.IsDisposed.Should().BeTrue("the it was removed from the collector.");
        }
        
        [Test]
        public void MessageIsDisposedAfterExecutingMessageGate()
        {
            MessageGate<TestMessage, DisposableMessage> gate = new();
            TestMessage startMessage = new();
            gate.SendAndContinue(startMessage, _=>{}, result =>
            {
                result.EndMessage.IsDisposed.Should().BeFalse("I am using it");
            });
            DisposableMessage message = new(startMessage);
            message.SetUserCount(1);
            gate.Check(message);
            message.Used();

            message.IsDisposed.Should().BeTrue("the gate was executed.");
        }
        
        [Test]
        public void MessageNotDisposedIfHeldByAggregator()
        {
            MessageGate<TestMessage, DisposableMessage> aggregator = new();
            TestMessage startMessage = new();
            TestMessage startMessage2 = new();
            aggregator.SendAndAggregate(new []{startMessage, startMessage2}, _ => { });
            DisposableMessage message = new(startMessage);
            message.SetUserCount(1);
            aggregator.Check(message);
            message.Used();

            message.IsDisposed.Should().BeFalse("the aggregator blocked the dispose.");
        }
        
        [Test]
        public void MessageIsDisposedIfAggregatorIsExecuted()
        {
            MessageGate<TestMessage, DisposableMessage> aggregator = new();
            TestMessage startMessage = new();
            TestMessage startMessage2 = new();
            aggregator.SendAndContinue(new []{startMessage, startMessage2}, _=>{},
                                       result =>
                                       {
                                           foreach (DisposableMessage endMessage in result.EndMessages)
                                           {
                                               endMessage.IsDisposed.Should().BeFalse("I am using it.");
                                           }
                                       });
            DisposableMessage message = new(startMessage);
            message.SetUserCount(1);
            DisposableMessage message2 = new(startMessage2);
            aggregator.Check(message);
            message.Used();
            aggregator.Check(message2);

            message.IsDisposed.Should().BeTrue("the aggregator is finished.");
        }

        private class DisposableMessage : Message
        {
            public DisposableMessage(params Message[] messages)
                : base(messages)
            {
            }
            
            public bool IsDisposed { get; private set; }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    IsDisposed = true;
                }
                base.Dispose(disposing);
            }

            protected override string DataToString()
            {
                return string.Empty;
            }
        }
    }
}