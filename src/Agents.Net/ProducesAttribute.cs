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
    /// This attribute declares which <see cref="Message"/>s are produced by the <see cref="Agent"/>.
    /// </summary>
    /// <remarks>
    /// This attribute is only valid for <see cref="Agent"/> classes. It serves only for documentation purposes.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class ProducesAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProducesAttribute"/> class.
        /// </summary>
        /// <param name="messageType">The type the the <see cref="Message"/> that is produced by the <see cref="Agent"/>.</param>
        public ProducesAttribute(Type messageType)
        {
            MessageType = messageType;
        }

        /// <summary>
        /// Gets the type of <see cref="Message"/> that is produced by the <see cref="Agent"/>.
        /// </summary>
        public Type MessageType { get; }
    }
}