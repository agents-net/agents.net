﻿#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Text;

namespace Agents.Net
{
    public abstract class InterceptorAgent : Agent
    {
        protected InterceptorAgent(InterceptorAgentDefinition interceptorDefinition, IMessageBoard messageBoard) : base(interceptorDefinition, messageBoard)
        {
            InterceptorDefinition = interceptorDefinition;
        }

        public InterceptorAgentDefinition InterceptorDefinition { get; }

        protected override void ExecuteCore(Message messageData)
        {
            //override when not intercepted messages are consumed
        }

        public InterceptionAction Intercept(Message messageData)
        {
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
