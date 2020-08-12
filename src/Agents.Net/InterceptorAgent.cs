#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Runtime.ExceptionServices;
using NLog;

namespace Agents.Net
{
    [Produces(typeof(ExceptionMessage))]
    public abstract class InterceptorAgent : Agent
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly string agentName;

        protected InterceptorAgent(IMessageBoard messageBoard, string name = null) : base(messageBoard)
        {
            agentName = string.IsNullOrEmpty(name) ? GetType().Name : name;
        }

        protected override void ExecuteCore(Message messageData)
        {
            //override when not intercepted messages are consumed
        }

        public InterceptionAction Intercept(Message messageData)
        {
            Logger.Trace(new AgentLog(messageData, "Intercepting", agentName));

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

        protected abstract InterceptionAction InterceptCore(Message messageData);
    }
}
