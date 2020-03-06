#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Diagnostics.Contracts;
using System.Runtime.ExceptionServices;

namespace Agents.Net
{
    public abstract class Agent
    {
        private readonly IMessageBoard messageBoard;

        protected Agent(AgentDefinition definition, IMessageBoard messageBoard)
        {
            this.messageBoard = messageBoard;
            Definition = definition;
        }

        public AgentDefinition Definition { get; }

        public void Execute(Message messageData)
        {
            Contract.Requires(messageData != null, nameof(messageData) + " != null");
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

        protected void OnMessage(Message message)
        {
            messageBoard.Publish(message);
        }

        protected abstract void ExecuteCore(Message messageData);
    }
}