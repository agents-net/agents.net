#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System.Collections.Generic;
using Agents.Net;

namespace Agents.Net.Tests.Tools.Communities.TransactionManagerCommunity.Messages
{
    public class TransactionSuccessful : Message
    {
        public TransactionSuccessful(Message predecessorMessage): base(predecessorMessage)
        {
        }

        public TransactionSuccessful(IEnumerable<Message> predecessorMessages): base(predecessorMessages)
        {
        }

        protected override string DataToString()
        {
            return string.Empty;
        }
    }
}
