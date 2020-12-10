#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

namespace Agents.Net
{
    /// <summary>
    /// The action that should be taken with the message that was intercepted.
    /// </summary>
    public enum InterceptionAction
    {
        /// <summary>
        /// Publish the message to all consuming agents.
        /// </summary>
        Continue,
        /// <summary>
        /// Do not publish the message. If at least one <see cref="InterceptorAgent"/> returns this action, the message will not be published.
        /// </summary>
        DoNotPublish
    }
}
