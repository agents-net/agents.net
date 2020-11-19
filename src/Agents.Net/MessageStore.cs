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
    public sealed class MessageStore<T> : IDisposable
        where T:Message
    {
        private readonly T message;
        private readonly IDisposable disposableUse;

        public MessageStore(T message)
        {
            this.message = message;
            disposableUse = message?.DelayDispose();
        }

        public static implicit operator T(MessageStore<T> store) => FromMessageStore(store);
        public static implicit operator MessageStore<T>(T message) => ToMessageStore(message);

        public static T FromMessageStore(MessageStore<T> store) => store?.message;
        public static MessageStore<T> ToMessageStore(T message) => new MessageStore<T>(message);

        public void Dispose()
        {
            disposableUse?.Dispose();
        }
    }
}