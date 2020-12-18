#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;
using Agents.Net;

namespace Agents.Net.Benchmarks.FileManipulation
{
    [Consumes(typeof(FilesCompletedMessage))]
    public class ParallelForEachFinisher : Agent
    {
        private readonly Action finishAction;

        public ParallelForEachFinisher(IMessageBoard messageBoard, Action finishAction) : base(messageBoard)
        {
            this.finishAction = finishAction;
        }

        protected override void ExecuteCore(Message messageData)
        {
            finishAction();
        }
    }
}
