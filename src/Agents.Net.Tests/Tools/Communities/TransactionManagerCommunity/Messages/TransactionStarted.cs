#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System.Collections.Generic;
using Agents.Net;

namespace Agents.Net.Tests.Tools.Communities.TransactionManagerCommunity.Messages
{
    public class TransactionStarted : Message
    {
        public TransactionStarted(Message predecessorMessage, string data): base(predecessorMessage)
        {
            Data = data;
        }

        public TransactionStarted(IEnumerable<Message> predecessorMessages, string data): base(predecessorMessages)
        {
            Data = data;
        }

        public string Data { get; }

        protected override string DataToString()
        {
            return $"{nameof(Data)}: {Data}";
        }
    }
}
