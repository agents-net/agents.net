using System;
using System.Collections.Generic;
using System.IO;
using Agents.Net;

namespace Agents.Net.Benchmarks.FileManipulation
{
    public class FileFinder : Agent
    {
        #region Definition

        [AgentDefinition]
        public static AgentDefinition FileFinderDefinition { get; }
            = new AgentDefinition(new []
                                  {
                                      RootDirectoryDefinedMessage.RootDirectoryDefinedMessageDefinition
                                  },
                                  new []
                                  {
                                      FileFoundMessage.FileFoundMessageDefinition
                                  });

        #endregion

        public FileFinder(IMessageBoard messageBoard) : base(FileFinderDefinition, messageBoard)
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
            MessageDomain.CreateNewDomainsFor(messages);
            foreach (Message message in messages)
            {
                OnMessage(message);
            }
        }
    }
}
