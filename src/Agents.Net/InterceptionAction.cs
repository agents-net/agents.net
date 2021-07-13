#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

namespace Agents.Net
{
    /// <summary>
    /// The action that should be taken with the message that was intercepted.
    /// </summary>
    public class InterceptionAction
    {
        internal InterceptionResult Result { get; }

        internal InterceptionDelayToken DelayToken { get; }

        private InterceptionAction(InterceptionResult result)
        {
            Result = result;
        }

        private InterceptionAction(InterceptionDelayToken delayToken)
        {
            DelayToken = delayToken;
            Result = InterceptionResult.Delay;
        }

        /// <summary>
        /// Publish the message to all consuming agents.
        /// </summary>
        public static InterceptionAction Continue => new InterceptionAction(InterceptionResult.Continue);
        /// <summary>
        /// Do not publish the message. If at least one <see cref="InterceptorAgent"/> returns this action, the message will not be published.
        /// </summary>
        /// <remarks>
        /// This action cannot be mixed with the <see cref="DoNotPublish"/> action. This will lead to an exception message.
        /// </remarks>
        public static InterceptionAction DoNotPublish => new InterceptionAction(InterceptionResult.DoNotPublish);
        /// <summary>
        /// Delay the message. If at least one <see cref="InterceptorAgent"/> returns this action, the message will be published only after all <see cref="InterceptionDelayToken"/>s are released.
        /// </summary>
        /// <param name="delayToken">The delay token with which to release the message for sending.</param>
        /// <remarks>
        /// This action cannot be mixed with the <see cref="DoNotPublish"/> action. This will lead to an exception message.
        /// </remarks>
        public static InterceptionAction Delay(out InterceptionDelayToken delayToken)
        {
            delayToken = new InterceptionDelayToken();
            return new InterceptionAction(delayToken);
        }
    }

    internal enum InterceptionResult
    {
        Continue,
        DoNotPublish,
        Delay
    }
}
