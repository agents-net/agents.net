﻿#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System.Collections.Generic;
using Agents.Net;

namespace Agents.Net.Benchmarks.FileManipulation
{
    public class FilesCompletedMessage : Message
    {
        public FilesCompletedMessage(Message predecessorMessage)
            : base(predecessorMessage)
        {
        }

        public FilesCompletedMessage(IEnumerable<Message> predecessorMessages)
            : base(predecessorMessages)
        {
        }

        protected override string DataToString()
        {
            return string.Empty;
        }
    }
}
