#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
//
///////////////////////////////////////////////////////////////////////////////
#endregion

namespace Agents.Net
{
    public class InitializeMessage : Message
    {
        #region Definition

        [MessageDefinition]
        public static MessageDefinition InitializeMessageDefinition { get; } =
            new MessageDefinition(nameof(InitializeMessage));

        #endregion

        public InitializeMessage() : base(new Message[0], InitializeMessageDefinition)
        {
        }

        protected override string DataToString()
        {
            return string.Empty;
        }
    }
}
