#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

namespace Agents.Net.Tests.Tools.Communities.LegacyServiceBridgeCommunity
{
    public interface ILegacyService
    {
        string ServiceCall(bool throwException);
    }
}