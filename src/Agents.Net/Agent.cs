#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Text;
using Serilog;
using Serilog.Events;

namespace Agents.Net
{
    /// <summary>
    /// Base class for all agents. It is necessary to inherit this base
    /// class in order to receive messages from the <see cref="IMessageBoard"/>.
    /// </summary>
    /// <remarks>
    /// The agents must define which messages they want to receive, intercept and which messages they produce.
    /// When messages are intercepted it is necessary to use the <see cref="InterceptorAgent"/> base class.
    /// </remarks>
    /// <example>
    /// <code>
    /// [Consumes(typeof(ConsumedMessage))]
    /// [Produces(typeof(ProducedMessage))]
    /// public class AgentImplementation : Agent
    /// {
    ///     //Implementation
    /// }
    /// </code>
    /// </example>
    public abstract class Agent
    {
        private readonly IMessageBoard messageBoard;
        private readonly string agentName;

        /// <summary>
        /// Initialized a new instance of the class <see cref="Agent"/>.
        /// </summary>
        /// <param name="messageBoard">The message board to send messages.</param>
        /// <param name="name">Optional name of the agent. The default is the name of the type.</param>
        /// <remarks>
        /// The <paramref name="name"/> is only used for logging purposes.
        /// </remarks>
        protected Agent(IMessageBoard messageBoard, string name = null)
        {
            this.messageBoard = messageBoard;
            agentName = string.IsNullOrEmpty(name) ? GetType().Name : name;
        }

        /// <summary>
        /// The id of the agent.
        /// </summary>
        /// <remarks>
        /// The id is only used for logging.
        /// </remarks>
        protected Guid Id { get; } = Guid.NewGuid();

        /// <summary>
        /// This method is called by the message boards to execute a certain message.
        /// </summary>
        /// <param name="messageData">The message data that is executed.</param>
        /// <exception cref="ArgumentNullException">When the message data is null.</exception>
        /// <remarks>
        /// Only messages which are defined with the <see cref="ConsumesAttribute"/> are passed to this method.
        /// Only the <see cref="IMessageBoard"/> should call this. It can also be used in unit tests.
        /// This method executes the <see cref="ExecuteCore"/> method with the provided message. Additionally it logs all received messages and throw an exception method if the <see cref="ExecuteCore"/> method throws an exception. 
        /// </remarks>
        public void Execute(Message messageData)
        {
            if (messageData == null)
            {
                throw new ArgumentNullException(nameof(messageData));
            }

            if (Log.IsEnabled(LogEventLevel.Verbose))
            {
                Log.Verbose("{@log}",
                            new AgentLog(agentName, "Executing", Id, messageData.ToMessageLog()));
            }

            try
            {
                ExecuteCore(messageData);
            }
            catch (Exception e)
            {
                ExceptionDispatchInfo exceptionInfo = ExceptionDispatchInfo.Capture(e);
                OnMessage(new ExceptionMessage(exceptionInfo, messageData, this));
            }
        }

        /// <summary>
        /// This method is used to send a single message to the message board.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <remarks>
        /// The message will be send to the <see cref="IMessageBoard"/> which was passed in the constructor.
        /// </remarks>
        protected void OnMessage(Message message)
        {
            if (Log.IsEnabled(LogEventLevel.Verbose))
            {
                if (message is ExceptionMessage exceptionMessage &&
                    exceptionMessage.ExceptionInfo != null)
                {
                    Log.Verbose(exceptionMessage.ExceptionInfo.SourceException, "{@log}",
                                new AgentLog(agentName, "Publishing", Id, message.ToMessageLog()));
                }
                else
                {
                    Log.Verbose("{@log}",
                                new AgentLog(agentName, "Publishing", Id, message?.ToMessageLog()));
                }
            }
            messageBoard.Publish(message);
        }

        /// <summary>
        /// This method is used to send multiple messages to the message board at the same time. All message open up a new message domain.
        /// </summary>
        /// <param name="messages">All messages to be send.</param>
        /// <remarks>
        /// The message will be send to the <see cref="IMessageBoard"/> which was passed in the constructor.
        /// To accumulate all messages again it is necessary that all send messages are of the same type.
        /// </remarks>
        protected void OnMessages(IReadOnlyCollection<Message> messages)
        {
            MessageDomain.CreateNewDomainsFor(messages);
            foreach (Message message in messages)
            {
                OnMessage(message);
            }
        }

        /// <summary>
        /// The method which should be overridden by the implementation of the agent. It accepts the messages in the same way the <see cref="Execute"/> method does.
        /// </summary>
        /// <param name="messageData">The received message.</param>
        protected abstract void ExecuteCore(Message messageData);
    }
}