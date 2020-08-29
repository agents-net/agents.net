using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Agents.Net.Tests.Tools
{
    internal class WaitingConsole : IConsole, IDisposable
    {
        private readonly ConcurrentStack<string> messages = new ConcurrentStack<string>();
        private readonly AutoResetEvent messageReceived = new AutoResetEvent(false);

        public void WriteLine(string message)
        {
            messages.Push(message);
            messageReceived.Set();
        }

        public bool WaitForMessages(out IEnumerable<string> receivedMessages, int timeout = 300)
        {
            while (messageReceived.WaitOne(timeout))
            {
                //do nothing
            }

            receivedMessages = messages.ToArray();

            return receivedMessages.Any();
        }

        public void Dispose()
        {
            messageReceived?.Dispose();
        }
    }
}
