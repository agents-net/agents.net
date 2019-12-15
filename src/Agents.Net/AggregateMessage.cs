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
    public abstract class AggregateMessage : Message
    {
        public IEnumerable<Message> AggregatedMessages { get; }

        protected AggregateMessage(IEnumerable<Message> aggregatedMessages, MessageDefinition messageDefinition, params Message[] childMessages) : base(aggregatedMessages, messageDefinition, childMessages)
        {
            AggregatedMessages = aggregatedMessages;
        }
    }
}
