#region Copyright
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
#endregion

using System;

namespace Agents.Net
{
    /// <summary>
    /// A wrapper class for a message that automatically uses <see cref="Message.DelayDispose"/> to delay the message disposal until this store is disposed.
    /// </summary>
    /// <typeparam name="T">The type of the encapsulated message.</typeparam>
    public sealed class MessageStore<T> : IDisposable
        where T:Message
    {
        private readonly T message;
        private readonly IDisposable disposableUse;

        /// <summary>
        /// Creates a new instance of <see cref="MessageStore{T}"/>.
        /// </summary>
        /// <param name="message">The message to encapsulate.</param>
        /// <remarks>
        /// It is not necessary to use this constructor as there are implicit and explicit conversions from and to <see cref="Message"/>.
        /// </remarks>
        public MessageStore(T message)
        {
            this.message = message;
            disposableUse = message?.DelayDispose();
        }

        /// <summary>
        /// Implicitly converts from a <see cref="MessageStore{T}"/> to the encapsulated <see cref="Message"/>. 
        /// </summary>
        /// <param name="store">The store to convert.</param>
        /// <returns>The encapsulated message.</returns>
        public static implicit operator T(MessageStore<T> store) => FromMessageStore(store);
        /// <summary>
        /// Implicitly encapsulates a <see cref="Message"/> in a <see cref="MessageStore{T}"/>.
        /// </summary>
        /// <param name="message">The message to encapsulate.</param>
        /// <returns>The new message store.</returns>
        public static implicit operator MessageStore<T>(T message) => ToMessageStore(message);

        /// <summary>
        /// Explicitly converts from a <see cref="MessageStore{T}"/> to the encapsulated <see cref="Message"/>. 
        /// </summary>
        /// <param name="store">The store to convert.</param>
        /// <returns>The encapsulated message.</returns>
        public static T FromMessageStore(MessageStore<T> store) => store?.message;
        /// <summary>
        /// Explicitly encapsulates a <see cref="Message"/> in a <see cref="MessageStore{T}"/>.
        /// </summary>
        /// <param name="message">The message to encapsulate.</param>
        /// <returns>The new message store.</returns>
        public static MessageStore<T> ToMessageStore(T message) => new MessageStore<T>(message);

        /// <inheritdoc />
        public void Dispose()
        {
            disposableUse?.Dispose();
        }
    }
}