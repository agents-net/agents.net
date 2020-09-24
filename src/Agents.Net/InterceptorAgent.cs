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
using Serilog;
using Serilog.Events;

namespace Agents.Net
{
    [Produces(typeof(ExceptionMessage))]
    public abstract class InterceptorAgent : Agent
    {
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

        protected abstract InterceptionAction InterceptCore(Message messageData);
    }
}
