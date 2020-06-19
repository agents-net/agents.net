using System.Collections.Generic;
using System.IO;
using System.Linq;
using Agents.Net;

namespace Agents.Net.Benchmarks.FileManipulation
{
    public class AllFilesFoundMessage : Message
    {
        #region Definition

        [MessageDefinition]
        public static MessageDefinition AllFilesFoundMessageDefinition { get; } =
            new MessageDefinition(nameof(AllFilesFoundMessage));

        #endregion

        public AllFilesFoundMessage(IEnumerable<FileInfo> infos, Message predecessorMessage,
                                    params Message[] childMessages)
            : base(predecessorMessage, AllFilesFoundMessageDefinition, childMessages)
        {
            Infos = infos;
        }

        public AllFilesFoundMessage(IEnumerable<FileInfo> infos, IEnumerable<Message> predecessorMessages,
                                    params Message[] childMessages)
            : base(predecessorMessages, AllFilesFoundMessageDefinition, childMessages)
        {
            Infos = infos;
        }

        public IEnumerable<FileInfo> Infos { get; }

        protected override string DataToString()
        {
            return $"{nameof(Infos)}: {Infos.Count()}";
        }
    }
}
