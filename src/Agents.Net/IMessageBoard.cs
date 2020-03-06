#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
//
///////////////////////////////////////////////////////////////////////////////
#endregion

namespace Agents.Net
{
    public interface IMessageBoard
    {
        void Publish(Message message);
        void Start();
        void Register(MessageDefinition trigger, Agent agent);
        void RegisterInterceptor(MessageDefinition trigger, InterceptorAgent agent);
    }
}