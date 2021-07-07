#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;
using System.Threading;

namespace Agents.Net
{
    /// <summary>
    /// This is a helper class which allows to collect a message pair consisting of a start and an end message.
    /// </summary>
    /// <typeparam name="TStart">Type of the start message.</typeparam>
    /// <typeparam name="TEnd">Type of the end message.</typeparam>
    /// <remarks>
    /// <para>This helper should be used with caution as it breaks the basic concept of the agent framework,
    /// that each agent does exactly one thing. There are only two use cases were this helper should be used.
    /// <list type="number">
    /// <item>
    ///     <term><b>An agent serving as a legacy bridge:</b><br/></term>
    ///     <description>
    ///         When working in a legacy system it is sometimes necessary to have an agent serving a service interface.
    ///         In this case it is often so that the service call provides some parameters and expects a
    ///         specific result.<br/>
    ///         To achieve that the service agent needs to wait inside the service call for a specific end message.
    ///         In addition to that, the service call need to terminate once an exception message was send. It would
    ///         wait forever otherwise.
    ///     </description>
    /// </item>
    /// <item>
    ///     <term><b>Calling a simple basic operation inside the agent framework:</b><br/></term>
    ///     <description>
    ///         Let's assume the following use case: The application has a file system abstraction which can
    ///         execute CRUD operations on files and directories. Now I want to create an agent, that creates
    ///         a new configuration file.<br/>
    ///         Without this class the solution would look like this:
    /// <code>
    ///  ---------------         -------------          --------------------------
    /// | ConfigCreator | ----> | FileCreator | -----> | ConfigFileCreatedWatcher |
    ///  ---------------         -------------          --------------------------
    /// FileCreating             FileCreated            ConfigFileCreated
    /// </code>
    ///         The <c>ConfigCreator</c> would only create the <c>FileCreating</c> message and mark it somehow
    ///         for the <c>ConfigFileCreatedWatcher</c> which only marks the <c>FileCreated</c> message. This would
    ///         unnecessarily increase the amount of agents in the system.<br/>
    ///         With this class the solution would look like this:
    /// <code>
    ///  ---------------          -------------
    /// | ConfigCreator | &lt;----> | FileCreator |
    ///  ---------------          -------------
    /// FileCreating             FileCreated
    /// ConfigFileCreated
    /// </code>
    ///         Similar are operations such as executing an external process or database operations are more examples
    ///         were the second use case would helpful.
    ///     </description>
    /// </item>
    /// </list>
    /// </para>
    /// <para> Internally this helper uses <see cref="MessageCollector{T1,T2}"/>s to execute the boilerplate code.
    /// Therefore all information regarding the <see cref="MessageCollector{T1,T2}"/> applies here to, such as
    /// considering message domains.
    /// </para>
    /// <para> Speaking of domains. The class will internally use a message domain to mark the start message. It is
    /// not necessary to do this by the calling code.
    /// </para>
    /// </remarks>
    /// <example>
    /// This example shows the first use case of a legacy service call.
    /// <code>
    /// [Consumes(typeof(TEnd))]
    /// [Consumes(typeof(ExceptionMessage))]
    /// [Produces(typeof(TStart))]
    /// public class ServiceAgentImplementation : Agent, IService
    /// {
    ///     private readonly MessageGate&lt;TStart,TEnd&gt; gate = new MessageGate&lt;TStart,TEnd&gt;();
    /// 
    ///     public ServiceAgentImplementation(IMessageBoard messageBoard) : base(messageBoard)
    ///     {
    ///     }
    ///
    ///     protected override void ExecuteCore(Message messageData)
    ///     {
    ///         gate.Check(messageData);
    ///     }
    ///
    ///     public TResult ServiceCall(TParam parameters)
    ///     {
    ///         MessageGateResult&lt;TEnd&gt; result = gate.SendAndAwait(parameters);
    ///         //evaluate result and return TResult
    ///     }
    /// }
    /// </code>
    /// </example>
    public class MessageGate<TStart,TEnd> 
        where TEnd : Message
        where TStart : Message
    {
        /// <summary>
        /// A constant value to tell the <see cref="MessageGate{TStart,TEnd}"/> that it has to wait forever.
        /// </summary>
        public const int NoTimout = -1;

        /// <summary>
        /// The method to send a start message and wait for the end message.
        /// </summary>
        /// <param name="startMessage">The start message.</param>
        /// <param name="timeout">
        /// Optionally a timeout after which the method will return, without sending the result.
        /// By default the timeout is <see cref="NoTimout"/>
        /// </param>
        /// <param name="cancellationToken">
        /// Optionally a cancellation token to cancel the wait operation. This is helpful, when for example the
        /// imitated service call is an async method. By default no CancellationToken will be used.
        /// </param>
        /// <returns>The <see cref="MessageGateResult{TEnd}"/> of the operation.</returns>
        /// <remarks>For an example how to use this class see the type documentation.</remarks>
        public MessageGateResult<TEnd> SendAndAwait(TStart startMessage, int timeout = NoTimout,
                                                    CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks whether the provided exception message is the end message or an exception message for the awaited <see cref="SendAndAwait"/> operation.
        /// </summary>
        /// <param name="message">The message to check.</param>
        /// <returns><c>true</c>, if the message was the end message or an exception message for the <see cref="SendAndAwait"/> operation.</returns>
        /// <remarks>For an example how to use this class see the type documentation.</remarks>
        public bool Check(Message message)
        {
            throw new NotImplementedException();
        }
    }
}