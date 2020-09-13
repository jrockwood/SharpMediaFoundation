// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncCallbackBase.cs" company="Justin Rockwood">
//   Copyright (c) Justin Rockwood. All Rights Reserved. Licensed under the Apache License, Version 2.0. See
//   LICENSE.txt in the project root for license information.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------------

namespace SharpMediaFoundation
{
    using SharpGen.Runtime;

    /// <summary>
    /// A default implementation of <see cref="IAsyncCallback"/>.
    /// </summary>
    public abstract class AsyncCallbackBase : CallbackBase, IAsyncCallback
    {
        //// ===========================================================================================================
        //// Properties
        //// ===========================================================================================================

        public virtual AsyncCallbackFlags Flags => AsyncCallbackFlags.None;

        public virtual WorkQueueId WorkQueueId => WorkQueueId.Standard;

        //// ===========================================================================================================
        //// Methods
        //// ===========================================================================================================

        public abstract void Invoke(AsyncResult asyncResult);
    }
}
