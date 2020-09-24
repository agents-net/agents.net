#region Copyright
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
using Serilog;
using Serilog.Events;

namespace Agents.Net
{
    public abstract class Agent
    {
        private readonly IMessageBoard messageBoard;
        private readonly string agentName;

        protected Agent(IMessageBoard messageBoard, string name = null)
        {
            this.messageBoard = messageBoard;
            agentName = string.IsNullOrEmpty(name) ? GetType().Name : name;
        }

        protected Guid Id { get; } = Guid.NewGuid();

        public void Execute(Message messageData)
        {
            if (messageData == null)
            {
                throw new ArgumentNullException(nameof(messageData));
            }

            if (Log.IsEnabled(LogEventLevel.Verbose))
            {
                Log.Verbose("{@log:lj}",
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

        protected void OnMessage(Message message)
        {
            if (Log.IsEnabled(LogEventLevel.Verbose))
            {
                Log.Verbose("{@log:lj}",
                            new AgentLog(agentName, "Publishing", Id, message?.ToMessageLog()));
            }
            messageBoard.Publish(message);
        }

        protected void OnMessages(IReadOnlyCollection<Message> messages, bool sendDomainCreatedMessage = false)
        {
            MessageDomainsCreatedMessage createdMessage = MessageDomain.CreateNewDomainsFor(messages);
            if (sendDomainCreatedMessage)
            {
                OnMessage(createdMessage);
            }
            else
            {
                createdMessage.Dispose();
            }
            foreach (Message message in messages)
            {
                OnMessage(message);
            }
        }

        protected abstract void ExecuteCore(Message messageData);
    }
}