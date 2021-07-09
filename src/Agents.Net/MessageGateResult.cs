#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System.Collections.Generic;

namespace Agents.Net
{
    /// <summary>
    /// The result of the <see cref="MessageGate{TStart,TEnd}.SendAndAwait"/> method.
    /// </summary>
    /// <typeparam name="TEnd">The type of the end message</typeparam>
    /// <remarks>See <see cref="MessageGate{TStart,TEnd}"/> to understand the usage of this class.</remarks>
    public class MessageGateResult<TEnd> where TEnd : Message
    {
        /// <summary>
        /// Instantiates the class <see cref="MessageGateResult{TEnd}"/>
        /// </summary>
        /// <param name="result">The result kind.</param>
        /// <param name="endMessage">The end message.</param>
        /// <param name="exceptions">Exception messages.</param>
        public MessageGateResult(MessageGateResultKind result, TEnd endMessage, IEnumerable<ExceptionMessage> exceptions)
        {
            Result = result;
            EndMessage = endMessage;
            Exceptions = exceptions;
        }

        /// <summary>
        /// The result kind. Meaning if the operation was successful or not. 
        /// </summary>
        public MessageGateResultKind Result { get; }
        
        /// <summary>
        /// The end message. Can be <c>null</c> if the operation was not successful.
        /// </summary>
        public TEnd EndMessage { get; }
        
        /// <summary>
        /// Exceptions that were logged during the execution.
        /// </summary>
        /// <remarks>
        /// The operation stops after the first exception. But it is possible that before
        /// the result is instantiated, that the gate received more exceptions.
        /// </remarks>
        public IEnumerable<ExceptionMessage> Exceptions { get; }
    }

    /// <summary>
    /// The result kind of the <see cref="MessageGate{TStart,TEnd}.SendAndAwait"/> method.
    /// </summary>
    /// <remarks>See <see cref="MessageGate{TStart,TEnd}"/> to understand the usage of this enum.</remarks>
    public enum MessageGateResultKind
    {
        /// <summary>
        /// The operation was successful. Meaning the end message was found.
        /// </summary>
        Success,
        /// <summary>
        /// At least one <see cref="ExceptionMessage"/> was received during the execution.
        /// </summary>
        Exception,
        /// <summary>
        /// The operation was canceled by the <see cref="System.Threading.CancellationToken"/> provided to the <see cref="MessageGate{TStart,TEnd}.SendAndAwait"/> method.
        /// </summary>
        Canceled,
        /// <summary>
        /// The operation ran into the configured timeout.
        /// </summary>
        Timeout
    }
}