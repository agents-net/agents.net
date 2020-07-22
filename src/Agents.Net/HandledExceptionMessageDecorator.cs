#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Collections.Generic;

namespace Agents.Net
{
    public class HandledExceptionMessageDecorator : MessageDecorator
    {
        private HandledExceptionMessageDecorator(ExceptionMessage decoratedMessage, IEnumerable<Message> additionalPredecessors = null)
            : base(decoratedMessage, additionalPredecessors)
        {
        }

        public static HandledExceptionMessageDecorator Decorate(ExceptionMessage exceptionMessage,
            params Message[] additionalPredecessors)
        {
            return new HandledExceptionMessageDecorator(exceptionMessage, additionalPredecessors);
        }

        protected override string DataToString()
        {
            return string.Empty;
        }
    }
}
