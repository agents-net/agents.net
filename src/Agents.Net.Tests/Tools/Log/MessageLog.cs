using System;
using System.Collections.Generic;
using System.Text;

namespace Agents.Net.Tests.Tools.Log
{
    public class MessageLog
    {
        public Guid Id { get; set; }
        public string Definition { get; set; }
        public Guid[] Predecessors { get; set; }
        public Guid MessageDomain { get; set; }
        public string Data { get; set; }
        public MessageLog[] Children { get; set; }
    }
}
