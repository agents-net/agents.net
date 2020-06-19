using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Agents.Net.Benchmarks.FileManipulation
{
    internal static class Logic
    {
        public static bool IsRelevantFile(this FileInfo file)
        {
            return file.Extension.Equals(".cs", StringComparison.OrdinalIgnoreCase);
        }

        public static void ManipulateFile(this FileInfo file)
        {
            Stopwatch watch = Stopwatch.StartNew();
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"// {Guid.NewGuid():D}");
            using FileStream fileStream = file.Open(FileMode.Open, FileAccess.ReadWrite);
            string content;
            Encoding encoding;
            using (StreamReader reader = new StreamReader(fileStream, leaveOpen:true))
            {
                encoding = reader.CurrentEncoding;
                content = reader.ReadToEnd();
            }

            try
            {
                string firstLine = content.Substring(0, content.IndexOfAny(new[] {'\r', '\n'}));
                bool manipulate = !firstLine.Contains("DoNotTouchThis", StringComparison.OrdinalIgnoreCase);
            
                if (manipulate)
                {
                    int index = content.IndexOf('\n');
                    builder.Append(content.Substring(index + 1));

                    fileStream.SetLength(0);
                    using StreamWriter writer = new StreamWriter(fileStream, encoding);
                    writer.Write(builder);
                    fileStream.SetLength(fileStream.Position);
                }
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw new ArgumentOutOfRangeException($"Current file: {file.FullName}; Content: {content}",e);
            }
            watch.Stop();
        }
    }
}
