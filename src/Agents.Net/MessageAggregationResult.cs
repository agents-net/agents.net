#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System.Collections.Generic;
using System.Linq;

namespace Agents.Net
{
    /// <summary>
    /// The result of the <see cref="MessageGate{TStart,TEnd}.SendAndContinue(IReadOnlyCollection{TStart},System.Action{Agents.Net.Message},System.Action{Agents.Net.MessageAggregationResult{TEnd}},int,System.Threading.CancellationToken)"/> method.
    /// </summary>
    /// <typeparam name="TEnd"></typeparam>
    public class MessageAggregationResult<TEnd> where TEnd : Message
    {
        /// <summary>
        /// Instantiate a new instance of <see cref="MessageAggregationResult{TEnd}"/>.
        /// </summary>
        /// <param name="result">The aggregation result.</param>
        /// <param name="endMessages">The final messages.</param>
        /// <param name="exceptions">Exceptions during execution.</param>
        public MessageAggregationResult(MessageGateResultKind result, IReadOnlyCollection<TEnd> endMessages, IReadOnlyCollection<ExceptionMessage> exceptions)
        {
            Result = result;
            EndMessages = endMessages;
            Exceptions = exceptions;
        }
        
        /// <summary>
        /// The result of the aggregation.
        /// </summary>
        public MessageGateResultKind Result { get; }
        
        /// <summary>
        /// The final messages.
        /// </summary>
        public IReadOnlyCollection<TEnd> EndMessages { get; }
        
        /// <summary>
        /// The exception messages that were recorded.
        /// </summary>
        public IReadOnlyCollection<ExceptionMessage> Exceptions { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{nameof(Result)}: {Result}";
        }
    }
}