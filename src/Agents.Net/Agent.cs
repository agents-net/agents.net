#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;
using System.Collections.Concurrent;
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
    public abstract class Agent : IDisposable
    {
        private readonly IMessageBoard messageBoard;
        private readonly string agentName;
        private readonly ConcurrentBag<IDisposable> disposables = new ConcurrentBag<IDisposable>();

        /// <summary>
        /// Initialized a new instance of the class <see cref="Agent"/>.
        /// </summary>
        /// <param name="messageBoard">The message board to send messages.</param>
        protected Agent(IMessageBoard messageBoard)
        {
            this.messageBoard = messageBoard;
            agentName = GetType().Name;
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
        /// This method is no deprecated. Please switch to the new <see cref="MessageGate{TStart,TEnd}.SendAndAggregate"/> method.
        /// </summary>
        /// <param name="messages">All messages to be send.</param>
        /// <remarks>
        /// The message will be send to the <see cref="IMessageBoard"/> which was passed in the constructor.
        /// To accumulate all messages again it is necessary that all send messages are of the same type.
        /// </remarks>
        [Obsolete("This method is no longer maintained. Please switch to the new MessageAggregator<TStart,TEnd>.SendAndAggregate method. This method will be removed with version 2022.6")]
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

        /// <summary>
        /// Thread-safely adds an <see cref="IDisposable"/> for disposing on <see cref="Dispose(bool)"/> 
        /// </summary>
        /// <param name="disposable">The disposable to add.</param>
        protected void AddDisposable(IDisposable disposable)
        {
            disposables.Add(disposable);
        }

        /// <summary>
        /// Dispose any resources that are stored in the message.
        /// </summary>
        /// <param name="disposing">If <c>true</c> it was called from the <see cref="Dispose()"/> method.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (IDisposable disposable in disposables)
                {
                    disposable.Dispose();
                }
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}