#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;
using System.Collections.Generic;
using Agents.Net;

namespace Agents.Net.Tests.Tools.Communities.LegacyServiceBridgeCommunity.Messages
{
    public class ServiceParameterPassed : Message
    {
        public ServiceParameterPassed(bool throwException): base(Array.Empty<Message>())
        {
            ThrowException = throwException;
        }

        public bool ThrowException { get; }
        
        protected override string DataToString()
        {
            return $"{nameof(ThrowException)}: {ThrowException}";
        }
    }
}
