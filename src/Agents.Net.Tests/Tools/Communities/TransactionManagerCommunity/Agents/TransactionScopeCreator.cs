#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;
using Agents.Net;
using Agents.Net.Tests.Tools.Communities.TransactionManagerCommunity.Messages;

namespace Agents.Net.Tests.Tools.Communities.TransactionManagerCommunity.Agents
{
    [Consumes(typeof(InformationGathered))]
    [Consumes(typeof(TransactionFinished))]
    [Consumes(typeof(ExceptionMessage))]
    [Produces(typeof(TransactionStarted))]
    [Produces(typeof(TransactionSuccessful))]
    [Produces(typeof(TransactionRollback))]
    public class TransactionScopeCreator : Agent
    {
        private readonly MessageGate<TransactionStarted, TransactionFinished> gate = new ();
        public TransactionScopeCreator(IMessageBoard messageBoard) : base(messageBoard)
        {
        }

        protected override void ExecuteCore(Message messageData)
        {
            if (messageData.TryGet(out InformationGathered informationGathered))
            {
                gate.SendAndContinue(new TransactionStarted(messageData, informationGathered.Information), OnMessage,
                                     result =>
                                     {
                                         if (result.Result == MessageGateResultKind.Success)
                                         {
                                             OnMessage(new TransactionSuccessful(result.EndMessage));
                                         }
                                         else
                                         {
                                             OnMessage(new TransactionRollback(result.Exceptions));
                                         }
                                     });
            }
            else
            {
                gate.Check(messageData);
            }
        }
    }
}
