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
        #region Definition

        [MessageDefinition]
        public static MessageDefinition HandledExceptionMessageDecoratorDefinition { get; } =
            new MessageDefinition(nameof(HandledExceptionMessageDecorator));

        #endregion

        public HandledExceptionMessageDecorator(ExceptionMessage decoratedMessage, IEnumerable<Message> additionalPredecessors = null)
            : base(decoratedMessage, HandledExceptionMessageDecoratorDefinition, additionalPredecessors)
        {
        }

        protected override string DataToString()
        {
            return string.Empty;
        }
    }
}
