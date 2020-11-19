#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;

namespace Agents.Net
{
    public class InitializeMessage : Message
    {
        public InitializeMessage() : base(Array.Empty<Message>())
        {
        }

        protected override string DataToString()
        {
            return string.Empty;
        }
    }
}
