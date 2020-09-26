using System;
using System.Collections.Generic;

namespace Agents.Net
{
    public class MessageLog
    {
        public MessageLog(string name, Guid id, IEnumerable<Guid> predecessors, Guid domain, string data, MessageLog child)
        {
            Name = name;
            Id = id;
            Predecessors = predecessors;
            Domain = domain;
            Data = data;
            Child = child;
        }

        public string Name { get; }
        public Guid Id { get; }
        public IEnumerable<Guid> Predecessors { get; }
        public Guid Domain { get; }
        public string Data { get; }
        public MessageLog Child { get; }
    }
}