#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;
using System.Linq;
using Agents.Net;

namespace Agents.Net.Benchmarks.FileManipulation
{
    [Intercepts(typeof(AllFilesFoundMessage))]
    [Produces(typeof(AllRelevantFilesFoundMessage))]
    public class ParallelFileFilter : InterceptorAgent
    {
        public ParallelFileFilter(IMessageBoard messageBoard) : base(messageBoard)
        {
        }

        protected override InterceptionAction InterceptCore(Message messageData)
        {
            AllFilesFoundMessage files = messageData.Get<AllFilesFoundMessage>();
            AllRelevantFilesFoundMessage.Decorate(files, files.Infos.Where(i => i.IsRelevantFile()));
            return InterceptionAction.Continue;
        }
    }
}
