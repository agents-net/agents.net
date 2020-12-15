#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;
using System.Collections.Generic;
using System.Linq;

namespace Agents.Net
{
    /// <summary>
    /// The base class for message decorators.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class is the only way to build a hierarchy of messages. This can be done with an <see cref="InterceptorAgent"/> or by the message producing agent itself.
    /// </para>
    /// <para>
    /// When using <see cref="Message.Get{T}"/> the whole hierarchy of message decorators is searched for the specific message type.
    /// </para>
    /// </remarks>
    /// <example>
    /// This is the recommended way of writing a message decorator:
    /// <code>
    /// public class SpecificMessageDecorator : MessageDecorator
    /// {
    ///     private SpecificMessageDecorator(Message decoratedMessage, IEnumerable&lt;Message&gt; additionalPredecessors = null) :
    ///         base(decoratedMessage, additionalPredecessors)
    ///     {
    ///     }
    ///     
    ///     public static SpecificMessageDecorator Decorate(DecoratedMessage declaredMessage, IEnumerable&lt;Message&gt; additionalPredecessors = null)
    ///     {
    ///         return new SpecificMessageDecorator(declaredMessage);
    ///     }
    /// 
    ///     protected override string DataToString()
    ///     {
    ///         //maybe log additional data
    ///     }
    /// }
    ///
    /// //Used in client code like this:
    ///
    /// SpecificMessageDecorator.Decorate(decoratedMessage);
    /// </code>
    /// </example>
    public abstract class MessageDecorator : Message
    {
        /// <summary>
        /// Initializes a new instance of <see cref="MessageDecorator"/>
        /// </summary>
        /// <param name="decoratedMessage">The message that should be decorated.</param>
        /// <param name="additionalPredecessors">The <paramref name="decoratedMessage"/> is automatically the predecessor of this message. With this parameter additional predecessors can be specified.</param>
        /// <remarks>
        /// The <paramref name="decoratedMessage"/> is not necessarily the direct <see cref="Message.Child"/> of this message, as it is undetermined which message decorator comes first when two decorators are applied at the same time. But it is thread safe to do so. 
        /// </remarks>
        protected MessageDecorator(Message decoratedMessage, IEnumerable<Message> additionalPredecessors = null) 
            : base(decoratedMessage?.Predecessors.Concat(additionalPredecessors ?? Enumerable.Empty<Message>()).Distinct()
            ??Enumerable.Empty<Message>())
        {
            SwitchDomain(decoratedMessage?.MessageDomain);
            SetChild(decoratedMessage?.ReplaceHead(this));
        }

        /// <summary>
        /// Checks whether the <paramref name="message"/> is a decorated message or not.
        /// </summary>
        /// <param name="message">The message to check.</param>
        /// <returns><c>true</c> if the message is decorated; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">If the <paramref name="message"/> is <c>null</c>.</exception>
        /// <remarks>
        /// An example when this method can be used is shown in the FileManipulationBenchmark benchmark test.
        /// </remarks>
        public static bool IsDecorated(Message message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            return message.Is<MessageDecorator>();
        }
    }
}
