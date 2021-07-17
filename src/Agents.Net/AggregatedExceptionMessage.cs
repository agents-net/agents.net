#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;

namespace Agents.Net
{
    /// <summary>
    /// This is an aggregated message that contains one or more <see cref="ExceptionMessage"/>s.
    /// </summary>
    public class AggregatedExceptionMessage : ExceptionMessage
    {
        /// <summary>
        /// The aggregated exceptions.
        /// </summary>
        public IEnumerable<ExceptionMessage> Exceptions { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="AggregatedExceptionMessage"/>.
        /// </summary>
        /// <param name="message">An optional custom message for this message.</param>
        /// <param name="exceptions">The aggregated exception messages.</param>
        public AggregatedExceptionMessage(IReadOnlyCollection<ExceptionMessage> exceptions, string message = null)
            : base(exceptions, message)
        {
            Exceptions = exceptions;
        }

        /// <inheritdoc />
        protected override string DataToString()
        {
            return $"{nameof(CustomMessage)}: {CustomMessage}; {nameof(Exceptions)}: {Environment.NewLine}{string.Join(Environment.NewLine,Exceptions)}";
        }
    }
}