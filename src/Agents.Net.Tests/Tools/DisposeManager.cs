using System.Collections.Concurrent;
using FluentAssertions;

namespace Agents.Net.Tests.Tools
{
    [Consumes(typeof(Message))]
    public class DisposeManager : Agent
    {
        public DisposeManager(IMessageBoard messageBoard) : base(messageBoard)
        {
        }
        
        private readonly ConcurrentBag<DisposableMessage> messages = new ConcurrentBag<DisposableMessage>();

        public void CheckAllDisposed()
        {
            messages.Should().OnlyContain(message => message.IsDisposed);
        }

        protected override void ExecuteCore(Message messageData)
        {
            if (messageData.TryGet(out DisposableMessage disposableMessage))
            {
                messages.Add(disposableMessage);
            }
        }
    }
}