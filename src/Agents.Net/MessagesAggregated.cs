#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;
using System.Collections.Generic;

namespace Agents.Net
{
    /// <summary>
    /// This is the message that will be send automatically by the <see cref="MessageAggregator{TStart, TEnd}.SendAndAggregate"/> method.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MessagesAggregated<T> : Message where T:Message
    {
        /// <summary>
        /// The result of the aggregation.
        /// </summary>
        public IEnumerable<T> EndMessages { get; }

        internal MessagesAggregated(MessageAggregatorResult<T> aggregatorResult)
            : base(aggregatorResult.EndMessages)
        {
            EndMessages = aggregatorResult.EndMessages;
        }

        /// <inheritdoc />
        protected override string DataToString()
        {
            return $"{nameof(EndMessages)}: {Environment.NewLine}{string.Join(Environment.NewLine, EndMessages)}";
        }
    }
}