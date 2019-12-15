#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;

namespace Agents.Net
{
    [AttributeUsage(AttributeTargets.Property)]
    public class MessageDefinitionAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class AgentDefinitionAttribute : Attribute
    {
    }
}
