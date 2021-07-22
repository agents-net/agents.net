#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;
using System.Collections.Generic;

namespace Agents.Net
{
    /// <summary>
    /// This is the message that will be send automatically by the <see cref="MessageGate{TStart, TEnd}.SendAndAggregate"/> method.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MessagesAggregated<T> : Message where T:Message
    {
        /// <summary>
        /// The result of the aggregation.
        /// </summary>
        public MessageAggregationResult<T> Result { get; }

        internal MessagesAggregated(MessageAggregationResult<T> aggregationResult)
            : base(aggregationResult.EndMessages)
        {
            Result = aggregationResult;
        }

        /// <inheritdoc />
        protected override string DataToString()
        {
            return $"{nameof(Result)}: {Result}";
        }
    }
}