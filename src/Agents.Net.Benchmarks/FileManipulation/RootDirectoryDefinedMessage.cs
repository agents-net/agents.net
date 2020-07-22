using System;
using System.Collections.Generic;
using System.IO;
using Agents.Net;

namespace Agents.Net.Benchmarks.FileManipulation
{
    public class RootDirectoryDefinedMessage : Message
    {        public RootDirectoryDefinedMessage(DirectoryInfo rootDirectory)
            : base(Array.Empty<Message>())
        {
            RootDirectory = rootDirectory;
        }

        public DirectoryInfo RootDirectory { get; }

        protected override string DataToString()
        {
            return $"{nameof(RootDirectory)}: {RootDirectory}";
        }
    }
}
