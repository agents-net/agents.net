#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System.Collections.Generic;
using Agents.Net;

namespace Agents.Net.Benchmarks.FileManipulation
{
    public class FileCompletedMessage : Message
    {
        public FileCompletedMessage(Message predecessorMessage)
            : base(predecessorMessage)
        {
        }

        public FileCompletedMessage(IEnumerable<Message> predecessorMessages)
            : base(predecessorMessages)
        {
        }

        protected override string DataToString()
        {
            return string.Empty;
        }
    }
}
