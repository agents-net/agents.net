#region Copyright

//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT

#endregion

namespace Agents.Net
{
    /// <summary>
    /// The result kind of the <see cref="MessageGate{TStart,TEnd}.SendAndAwait"/> method.
    /// </summary>
    /// <remarks>See <see cref="MessageGate{TStart,TEnd}"/> to understand the usage of this enum.</remarks>
    public enum WaitResultKind
    {
        /// <summary>
        /// The operation was successful. Meaning the end message was found.
        /// </summary>
        Success,
        /// <summary>
        /// At least one <see cref="ExceptionMessage"/> was received during the execution.
        /// </summary>
        Exception,
        /// <summary>
        /// The operation was canceled by the <see cref="System.Threading.CancellationToken"/> provided to the <see cref="MessageGate{TStart,TEnd}.SendAndAwait"/> method.
        /// </summary>
        Canceled,
        /// <summary>
        /// The operation ran into the configured timeout.
        /// </summary>
        Timeout
    }
}