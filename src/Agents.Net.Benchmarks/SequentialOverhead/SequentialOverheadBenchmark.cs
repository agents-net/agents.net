using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Diagnostics.Windows.Configs;

namespace Agents.Net.Benchmarks.SequentialOverhead
{
    /// <summary>
    /// This benchmark tests the agent framework performance when executing sequential tasks.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     <strong>Use Case</strong>
    ///   </para>
    ///   <para>Running 1000 workloads sequentially. One workload with 1 ms and one workload with a <c>Thread.SpinWait(15)</c> workload (-1). This will show the overhead the framework will produce in the worst case scenario.</para>
    ///   <para>
    ///     <strong>Scenarios</strong>
    ///   </para>
    ///   <list type="table">
    ///     <item>
    ///       <description>
    ///         <c>SingleThread</c>
    ///       </description>
    ///       <description>Comparision base line.</description>
    ///     </item>
    ///     <item>
    ///       <description>
    ///         <c>AgentFramework</c>
    ///       </description>
    ///       <description>Using the agent framework to execute all tasks sequentially.</description>
    ///     </item>
    ///   </list>
    /// </remarks>
    public class SequentialOverheadBenchmark
    {
        private readonly AutoResetEvent finishedEvent = new AutoResetEvent(false);
        private IMessageBoard messageBoard;
        private const int Iterations = 1000;

        [GlobalSetup]
        public void Setup()
        {
            messageBoard = new MessageBoard();
            Community community = new Community(messageBoard);
            SpinWaiter waiter = new SpinWaiter(messageBoard, () => finishedEvent.Set());
            community.RegisterAgents(new Agent[] {waiter});
            messageBoard.Start();
        }

        [Params(-1)]
        public int Duration { get; set; }

        [Benchmark(Baseline = true)]
        public void SingleThread()
        {
            for (int i = 0; i < Iterations; i++)
            {
                if (Duration > 0)
                {
                    Thread.Sleep(Duration);
                }
                else
                {
                    Thread.SpinWait(15);
                }
            }
        }

        [Benchmark]
        public void AgentFramework()
        {
            messageBoard.Publish(new SpinWaitCountedMessage(Iterations, Duration, Array.Empty<Message>()));
            finishedEvent.WaitOne();
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            (messageBoard as IDisposable)?.Dispose();
            finishedEvent.Dispose();
        }
    }
}
