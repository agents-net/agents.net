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
    public class HandledExceptionMessage : DecoratedMessage
    {
        #region Definition

        [MessageDefinition]
        public static MessageDefinition HandledExceptionMessageDefinition { get; } =
            new MessageDefinition(nameof(HandledExceptionMessage));

        #endregion

        public HandledExceptionMessage(ExceptionMessage decoratedMessage, IEnumerable<Message> additionalPredecessors = null)
            : base(decoratedMessage, HandledExceptionMessageDefinition, additionalPredecessors)
        {
        }

        protected override string DataToString()
        {
            return string.Empty;
        }
    }
}
