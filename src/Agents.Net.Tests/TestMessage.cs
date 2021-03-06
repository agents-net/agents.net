﻿#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;
using System.Collections.Generic;
using Agents.Net;

namespace Agents.Net.Tests
{
    public class TestMessage : Message
    {
        public TestMessage(Message predecessorMessage)
            : base(predecessorMessage)
        {
        }

        public TestMessage(IEnumerable<Message> predecessorMessages)
            : base(predecessorMessages)
        {
        }

        public TestMessage()
            : base(Array.Empty<Message>())
        {
        }

        protected override string DataToString()
        {
            return string.Empty;
        }
    }

    public class TestMessageDecorator : MessageDecorator
    {
        private TestMessageDecorator(Message decoratedMessage, IEnumerable<Message> additionalPredecessors = null)
            : base(decoratedMessage, additionalPredecessors)
        {
        }

        public static TestMessageDecorator Decorate(Message message,
                                                    IEnumerable<Message> additionalPredecessors = null)
        {
            return new TestMessageDecorator(message, additionalPredecessors);
        }

        protected override string DataToString()
        {
            return string.Empty;
        }
    }
}
