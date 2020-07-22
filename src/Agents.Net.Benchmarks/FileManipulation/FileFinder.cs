using System;
using System.Collections.Generic;
using System.IO;
using Agents.Net;

namespace Agents.Net.Benchmarks.FileManipulation
{
    [Consumes(typeof(RootDirectoryDefinedMessage))]
    [Produces(typeof(FileFoundMessage))]
    public class FileFinder : Agent
    {        public FileFinder(IMessageBoard messageBoard) : base(messageBoard)
        {
        }

        protected override void ExecuteCore(Message messageData)
        {
            RootDirectoryDefinedMessage definedMessage = messageData.Get<RootDirectoryDefinedMessage>();
            List<Message> messages = new List<Message>();
            foreach (FileInfo file in definedMessage.RootDirectory.EnumerateFiles())
            {
                messages.Add(new FileFoundMessage(file, messageData));
            }
            OnMessages(messages);
        }
    }
}
