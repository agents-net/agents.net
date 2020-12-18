#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;
using Agents.Net;

namespace Agents.Net.Benchmarks.FileManipulation
{
    [Consumes(typeof(RelevantFileFoundMessage))]
    [Consumes(typeof(FileFoundMessage), Implicitly = true)]
    [Produces(typeof(FileCompletedMessage))]
    public class FileManipulator : Agent
    {
        public FileManipulator(IMessageBoard messageBoard) : base(messageBoard)
        {
        }

        protected override void ExecuteCore(Message messageData)
        {
            messageData.Get<FileFoundMessage>().File.ManipulateFile();
            OnMessage(new FileCompletedMessage(messageData));
        }
    }
}
