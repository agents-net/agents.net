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
using NLog;

namespace Agents.Net
{
    public abstract class Agent
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IMessageBoard messageBoard;
        private readonly string agentName;

        protected Agent(AgentDefinition definition, IMessageBoard messageBoard, string name = null)
        {
            this.messageBoard = messageBoard;
            Definition = definition;
            agentName = string.IsNullOrEmpty(name) ? GetType().Name : name;
        }

        public AgentDefinition Definition { get; }

        public void Execute(Message messageData)
        {
            if (messageData == null)
            {
                throw new ArgumentNullException(nameof(messageData));
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
            Logger.Trace(new AgentLog(message, "Publishing", agentName));
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

        private class AgentLog
        {
            private readonly Message message;
            private readonly string type;
            private readonly string agentName;

            public AgentLog(Message message, string type, string agentName)
            {
                this.message = message;
                this.type = type;
                this.agentName = agentName;
            }

            public override string ToString()
            {
                StringBuilder messageBuilder = new StringBuilder("{\"Message\": ");
                messageBuilder.Append(message.ToStringBuilder());
                messageBuilder.Append(", \"Agent\": \"");
                messageBuilder.Append(agentName);
                messageBuilder.Append("\", \"Type\": \"Publishing\"}");
                return messageBuilder.ToString();
            }
        }
    }
}