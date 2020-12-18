#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

namespace Agents.Net
{
    /// <summary>
    /// General interface for all communication boards.
    /// </summary>
    /// <remarks>
    /// The communication boards are responsible for sending the messages to all registered agents. 
    /// </remarks>
    public interface IMessageBoard
    {
        /// <summary>
        /// Publish the specific message to all consuming and intercepting agents.
        /// </summary>
        /// <param name="message">The message to publish.</param>
        void Publish(Message message);
        /// <summary>
        /// Start the message board.
        /// </summary>
        /// <remarks>
        /// Without executing this method, no message will be passed the any agent. When it is executed it will automatically send an <see cref="InitializeMessage"/>.
        /// </remarks>
        void Start();
        /// <summary>
        /// Register the agents to the message board.
        /// </summary>
        /// <param name="agents">Agents to register.</param>
        /// <remarks>
        /// <para>
        /// <see cref="Agent"/>s as well as <see cref="InterceptorAgent"/>s are registered with this method.
        /// </para>
        /// <para>
        /// It can be used after the <see cref="Start"/> method, but this should be avoided, as the newly registered agent will not get any messages of the past.
        /// </para>
        /// </remarks>
        void Register(params Agent[] agents);
    }
}