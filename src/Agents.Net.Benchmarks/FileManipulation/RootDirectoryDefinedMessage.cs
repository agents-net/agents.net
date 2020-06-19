using System;
using System.Collections.Generic;
using System.IO;
using Agents.Net;

namespace Agents.Net.Benchmarks.FileManipulation
{
    public class RootDirectoryDefinedMessage : Message
    {
        #region Definition

        [MessageDefinition]
        public static MessageDefinition RootDirectoryDefinedMessageDefinition { get; } =
            new MessageDefinition(nameof(RootDirectoryDefinedMessage));

        #endregion

        public RootDirectoryDefinedMessage(DirectoryInfo rootDirectory)
            : base(Array.Empty<Message>(), RootDirectoryDefinedMessageDefinition)
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
