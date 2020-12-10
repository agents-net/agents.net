#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System.Collections.Generic;
using System.Runtime.ExceptionServices;

namespace Agents.Net
{
    /// <summary>
    /// This message is send whenever there is an exception during the execution of an <see cref="Agent"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// When there is a known error state in an <see cref="Agent"/> it should send the exception message directly because it is faster than throwing an exception and catching it again.
    /// </para>
    /// <para>
    /// No one will handle these messages by default. That means, that the program will continue to be executed even after an exception occured. In order to change that behavior an <see cref="Agent"/> must be defined which handles these messages and decides when to stop the program. For that exceptions are captured with their respective <see cref="ExceptionDispatchInfo"/>. With that the exception can be recreated together with the stacktrace by calling <see cref="ExceptionDispatchInfo.Throw"/>.
    /// </para>
    /// </remarks>
    public class ExceptionMessage : Message
    {
        /// <summary>
        /// Get the captured <see cref="ExceptionDispatchInfo"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// With that the exception can be recreated together with the stacktrace by calling <see cref="ExceptionDispatchInfo.Throw"/>.
        /// </para>
        /// <para>
        /// It can be <c>null</c>, when the <see cref="CustomMessage"/> is used instead.
        /// </para>
        /// </remarks>
        public ExceptionDispatchInfo ExceptionInfo { get; }
        /// <summary>
        /// Get the custom exception message.
        /// </summary>
        /// <remarks>
        /// When an exception is captured, this value is <c>null</c>.
        /// </remarks>
        public string CustomMessage { get; }
        /// <summary>
        /// The agent which produced the exception.
        /// </summary>
        public Agent Agent { get; }

        /// <summary>
        /// Initialized a new instance of the class <see cref="ExceptionMessage"/> with a captured exception.
        /// </summary>
        /// <param name="exceptionInfo">The captured exception info.</param>
        /// <param name="message">The predecessor message.</param>
        /// <param name="agent">The agent that produced the exception.</param>
        public ExceptionMessage(ExceptionDispatchInfo exceptionInfo, Message message, Agent agent) : base(message)
        {
            ExceptionInfo = exceptionInfo;
            Agent = agent;
        }

        /// <summary>
        /// Initialized a new instance of the class <see cref="ExceptionMessage"/> with a custom message.
        /// </summary>
        /// <param name="customMessage">The custom exception text.</param>
        /// <param name="message">The predecessor message.</param>
        /// <param name="agent">The agent that produced the exception.</param>
        public ExceptionMessage(string customMessage, Message message, Agent agent) : base(message)
        {
            CustomMessage = customMessage;
            Agent = agent;
        }

        /// <summary>
        /// Initialized a new instance of the class <see cref="ExceptionMessage"/> with a captured exception.
        /// </summary>
        /// <param name="exceptionInfo">The captured exception info.</param>
        /// <param name="messages">The predecessor messages.</param>
        /// <param name="agent">The agent that produced the exception.</param>
        public ExceptionMessage(ExceptionDispatchInfo exceptionInfo, IEnumerable<Message> messages, Agent agent) : base(messages)
        {
            ExceptionInfo = exceptionInfo;
            Agent = agent;
        }

        /// <summary>
        /// Initialized a new instance of the class <see cref="ExceptionMessage"/> with a custom message.
        /// </summary>
        /// <param name="customMessage">The custom exception text.</param>
        /// <param name="messages">The predecessor messages.</param>
        /// <param name="agent">The agent that produced the exception.</param>
        public ExceptionMessage(string customMessage, IEnumerable<Message> messages, Agent agent) : base(messages)
        {
            CustomMessage = customMessage;
            Agent = agent;
        }

        /// <summary>
        /// Overridden data method.
        /// </summary>
        /// <returns><see cref="string.Empty"/></returns>
        /// <remarks>
        /// This is not used as  exception messages are locked differently than normal messages.
        /// </remarks>
        protected override string DataToString()
        {
            return string.Empty;
        }
    }
}