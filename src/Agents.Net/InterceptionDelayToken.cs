#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;

namespace Agents.Net
{
    /// <summary>
    /// The delay token to release a message that was delayed with the <see cref="InterceptionAction.Delay"/> result.
    /// </summary>
    public class InterceptionDelayToken
    {
        internal InterceptionDelayToken()
        {
            
        }
        
        private Action<DelayTokenReleaseIntention> onRelease;
        internal void Register(Action<DelayTokenReleaseIntention> onRelease)
        {
            this.onRelease = onRelease;
        }
        
        /// <summary>
        /// Release the message. After this the intercepted message will be send.
        /// </summary>
        /// <param name="intention">Indicates whether to publish the delayed message or not.</param>
        /// <remarks>
        /// If at least one <see cref="InterceptionDelayToken"/> returns the intention <see cref="DelayTokenReleaseIntention.DoNotPublish"/> the delayed message is not published.
        /// </remarks>
        public void Release(DelayTokenReleaseIntention intention = DelayTokenReleaseIntention.Publish)
        {
            onRelease?.Invoke(intention);
        }
    }

    /// <summary>
    /// The intention for the <see cref="InterceptionDelayToken.Release"/> method.
    /// </summary>
    public enum DelayTokenReleaseIntention
    {
        /// <summary>
        /// Publish the delayed message
        /// </summary>
        Publish,
        /// <summary>
        /// Do not publish the delayed message
        /// </summary>
        DoNotPublish
    }
}