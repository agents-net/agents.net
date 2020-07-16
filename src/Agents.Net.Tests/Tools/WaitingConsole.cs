using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
        }

        public bool WaitForMessage(out string message, int timeout = 100)
        {
            while (messageReceived.WaitOne(timeout)) { }

            return messages.TryPeek(out message);
        }

        public void Dispose()
        {
            messageReceived?.Dispose();
        }
    }
}
