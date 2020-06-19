using System.Collections.Generic;
using System.IO;
using Agents.Net;

namespace Agents.Net.Benchmarks.FileManipulation
{
    public class FileFoundMessage : Message
    {
        #region Definition

        [MessageDefinition]
        public static MessageDefinition FileFoundMessageDefinition { get; } =
            new MessageDefinition(nameof(FileFoundMessage));

        #endregion

        public FileFoundMessage(FileInfo file, Message predecessorMessage, params Message[] childMessages)
            : base(predecessorMessage, FileFoundMessageDefinition, childMessages)
        {
            File = file;
        }

        public FileFoundMessage(FileInfo file, IEnumerable<Message> predecessorMessages, params Message[] childMessages)
            : base(predecessorMessages, FileFoundMessageDefinition, childMessages)
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
