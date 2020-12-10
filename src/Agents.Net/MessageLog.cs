#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;
using System.Collections.Generic;

namespace Agents.Net
{
    /// <summary>
    /// This class is used to log a message in JSON format.
    /// </summary>
    /// <remarks>
    /// It can be used for analysing tests or the log.
    /// </remarks>
    public class MessageLog
    {
        /// <summary>
        /// Initialized a new instance of the class <see cref="MessageLog"/>.
        /// </summary>
        /// <param name="name">The name of the message.</param>
        /// <param name="id">The message id.</param>
        /// <param name="predecessors">The message predecessors.</param>
        /// <param name="domain">The message domain.</param>
        /// <param name="data">The data string representation.</param>
        /// <param name="child">The child, if existing.</param>
        public MessageLog(string name, Guid id, IEnumerable<Guid> predecessors, Guid domain, string data, MessageLog child)
        {
            Name = name;
            Id = id;
            Predecessors = predecessors;
            Domain = domain;
            Data = data;
            Child = child;
        }

        /// <summary>
        /// Get the name of the message type.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Get the id of the message.
        /// </summary>
        public Guid Id { get; }
        /// <summary>
        /// Get the ids of the predecessor messages.
        /// </summary>
        public IEnumerable<Guid> Predecessors { get; }
        /// <summary>
        /// Get the id of the message domain.
        /// </summary>
        public Guid Domain { get; }
        /// <summary>
        /// The the string representation of the message data.
        /// </summary>
        /// <remarks>
        /// This is the value for the <see cref="Message.DataToString"/> method.
        /// </remarks>
        public string Data { get; }
        /// <summary>
        /// Get the <see cref="MessageLog"/> representation of the child message.
        /// </summary>
        public MessageLog Child { get; }
    }
}