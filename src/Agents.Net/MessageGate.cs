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
    /// that each agent does exactly one thing. There are only four use cases were this helper should be used.
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
    ///         Use <see cref="SendAndContinue(TStart,System.Action{Agents.Net.Message},System.Action{Agents.Net.MessageGateResult{TEnd}},int,System.Threading.CancellationToken)"/> for that use case.
    ///     </description>
    /// </item>
    /// <item>
    ///     <term><b>When an agent sends multiple messages of the same type and aggregates them:</b><br/></term>
    ///     <description>
    ///         This is explicit parallelization. The work can be split in batches to execute them parallel. The batches can be as small as a single unit of work. This is almost as effective as creating batches based on the amount of available CPU Cores.
    ///         Use <see cref="SendAndAggregate"/> for that use case.
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
    ///         Use <see cref="SendAndContinue(TStart,System.Action{Agents.Net.Message},System.Action{Agents.Net.MessageGateResult{TEnd}},int,System.Threading.CancellationToken)"/> for that use case.
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
        /// Use this method only for the legacy service call. Of all other scenarios use <see cref="SendAndContinue(TStart,System.Action{Agents.Net.Message},System.Action{Agents.Net.MessageGateResult{TEnd}},int,System.Threading.CancellationToken)"/>.
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

        //Using only a dictionary is faster but there are weird errors.
        private readonly ConcurrentDictionary<MessageDomain, OneTimeAction> continueActions =
            new ConcurrentDictionary<MessageDomain, OneTimeAction>();
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
            if (timeout != NoTimout)
            {
                CancellationTokenSource timeoutSource = new CancellationTokenSource(timeout);
                sources.Add(timeoutSource);
                timeoutCancelToken = timeoutSource.Token;
            }
            CancellationTokenSource combinedSource = CancellationTokenSource.CreateLinkedTokenSource(userCancelToken, timeoutCancelToken);
            cancellationToken = combinedSource.Token;
            sources.Add(combinedSource);

            MessageDomain.CreateNewDomainsFor(startMessage);
            CancellationTokenRegistration register = default;
            OneTimeAction action = new OneTimeAction(OnReceived);
            continueActions.TryAdd(startMessage.MessageDomain, action);
            register = cancellationToken.Register(OnCancel);

            onMessage(startMessage);

            void Dispose()
            {
                foreach (CancellationTokenSource tokenSource in sources)
                {
                    tokenSource.Dispose();
                }
                
                register.Dispose();
            }

            void OnReceived(Message message)
            {
                if (!continueActions.TryRemove(startMessage.MessageDomain, out _))
                {
                    return;
                }

                MessageDomain.TerminateDomainsOf(startMessage);
                if (message.TryGet(out TEnd endMessage))
                {
                    continueAction(new MessageGateResult<TEnd>(MessageGateResultKind.Success, endMessage,
                                                               Enumerable.Empty<ExceptionMessage>()));
                }
                else
                {
                    ExceptionMessage exceptionMessage = message.Get<ExceptionMessage>();
                    continueAction(
                        new MessageGateResult<TEnd>(MessageGateResultKind.Exception, null, new[] {exceptionMessage}));
                }

                Dispose();
            }

            void OnCancel()
            {
                if (!continueActions.TryRemove(startMessage.MessageDomain, out _))
                {
                    return;
                }

                MessageDomain.TerminateDomainsOf(startMessage);
                MessageGateResultKind resultKind = MessageGateResultKind.Success;
                if (userCancelToken.IsCancellationRequested)
                {
                    resultKind = MessageGateResultKind.Canceled;
                }
                else if (timeoutCancelToken.IsCancellationRequested)
                {
                    resultKind = MessageGateResultKind.Timeout;
                }

                MessageGateResult<TEnd> result =
                    new MessageGateResult<TEnd>(resultKind, null, Enumerable.Empty<ExceptionMessage>());
                continueAction(result);
                Dispose();
            }
        }

        /// <summary>
        /// The method to send the start messages and wait for all end messages.
        /// </summary>
        /// <param name="startMessages">The start messages to send.</param>
        /// <param name="onMessage">The action to send the message.</param>
        /// <param name="onAggregated">The action to execute once a <see cref="MessageAggregationResult{TEnd}"/> was created.</param>
        /// <param name="timeout">
        /// Optionally a timeout after which the method will return, without sending the result.
        /// By default the timeout is <see cref="NoTimout"/>
        /// </param>
        /// <param name="cancellationToken">
        /// Optionally a cancellation token to cancel the continue operation. By default no CancellationToken will be used.
        /// </param>
        /// <remarks>
        /// <para>
        /// When a batch of messages is published using the this class they can be united again when all last messages of the execution chain are of the same type or an exception message.
        /// </para>
        /// <code>
        ///  --------------         ---------------------          -----------------
        /// | SplitMessage | ----> | IntermediateMessage | -----> | FinishedMessage |
        ///  --------------         ---------------------  |       -----------------
        ///                                                |
        ///                                                |       ------------------
        ///                                                *----> | ExceptionMessage |
        ///                                                |       ------------------
        ///                                                |
        ///                                                |       ------------------
        ///                                                 ----> | OtherEndMessage  |
        ///                                                        ------------------
        /// </code>
        /// <para>
        /// Looking at the example above it would not be possible to unite the <c>SplitMessages</c> again using this class as at least one <c>IntermediateMessage</c> let to an <c>OtherEndMessage</c>.
        /// </para>
        /// <para>
        /// This function is useful when the aggregated end messages need to be modified - for example
        /// filtered - before aggregating them. In all other cases it is better to use <see cref="SendAndAggregate"/>
        /// to automatically create and send an aggregated message. 
        /// </para>
        /// <example>
        /// This is an example, how to use this method correctly:
        /// <code>
        /// [Consumes(typeof(FinishedMessage))]
        /// [Consumes(typeof(ExceptionMessage))]
        /// [Produces(typeof(StartMessage))]
        /// [Produces(typeof(AggregatedMessage))]
        /// public class MessageAggregatorAgent : Agent
        /// {
        ///     private readonly MessageGate&lt;FinishedMessage&gt; gate = new MessageGate&lt;FinishedMessage&gt;();
        /// 
        ///     public MessageAggregatorAgent(IMessageBoard messageBoard) : base(messageBoard)
        ///     {
        ///     }
        /// 
        ///     protected override void ExecuteCore(Message messageData)
        ///     {
        ///         if(gate.Check(messageData))
        ///         {
        ///             return;
        ///         }
        ///         //create startMessages
        ///         gate.SendAndContinue(startMessages, OnMessage, result =>
        ///         {
        ///             //manipulate the results and produce aggregated message
        ///             OnMessage(aggregatedMessage);
        ///         });
        ///     }
        /// }
        /// </code>
        /// </example>
        /// </remarks>
        public void SendAndContinue(IReadOnlyCollection<TStart> startMessages, Action<Message> onMessage,
                                    Action<MessageAggregationResult<TEnd>> onAggregated, int timeout = NoTimout,
                                    CancellationToken cancellationToken = default)
        {
            if (startMessages == null)
            {
                throw new ArgumentNullException(nameof(startMessages));
            }

            if (onMessage == null)
            {
                throw new ArgumentNullException(nameof(onMessage));
            }

            if (onAggregated == null)
            {
                throw new ArgumentNullException(nameof(onAggregated));
            }

            int aggregated = 0;
            ConcurrentBag<MessageStore<TEnd>> endMessages = new ConcurrentBag<MessageStore<TEnd>>();
            ConcurrentBag<MessageStore<ExceptionMessage>> aggregatedExceptions = new ConcurrentBag<MessageStore<ExceptionMessage>>();
            MessageGateResultKind resultKind = MessageGateResultKind.Success;
            
            foreach (TStart message in startMessages)
            {
                SendAndContinue(message, onMessage, AggregateResult, timeout, cancellationToken);
            }
            
            void AggregateResult(MessageGateResult<TEnd> messageGateResult)
            {
                if (messageGateResult.EndMessage != null)
                {
                    endMessages.Add(messageGateResult.EndMessage);
                }

                foreach (ExceptionMessage exception in messageGateResult.Exceptions)
                {
                    aggregatedExceptions.Add(exception);
                }

                if (messageGateResult.Result != MessageGateResultKind.Success)
                {
                    //It is ok to override other values with the latest
                    resultKind = messageGateResult.Result;
                }

                if (Interlocked.Increment(ref aggregated) == startMessages.Count)
                {
                    CompleteAggregation();
                }

                void CompleteAggregation()
                {
                    onAggregated(
                        new MessageAggregationResult<TEnd>(resultKind, endMessages.Select(s => (TEnd) s).ToArray(),
                                                           aggregatedExceptions.Select(s => (ExceptionMessage) s).ToArray()));
                    foreach (MessageStore<TEnd> store in endMessages)
                    {
                        store.Dispose();
                    }

                    foreach (MessageStore<ExceptionMessage> store in aggregatedExceptions)
                    {
                        store.Dispose();
                    }
                }
            }
        }

        /// <summary>
        /// The method to send the start messages and aggregate all end messages in a <see cref="MessagesAggregated{T}"/> message.
        /// </summary>
        /// <param name="startMessages">The start messages to send.</param>
        /// <param name="onMessage">The action to send the message.</param>
        /// <remarks>
        /// <para>
        /// When a batch of messages is published using the this class they can be united again when all last messages of the execution chain are of the same type or an exception message.
        /// </para>
        /// <code>
        ///  --------------         ---------------------          -----------------
        /// | SplitMessage | ----> | IntermediateMessage | -----> | FinishedMessage |
        ///  --------------         ---------------------  |       -----------------
        ///                                                |
        ///                                                |       ------------------
        ///                                                *----> | ExceptionMessage |
        ///                                                |       ------------------
        ///                                                |
        ///                                                |       ------------------
        ///                                                 ----> | OtherEndMessage  |
        ///                                                        ------------------
        /// </code>
        /// <para>
        /// Looking at the example above it would not be possible to unite the <c>SplitMessages</c> again using this class as at least one <c>IntermediateMessage</c> let to an <c>OtherEndMessage</c>.
        /// </para>
        /// <example>
        /// Here a typical example how to setup and use this method:
        /// <code>
        /// [Consumes(typeof(FinishedMessage))]
        /// [Consumes(typeof(ExceptionMessage))]
        /// [Produces(typeof(StartMessage))]
        /// public class MessageAggregatorAgent : Agent
        /// {
        ///     private readonly MessageGate&lt;FinishedMessage&gt; gate = new MessageGate&lt;FinishedMessage&gt;();
        /// 
        ///     public MessageAggregatorAgent(IMessageBoard messageBoard) : base(messageBoard)
        ///     {
        ///     }
        /// 
        ///     protected override void ExecuteCore(Message messageData)
        ///     {
        ///         if(gate.Check(messageData))
        ///         {
        ///             return;
        ///         }
        ///         //create startMessages
        ///         gate.SendAndAggregate(startMessages, OnMessage);
        ///     }
        /// }
        /// </code>
        /// </example>
        /// </remarks>
        public void SendAndAggregate(IReadOnlyCollection<TStart> startMessages, Action<Message> onMessage)
        {
            if (startMessages == null)
            {
                throw new ArgumentNullException(nameof(startMessages));
            }

            if (onMessage == null)
            {
                throw new ArgumentNullException(nameof(onMessage));
            }

            SendAndContinue(startMessages, onMessage, result =>
            {
                onMessage(new MessagesAggregated<TEnd>(result));
            });
        }

        private bool IsActive => continueActions.Count > 0;

        /// <summary>
        /// Checks whether the provided exception message is the end message or an exception message for the awaited <see cref="SendAndAwait"/> operation.
        /// </summary>
        /// <param name="message">The message to check.</param>
        /// <returns><c>true</c>, if the message was the end message or an exception message for the <see cref="SendAndAwait"/> operation.</returns>
        /// <remarks>For an example how to use this class see the type documentation.</remarks>
        public bool Check(Message message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (!IsActive ||
                !(message.Is<TEnd>() ||
                message.Is<ExceptionMessage>()))
            {
                return false;
            }

            MessageDomain domain = message.MessageDomain;
            while (domain?.IsTerminated == true)
            {
                domain = domain.Parent;
            }

            if (domain != null &&
                continueActions.TryGetValue(domain, out OneTimeAction action))
            {
                action.Execute(message);
                return true;
            }
            return false;
        }

        private class OneTimeAction
        {
            private readonly Action<Message> action;
            private int executed = 0;

            public OneTimeAction(Action<Message> action)
            {
                this.action = action;
            }

            //Not using interlocked leaves a small error but improves performance.
            public void Execute(Message messageData)
            {
                if (Interlocked.Increment(ref executed) == 1)
                {
                    action(messageData);
                }
            }
        }
    }
}