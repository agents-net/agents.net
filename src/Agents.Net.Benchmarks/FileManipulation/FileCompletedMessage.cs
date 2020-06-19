using System.Collections.Generic;
using Agents.Net;

namespace Agents.Net.Benchmarks.FileManipulation
{
    public class FileCompletedMessage : Message
    {
        #region Definition

        [MessageDefinition]
        public static MessageDefinition FileCompletedMessageDefinition { get; } =
            new MessageDefinition(nameof(FileCompletedMessage));

        #endregion

        public FileCompletedMessage(Message predecessorMessage, params Message[] childMessages)
            : base(predecessorMessage, FileCompletedMessageDefinition, childMessages)
        {
        }

        public FileCompletedMessage(IEnumerable<Message> predecessorMessages, params Message[] childMessages)
            : base(predecessorMessages, FileCompletedMessageDefinition, childMessages)
        {
        }

        protected override string DataToString()
        {
            return string.Empty;
        }
    }
}
