#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
    /// that each agent does exactly one thing. There are only three use cases were this helper should be used.
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
    ///         Use <see cref="SendAndAwait"/> for that use case.
    ///     </description>
    /// </item>
    /// <item>
    ///     <term><b>When a single agent's task is to handle message pairs:</b><br/></term>
    ///     <description>
    ///         This is necessary for example when handling transactions. In this case the gate can be used to determine whether to rollback the transaction or not. Another example would be when an <see cref="InterceptorAgent"/> that delays the execution of a message so that an injected message chain can be executed.
    ///         Use <see cref="SendAndContinue"/> for that use case.
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
    ///         Use <see cref="SendAndContinue"/> for that use case.
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
    ///         MessageGateResult&lt;TEnd&gt; result = gate.SendAndAwait(parameters, OnMessage);
    ///         //evaluate result and return TResult
    ///     }
    /// }
    /// </code>
    /// </example>
    public class MessageGate<TStart,TEnd> 
        where TEnd : Message
        where TStart : Message
    {
        private readonly MessageCollector<TStart, ExceptionMessage> exceptionCollector;
        private readonly MessageCollector<TStart, TEnd> pairCollector = new MessageCollector<TStart, TEnd>();
        private readonly Dictionary<TStart, CancellationTokenSource> exceptionCancelTokens = new Dictionary<TStart, CancellationTokenSource>();
        private readonly Dictionary<TStart, List<ExceptionMessage>> exceptions = new Dictionary<TStart, List<ExceptionMessage>>();

        /// <summary>
        /// Initializes a new instance of <see cref="MessageGate{TStart,TEnd}"/>
        /// </summary>
        public MessageGate()
        {
            exceptionCollector = new MessageCollector<TStart, ExceptionMessage>(OnExceptionMessagesCollected);
        }

        private void OnExceptionMessagesCollected(MessageCollection<TStart, ExceptionMessage> set)
        {
            set.MarkAsConsumed(set.Message2);

            lock (exceptions)
            {
                if (exceptions.TryGetValue(set.Message1, out List<ExceptionMessage> messages))
                {
                    messages.Add(set.Message2);
                }
            }

            lock (exceptionCancelTokens)
            {
                if (exceptionCancelTokens.TryGetValue(set.Message1, out CancellationTokenSource source))
                {
                    source.Cancel();
                }
            }
        }

        /// <summary>
        /// A constant value to tell the <see cref="MessageGate{TStart,TEnd}"/> that it has to wait forever.
        /// </summary>
        public const int NoTimout = -1;

        /// <summary>
        /// The method to send a start message and wait for the end message.
        /// </summary>
        /// <param name="startMessage">The start message.</param>
        /// <param name="onMessage">The action to send the message.</param>
        /// <param name="timeout">
        /// Optionally a timeout after which the method will return, without sending the result.
        /// By default the timeout is <see cref="NoTimout"/>
        /// </param>
        /// <param name="cancellationToken">
        /// Optionally a cancellation token to cancel the wait operation. This is helpful, when for example the
        /// imitated service call is an async method. By default no CancellationToken will be used.
        /// </param>
        /// <returns>The <see cref="MessageGateResult{TEnd}"/> of the operation.</returns>
        /// <remarks>
        /// <para>For an example how to use this class see the type documentation.</para>
        /// <para>
        /// WARNING: Extensive use of this method will lead to time gaps in the execution. See .net issue on github: https://github.com/dotnet/runtime/issues/55562
        /// Use this method only for the legacy service call. Of all other scenarios use <see cref="SendAndContinue"/>.
        /// </para>
        /// </remarks>
        public MessageGateResult<TEnd> SendAndAwait(TStart startMessage, Action<Message> onMessage, int timeout = NoTimout,
                                                    CancellationToken cancellationToken = default)
        {
            if (startMessage == null)
            {
                throw new ArgumentNullException(nameof(startMessage));
            }

            if (onMessage == null)
            {
                throw new ArgumentNullException(nameof(onMessage));
            }
            
            using (ManualResetEventSlim resetEvent = new ManualResetEventSlim())
            {
                ManualResetEventSlim local = resetEvent;
                MessageGateResult<TEnd> result = null;
                SendAndContinue(startMessage, onMessage, r =>
                {
                    result = r;
                    local.Set();
                },timeout, cancellationToken);
                
                //Do not use cancellation token here. The this is handled in the SendAndContinueMethod
                resetEvent.Wait();
                return result;
            }
        }

        /// <summary>
        /// The method to send a start message and wait for the end message.
        /// </summary>
        /// <param name="startMessage">The start message.</param>
        /// <param name="onMessage">The action to send the message.</param>
        /// <param name="continueAction">The action to execute once a <see cref="MessageGateResult{TEnd}"/> was created.</param>
        /// <param name="timeout">
        /// Optionally a timeout after which the method will return, without sending the result.
        /// By default the timeout is <see cref="NoTimout"/>
        /// </param>
        /// <param name="cancellationToken">
        /// Optionally a cancellation token to cancel the continue operation. By default no CancellationToken will be used.
        /// </param>
        /// <remarks>
        /// <para>For an example how to use this class see the type documentation.</para>
        /// </remarks>
        public void SendAndContinue(TStart startMessage, Action<Message> onMessage, Action<MessageGateResult<TEnd>> continueAction, int timeout = NoTimout,
                                    CancellationToken cancellationToken = default)
        {
            if (startMessage == null)
            {
                throw new ArgumentNullException(nameof(startMessage));
            }

            if (onMessage == null)
            {
                throw new ArgumentNullException(nameof(onMessage));
            }

            if (continueAction == null)
            {
                throw new ArgumentNullException(nameof(continueAction));
            }

            CancellationToken userCancelToken = cancellationToken;
            CancellationToken timeoutCancelToken = default;
            List<CancellationTokenSource> sources = new List<CancellationTokenSource>();
            CancellationTokenSource exceptionSource = new CancellationTokenSource();
            sources.Add(exceptionSource);
            lock (exceptionCancelTokens)
            {
                exceptionCancelTokens.Add(startMessage, exceptionSource);
            }
            CancellationToken exceptionCancelToken = exceptionSource.Token;
            if (timeout != NoTimout)
            {
                CancellationTokenSource timeoutSource = new CancellationTokenSource(timeout);
                sources.Add(timeoutSource);
                timeoutCancelToken = timeoutSource.Token;
            }
            CancellationTokenSource combinedSource = CancellationTokenSource.CreateLinkedTokenSource(userCancelToken, timeoutCancelToken, exceptionCancelToken);
            cancellationToken = combinedSource.Token;
            sources.Add(combinedSource);
            
            lock (exceptions)
            {
                exceptions.Add(startMessage, new List<ExceptionMessage>());
            }

            MessageDomain.CreateNewDomainsFor(startMessage);
            exceptionCollector.Push(startMessage);
            CancellationTokenRegistration register = default;
            pairCollector.PushAndContinue(startMessage, set =>
            {
                set.MarkAsConsumed(set.Message1);
                set.MarkAsConsumed(set.Message2);
                TEnd endMessage = set.Message2;
                    
                MessageDomain.TerminateDomainsOf(startMessage);
                
                Dispose();
                MessageGateResult<TEnd> result = new MessageGateResult<TEnd>(MessageGateResultKind.Success, endMessage, Enumerable.Empty<ExceptionMessage>());
                continueAction(result);
            }, cancellationToken);
            register = cancellationToken.Register(() =>
            {
                MessageDomain.TerminateDomainsOf(startMessage);
                MessageGateResultKind resultKind = MessageGateResultKind.Success;
                if (userCancelToken.IsCancellationRequested)
                {
                    resultKind = MessageGateResultKind.Canceled;
                }
                else if (exceptionCancelToken.IsCancellationRequested)
                {
                    resultKind = MessageGateResultKind.Exception;
                }
                else if (timeoutCancelToken.IsCancellationRequested)
                {
                    resultKind = MessageGateResultKind.Timeout;
                }

                IEnumerable<ExceptionMessage> currentExceptions;
                lock (exceptions)
                {
                    currentExceptions = exceptions[startMessage];
                    exceptions.Remove(startMessage);
                }

                Dispose();
                MessageGateResult<TEnd> result = new MessageGateResult<TEnd>(resultKind, null, currentExceptions);
                continueAction(result);
            });
                
            onMessage(startMessage);

            void Dispose()
            {
                foreach (CancellationTokenSource tokenSource in sources)
                {
                    tokenSource.Dispose();
                }
                
                register.Dispose();

                lock (exceptionCancelTokens)
                {
                    exceptionCancelTokens.Remove(startMessage);
                }
            }
        }

        /// <summary>
        /// Checks whether the provided exception message is the end message or an exception message for the awaited <see cref="SendAndAwait"/> operation.
        /// </summary>
        /// <param name="message">The message to check.</param>
        /// <returns><c>true</c>, if the message was the end message or an exception message for the <see cref="SendAndAwait"/> operation.</returns>
        /// <remarks>For an example how to use this class see the type documentation.</remarks>
        public bool Check(Message message)
        {
            bool result = exceptionCollector.TryPush(message);
            result |= pairCollector.TryPush(message);
            return result;
        }
    }
}