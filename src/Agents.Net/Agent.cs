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

namespace Agents.Net
{
    public abstract class Agent
    {
        private readonly MessageBoard messageBoard;

        protected Agent(AgentDefinition definition, MessageBoard messageBoard)
        {
            Definition = definition;
            this.messageBoard = messageBoard;
        }

        public AgentDefinition Definition { get; }

        public bool Completed { get; private set; }

        public void Execute(Message messageData)
        {
            Completed = false;
            try
            {
                ExecuteCore(messageData);
            }
            catch (Exception e)
            {
                ExceptionDispatchInfo exceptionInfo = ExceptionDispatchInfo.Capture(e);
                OnMessage(new ExceptionMessage(exceptionInfo, messageData, this));
            }

            Completed = true;
        }

        protected void OnMessage(Message message)
        {
            messageBoard.Publish(message);
        }

        protected abstract void ExecuteCore(Message messageData);
    }
}