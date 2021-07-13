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
        
        private Action onRelease;
        internal void Register(Action onRelease)
        {
            this.onRelease = onRelease;
        }
        
        /// <summary>
        /// Release the message. After this the intercepted message will be send.
        /// </summary>
        public void Release()
        {
            onRelease?.Invoke();
        }
    }
}