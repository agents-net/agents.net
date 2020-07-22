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
        void Register(params Agent[] agents);
    }
}