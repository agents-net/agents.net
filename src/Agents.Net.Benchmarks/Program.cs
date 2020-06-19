using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Threading;
using Agents.Net.Benchmarks.FileManipulation;
using Agents.Net.Benchmarks.ParallelThreadSleep;
using Agents.Net.Benchmarks.SequentialOverhead;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace Agents.Net.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<SequentialOverheadBenchmark>();
        }
    }
}
