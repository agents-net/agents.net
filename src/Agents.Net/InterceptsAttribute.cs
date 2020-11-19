#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;

namespace Agents.Net
{
    /// <summary>
    /// This attribute declares which <see cref="Message"/>s are intercepted by the <see cref="InterceptorAgent"/>.
    /// </summary>
    /// <remarks>
    /// This attribute is only valid for <see cref="InterceptorAgent"/> classes.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class InterceptsAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InterceptsAttribute"/> class.
        /// </summary>
        /// <param name="messageType">The type the the <see cref="Message"/> that is intercepted by the <see cref="InterceptorAgent"/>.</param>
        public InterceptsAttribute(Type messageType)
        {
            MessageType = messageType;
        }

        /// <summary>
        /// Gets the type of <see cref="Message"/> that is intercepted by the <see cref="InterceptorAgent"/>.
        /// </summary>
        public Type MessageType { get; }
    }
}