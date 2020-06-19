using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;

namespace Agents.Net.Benchmarks.FileManipulation
{
    /// <summary>
    /// This benchmark tests the agent framework performance in manipulating files.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     <strong>Use Case</strong>
    ///   </para>
    ///   <para>Checking 2000 Files. Changing the content of 400 files. 1500 files have the wrong file extension. 100 files start with a line containing "DoNotTouchThis". This scenario is a scenario which should show the strength of the framework to do parallel execution. It contains some random elements like the speed of the hard drive, therefore this is mor of a real world scenario.</para>
    ///   <para>
    ///     <strong>Scenarios</strong>
    ///   </para>
    ///   <list type="table">
    ///     <item>
    ///       <description>
    ///         <c>SingleThread</c>
    ///       </description>
    ///       <description>Processing all file one by one.</description>
    ///     </item>
    ///     <item>
    ///       <description>
    ///         <c>ParallelForEach</c>
    ///       </description>
    ///       <description>Using <c>Parallel.ForEach</c> with default value for number of threads used.</description>
    ///     </item>
    ///     <item>
    ///       <description>
    ///         <c>AgentFramework</c>
    ///       </description>
    ///       <description>Using the agent framework to manipulate all files.</description>
    ///     </item>
    ///     <item>
    ///       <description>
    ///         <c>AgentFrameworkParallelForEach</c>
    ///       </description>
    ///       <description>Using the agent framework to manipulate all files. The actual file manipulation is then done with Parallel.ForEach.</description>
    ///     </item>
    ///   </list>
    /// </remarks>
    public class FileManipulationBenchmark
    {
        private DirectoryInfo directory;
        private DirectoryInfo singleThreadDirectory;
        private DirectoryInfo parallelForEachDirectory;
        private DirectoryInfo agentFrameworkDirectory;
        private DirectoryInfo agentFrameworkParallelForEachDirectory;
        private readonly AutoResetEvent finishedEvent = new AutoResetEvent(false);
        private IContainer container;
        private IMessageBoard messageBoard;
        private readonly AutoResetEvent parallelForEachFinishedEvent = new AutoResetEvent(false);
        private IContainer parallelForEachContainer;
        private IMessageBoard parallelForEachMessageBoard;

        [GlobalSetup]
        public void Setup()
        {
            directory = new DirectoryInfo(Path.Combine(Path.GetTempPath(),Guid.NewGuid().ToString("D")));
            directory.Create();
            for (int i = 0; i < 1500; i++)
            {
                File.WriteAllText(Path.Combine(directory.FullName,$"{Guid.NewGuid():D}.xml"),$"<test>{Guid.NewGuid():D}</test>");
            }

            for (int i = 0; i < 400; i++)
            {
                File.WriteAllText(Path.Combine(directory.FullName, $"{Guid.NewGuid():D}.cs"),
                                  $"// {Guid.NewGuid():D}{Environment.NewLine}" +
                                  $"// {Guid.NewGuid():D}{Environment.NewLine}" +
                                  $"// {Guid.NewGuid():D}{Environment.NewLine}" +
                                  $"// {Guid.NewGuid():D}");
            }

            for (int i = 0; i < 100; i++)
            {
                File.WriteAllText(Path.Combine(directory.FullName,$"{Guid.NewGuid():D}.cs"),$"// DoNotTouchThis{Environment.NewLine}" +
                                                                                            $"// {Guid.NewGuid():D}{Environment.NewLine}" +
                                                                                            $"// {Guid.NewGuid():D}{Environment.NewLine}" +
                                                                                            $"// {Guid.NewGuid():D}{Environment.NewLine}" +
                                                                                            $"// {Guid.NewGuid():D}");
            }

            singleThreadDirectory = directory.CreateSubdirectory("SingleThread");
            parallelForEachDirectory = directory.CreateSubdirectory("ParallelForEach");
            agentFrameworkDirectory = directory.CreateSubdirectory("AgentFramework");
            agentFrameworkParallelForEachDirectory = directory.CreateSubdirectory("AgentFrameworkParallelForEach");
            foreach (FileInfo file in directory.EnumerateFiles())
            {
                file.CopyTo(Path.Combine(singleThreadDirectory.FullName, file.Name));
                file.CopyTo(Path.Combine(parallelForEachDirectory.FullName, file.Name));
                file.CopyTo(Path.Combine(agentFrameworkDirectory.FullName, file.Name));
                file.CopyTo(Path.Combine(agentFrameworkParallelForEachDirectory.FullName, file.Name));
            }

            SetupCommunities();
        }

        private void SetupCommunities()
        {
            {
                ContainerBuilder builder = new ContainerBuilder();
                builder.RegisterModule(new FileManipulationModule(() => { finishedEvent.Set(); }));
                container = builder.Build();
                messageBoard = container.Resolve<IMessageBoard>();
                Community community = container.Resolve<Community>();
                Agent[] agents = container.Resolve<IEnumerable<Agent>>().ToArray();
                community.RegisterAgents(agents);
                messageBoard.Start();
            }
            {
                ContainerBuilder builder = new ContainerBuilder();
                builder.RegisterModule(new ParallelForEachModule(() => { parallelForEachFinishedEvent.Set(); }));
                parallelForEachContainer = builder.Build();
                parallelForEachMessageBoard = parallelForEachContainer.Resolve<IMessageBoard>();
                Community community = parallelForEachContainer.Resolve<Community>();
                Agent[] agents = parallelForEachContainer.Resolve<IEnumerable<Agent>>().ToArray();
                community.RegisterAgents(agents);
                parallelForEachMessageBoard.Start();
            }
        }

        [Benchmark(Baseline = true)]
        public void SingleThread()
        {
            foreach (FileInfo file in singleThreadDirectory.EnumerateFiles().Where(f => f.IsRelevantFile()))
            {
                file.ManipulateFile();
            }
        }

        [Benchmark]
        public void ParallelForEach()
        {
            Parallel.ForEach(parallelForEachDirectory.EnumerateFiles(), info =>
            {
                if (info.IsRelevantFile())
                {
                    info.ManipulateFile();
                }
            });
        }

        [Benchmark]
        public void AgentFramework()
        {
            messageBoard.Publish(new RootDirectoryDefinedMessage(agentFrameworkDirectory));
            finishedEvent.WaitOne();
        }

        [Benchmark]
        public void AgentFrameworkParallelForEach()
        {
            parallelForEachMessageBoard.Publish(new RootDirectoryDefinedMessage(agentFrameworkParallelForEachDirectory));
            parallelForEachFinishedEvent.WaitOne();
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            directory.Delete(true);
            container.Dispose();
            finishedEvent.Dispose();
            parallelForEachContainer.Dispose();
            parallelForEachFinishedEvent.Dispose();
        }
    }
}
