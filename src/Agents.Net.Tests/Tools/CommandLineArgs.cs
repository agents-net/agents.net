using System;
using System.Collections.Generic;
using System.Text;

namespace Agents.Net.Tests.Tools
{
    public class CommandLineArgs
    {
        public CommandLineArgs(params string[] arguments)
        {
            Arguments = arguments;
        }

        public string[] Arguments { get; }
    }
}
