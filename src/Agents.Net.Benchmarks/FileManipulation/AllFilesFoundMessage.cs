using System.Collections.Generic;
using System.IO;
using System.Linq;
using Agents.Net;

namespace Agents.Net.Benchmarks.FileManipulation
{
    public class AllFilesFoundMessage : Message
    {
        public AllFilesFoundMessage(IEnumerable<FileInfo> infos, Message predecessorMessage)
            : base(predecessorMessage)
        {
            Infos = infos;
        }

        public AllFilesFoundMessage(IEnumerable<FileInfo> infos, IEnumerable<Message> predecessorMessages)
            : base(predecessorMessages)
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
