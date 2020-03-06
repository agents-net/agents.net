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
    public class InitializeMessage : Message
    {
        #region Definition

        [MessageDefinition]
        public static MessageDefinition InitializeMessageDefinition { get; } =
            new MessageDefinition(nameof(InitializeMessage));

        #endregion

        public InitializeMessage() : base(Array.Empty<Message>(), InitializeMessageDefinition)
        {
        }

        protected override string DataToString()
        {
            return string.Empty;
        }
    }
}
