using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using BenchmarkDotNet.Attributes;
using NLog;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;
using NLog.Targets.Wrappers;

namespace Agents.Net.Benchmarks.SequentialOverhead
{
    /// <summary>
    /// This benchmark tests the agent framework performance when executing sequential tasks with enabled logging.
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
    public class LoggingOverheadBenchmark
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly AutoResetEvent finishedEvent = new AutoResetEvent(false);
        private IMessageBoard messageBoard;
        private const int Iterations = 1000;
        private LoggingConfiguration emptyConfiguration;

        [GlobalSetup]
        public void Setup()
        {
            messageBoard = new MessageBoard();
            SpinWaiter waiter = new SpinWaiter(messageBoard, () => finishedEvent.Set(), false);
            messageBoard.Register(waiter);
            messageBoard.Start();

            LoggingConfiguration config = new LoggingConfiguration();
            Layout layout = new JsonLayout
            {
                Attributes =
                {
                    new JsonAttribute("time", Layout.FromString(@"${longdate}")),
                    new JsonAttribute("level", Layout.FromString(@"${level:upperCase=true}")),
                    new JsonAttribute("message", Layout.FromString(@"${message}")),
                    new JsonAttribute("logger", Layout.FromString(@"${logger}")),
                    new JsonAttribute("exception", new JsonLayout
                    {
                        Attributes = 
                        { 
                            new JsonAttribute("type", "${exception:format=Type}"),
                            new JsonAttribute("message", "${exception:format=Message}"),
                            new JsonAttribute("stacktrace", "${exception:format=StackTrace}"),
                            new JsonAttribute("innerException", new JsonLayout
                            {
                                Attributes =
                                {
                                    new JsonAttribute("type", "${exception:format=:innerFormat=Type:MaxInnerExceptionLevel=5:InnerExceptionSeparator=}"),
                                    new JsonAttribute("message", "${exception:format=:innerFormat=Message:MaxInnerExceptionLevel=5:InnerExceptionSeparator=}"),
                                    new JsonAttribute("stacktrace", "${exception:format=:innerFormat=StackTrace:MaxInnerExceptionLevel=5:InnerExceptionSeparator=}"),
                                },
                                RenderEmptyObject = false
                            }, false)
                        },
                        RenderEmptyObject = false
                    },false),
                }
            };
            layout = new CompoundLayout
            {
                Layouts =
                {
                    layout,
                    Layout.FromString(",")
                }
            };
// Targets where to log to: File and Console
            string logFileName = Path.GetTempFileName();
            FileTarget logfile = new FileTarget("logfile")
            {
                FileName = logFileName,
                DeleteOldFileOnStartup = true,
                Layout = layout,
                Footer ="{}]",
                Header = "[",
            };
            BufferingTargetWrapper wrapper = new BufferingTargetWrapper(logfile, 1000, 100, BufferingTargetWrapperOverflowAction.Flush);
// Rules for mapping loggers to targets            
            config.AddRule(LogLevel.Trace, LogLevel.Fatal, wrapper);
// Apply config    
            emptyConfiguration = LogManager.Configuration;
            LogManager.Configuration = config;
        }

        [Params(-1, 1)]
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
                Logger.Trace("Executed "+i);
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
            LogManager.Configuration = emptyConfiguration;
        }
    }
}
