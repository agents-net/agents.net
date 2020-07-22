#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;

namespace Agents.Net
{
    /// <summary>
    /// This attribute declares which <see cref="Message"/>s are consumed by the <see cref="Agent"/>.
    /// </summary>
    /// <remarks>
    /// This attribute is only valid for <see cref="Agent"/>s.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ConsumesAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsumesAttribute"/> class.
        /// </summary>
        /// <param name="messageType">The type the the <see cref="Message"/> that is consumed by the <see cref="Agent"/>.</param>
        public ConsumesAttribute(Type messageType)
        {
            MessageType = messageType;
        }

        /// <summary>
        /// Gets the type of <see cref="Message"/> that is consumed by the <see cref="Agent"/>.
        /// </summary>
        public Type MessageType { get; }
        
        /// <summary>
        /// Gets or sets the Boolean value indicating whether the <see cref="MessageType"/> is consumed explicitly or
        /// implicitly. The default value is <c>false</c>.
        /// </summary>
        /// <returns><c>true</c> if the <see cref="MessageType"/> is consumed implicitly; otherwise <c>false</c>.</returns>
        /// <remarks>
        /// Consuming the message implicitly means, that the <see cref="Agent.Execute"/> method is not called for this
        /// <see cref="MessageType"/>. The <see cref="Message"/> is retrieved and used otherwise. If this property is
        /// set to <c>false</c> it has no effect on the execution, but serves as documentation.
        /// </remarks>
        public bool Implicitly { get; set; }
    }
}