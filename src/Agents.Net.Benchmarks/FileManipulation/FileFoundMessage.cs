#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System.Collections.Generic;
using System.IO;
using Agents.Net;

namespace Agents.Net.Benchmarks.FileManipulation
{
    public class FileFoundMessage : Message
    {
        public FileFoundMessage(FileInfo file, Message predecessorMessage)
            : base(predecessorMessage)
        {
            File = file;
        }

        public FileFoundMessage(FileInfo file, IEnumerable<Message> predecessorMessages)
            : base(predecessorMessages)
        {
            File = file;
        }

        public FileInfo File { get; }

        protected override string DataToString()
        {
            return $"{nameof(File)}: {File}";
        }
    }
}
