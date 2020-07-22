using System.Collections.Generic;
using System.IO;
using System.Linq;
using Agents.Net;

namespace Agents.Net.Benchmarks.FileManipulation
{
    public class AllFilesFoundMessage : Message
    {        public AllFilesFoundMessage(IEnumerable<FileInfo> infos, Message predecessorMessage,
                                    params Message[] childMessages)
            : base(predecessorMessage, childMessages:childMessages)
        {
            Infos = infos;
        }

        public AllFilesFoundMessage(IEnumerable<FileInfo> infos, IEnumerable<Message> predecessorMessages,
                                    params Message[] childMessages)
            : base(predecessorMessages, childMessages:childMessages)
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
