#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;

namespace Agents.Net
{
    /// <summary>
    /// The first message in the lifetime of a <see cref="IMessageBoard"/>. It is produced the by the <see cref="IMessageBoard"/> when the <see cref="IMessageBoard.Start"/> method is called.
    /// </summary>
    public class InitializeMessage : Message
    {
        /// <summary>
        /// Initialized a new instance of the class <see cref="InitializeMessage"/>.
        /// </summary>
        /// <remarks>
        /// This message does not have any predecessor message, therefore None can be given here.
        /// </remarks>
        public InitializeMessage() : base(Array.Empty<Message>())
        {
        }

        /// <summary>
        /// Overridden data method.
        /// </summary>
        /// <returns><see cref="string.Empty"/></returns>
        protected override string DataToString()
        {
            return string.Empty;
        }
    }
}
