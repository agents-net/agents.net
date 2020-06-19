using System.Collections.Generic;
using Agents.Net;

namespace Agents.Net.Benchmarks.FileManipulation
{
    public class FilesCompletedMessage : Message
    {
        #region Definition

        [MessageDefinition]
        public static MessageDefinition FilesCompletedMessageDefinition { get; } =
            new MessageDefinition(nameof(FilesCompletedMessage));

        #endregion

        public FilesCompletedMessage(Message predecessorMessage, params Message[] childMessages)
            : base(predecessorMessage, FilesCompletedMessageDefinition, childMessages)
        {
        }

        public FilesCompletedMessage(IEnumerable<Message> predecessorMessages, params Message[] childMessages)
            : base(predecessorMessages, FilesCompletedMessageDefinition, childMessages)
        {
        }

        protected override string DataToString()
        {
            return string.Empty;
        }
    }
}
