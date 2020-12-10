#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;
using System.Runtime.ExceptionServices;
using Serilog;
using Serilog.Events;

namespace Agents.Net
{
    /// <summary>
    /// Base class for all agents that want to intercept a message.
    /// </summary>
    /// <remarks>
    /// Intercepting a message means, that it gets the message before any agent that is consuming the message. It can than do anything with that message - changing it, replacing it, decorating it, ... At the end the interceptor decides whether to publish the intercepted message or not.
    /// </remarks>
    /// <example>
    /// <code>
    /// [Intercepts(typeof(InterceptedMessage))]
    /// [Consumes(typeof(ConsumedMessage))]
    /// [Produces(typeof(ProducedMessage))]
    /// public class AgentImplementation : Agent
    /// {
    ///     //Implementation
    /// }
    /// </code>
    /// </example>
    [Produces(typeof(ExceptionMessage))]
    public abstract class InterceptorAgent : Agent
    {
        private readonly string agentName;

        /// <summary>
        /// Initialized a new instance of the class <see cref="InterceptorAgent"/>.
        /// </summary>
        /// <param name="messageBoard">The message board to send messages to.</param>
        /// <param name="name">Optional name of the agent. The default is the name of the type.</param>
        /// <remarks>
        /// The <paramref name="name"/> is only used for logging purposes.
        /// </remarks>
        protected InterceptorAgent(IMessageBoard messageBoard, string name = null) : base(messageBoard)
        {
            agentName = string.IsNullOrEmpty(name) ? GetType().Name : name;
        }

        /// <inheritdoc />
        protected override void ExecuteCore(Message messageData)
        {
            //override when not intercepted messages are consumed
        }

        /// <summary>
        /// This method is called by the message boards to intercept a certain message.
        /// </summary>
        /// <param name="messageData">The message that is intercepted.</param>
        /// <exception cref="ArgumentNullException">When the message data is null.</exception>
        /// <returns>The <see cref="InterceptionAction"/>, which defines whether the original message is published or not.</returns>
        /// <remarks>
        /// <para>Only messages which are defined with the <see cref="InterceptsAttribute"/> are passed to this method.</para>
        /// <para>Only the <see cref="IMessageBoard"/> should call this. It can also be used in unit tests.</para>
        /// <para>This method executes the <see cref="InterceptCore"/> method with the provided message. Additionally it logs all received messages and throw an exception method if the <see cref="InterceptCore"/> method throws an exception.</para> 
        /// </remarks>
        public InterceptionAction Intercept(Message messageData)
        {
            if (messageData == null)
            {
                throw new ArgumentNullException(nameof(messageData));
            }
            
            if (Log.IsEnabled(LogEventLevel.Verbose))
            {
                Log.Verbose("{@log}",
                            new AgentLog(agentName, "Intercepting", Id, messageData.ToMessageLog()));
            }

            try
            {
                return InterceptCore(messageData);
            }
            catch (Exception e)
            {
                ExceptionDispatchInfo exceptionInfo = ExceptionDispatchInfo.Capture(e);
                OnMessage(new ExceptionMessage(exceptionInfo, messageData, this));
            }
            return InterceptionAction.DoNotPublish;
        }

        /// <summary>
        /// The method which should be overridden by the implementation of the agent. It accepts the messages in the same way the <see cref="Intercept"/> method does.
        /// </summary>
        /// <param name="messageData">The received message.</param>
        /// <returns>The <see cref="InterceptionAction"/>, which defines whether the original message is published or not.</returns>
        protected abstract InterceptionAction InterceptCore(Message messageData);
    }
}
