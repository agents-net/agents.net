using System;
using Agents.Net;

namespace Agents.Net.Benchmarks.FileManipulation
{
    [Consumes(typeof(FileFoundMessage))]
    [Produces(typeof(FileCompletedMessage))]
    public class GenericFileCompleter : Agent
    {        public GenericFileCompleter(IMessageBoard messageBoard) : base(messageBoard)
        {
        }

        protected override void ExecuteCore(Message messageData)
        {
            if (!MessageDecorator.IsDecorated(messageData))
            {
                OnMessage(new FileCompletedMessage(messageData));
            }
        }
    }
}
