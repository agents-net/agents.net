﻿using System;
using System.Collections.Generic;
using System.Linq;
using Agents.Net;

namespace Agents.Net.Benchmarks.FileManipulation
{
    [Consumes(typeof(FileCompletedMessage))]
    public class FileCompletedAggregator : Agent
    {        private readonly MessageAggregator<FileCompletedMessage> aggregator;
        private readonly Action terminateAction;

        public FileCompletedAggregator(IMessageBoard messageBoard, Action terminateAction) : base(messageBoard)
        {
            this.terminateAction = terminateAction;
            aggregator = new MessageAggregator<FileCompletedMessage>(OnAggregated);
        }

        private void OnAggregated(ICollection<FileCompletedMessage> aggregate)
        {
            FileCompletedMessage[] messageCollection = aggregate.ToArray();
            MessageDomain.TerminateDomainsOf(messageCollection);
            terminateAction();
        }

        protected override void ExecuteCore(Message messageData)
        {
            aggregator.Aggregate(messageData);
        }
    }
}
