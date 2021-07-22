#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using Agents.Net;

namespace Agents.Net.Benchmarks.FileManipulation
{
    [Consumes(typeof(RootDirectoryDefinedMessage))]
    [Produces(typeof(FileFoundMessage))]
    public class FileFinder : Agent
    {
        private readonly MessageGate<FileFoundMessage, FileCompletedMessage> gate = new();
        private readonly Action terminateAction;
        
        public FileFinder(IMessageBoard messageBoard, Action terminateAction) : base(messageBoard)
        {
            this.terminateAction = terminateAction;
        }

        protected override void ExecuteCore(Message messageData)
        {
            RootDirectoryDefinedMessage definedMessage = messageData.Get<RootDirectoryDefinedMessage>();
            List<FileFoundMessage> messages = new();
            foreach (FileInfo file in definedMessage.RootDirectory.EnumerateFiles())
            {
                messages.Add(new FileFoundMessage(file, messageData));
            }
            gate.SendAndContinue(messages,OnMessage, result =>
            {
                terminateAction();
            });
        }
    }
}
