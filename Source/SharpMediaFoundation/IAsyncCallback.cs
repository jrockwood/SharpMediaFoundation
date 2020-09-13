// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="IAsyncCallback.cs" company="Justin Rockwood">
//   Copyright (c) Justin Rockwood. All Rights Reserved. Licensed under the Apache License, Version 2.0. See
//   LICENSE.txt in the project root for license information.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------------

namespace SharpMediaFoundation
{
    using SharpGen.Runtime;

    [Shadow(typeof(AsyncCallbackShadow))]
    public partial interface IAsyncCallback
    {
        /// <summary>
        /// Gets a flag indicating the behavior of the callback object's <strong><see
        /// cref="IAsyncCallback.Invoke"/></strong> method. Default behavior should be <see cref="AsyncCallbackFlags.None"/>.
        /// </summary>
        /// <value>
        /// The a flag indicating the behavior of the callback object's <strong><see
        /// cref="IAsyncCallback.Invoke"/></strong> method.
        /// </value>
        /// <unmanaged>
        /// HRESULT IMFAsyncCallback::GetParameters([Out] MFASYNC_CALLBACK_FLAGS* pdwFlags,[Out] unsigned int* pdwQueue)
        /// </unmanaged>
        /// <unmanaged-short>IMFAsyncCallback::GetParameters</unmanaged-short>
        AsyncCallbackFlags Flags { get; }

        /// <summary>
        /// Gets the identifier of the work queue on which the callback is dispatched. See remarks.
        /// </summary>
        /// <value>The work queue identifier.</value>
        /// <remarks>
        /// <p>
        /// This value can specify one of the standard Media Foundation work queues, or a work queue created by the
        /// application. For list of standard Media Foundation work queues, see <strong>Work Queue Identifiers</strong>.
        /// To create a new work queue, call <strong><see cref="MediaFactory.AllocateWorkQueue"/></strong>. The default
        /// value is <strong><see cref="WorkQueueType.Standard"/></strong>.
        /// </p>
        /// <p>
        /// If the work queue is not compatible with the value returned in <em>pdwFlags</em>, the Media Foundation
        /// platform returns <strong><see cref="ResultCode.InvalidWorkQueue"/></strong> when it
        /// tries to dispatch the callback. (See <strong><see cref="MediaFactory.PutWorkItem"/></strong>.)
        /// </p>
        /// </remarks>
        /// <unmanaged>
        /// HRESULT IMFAsyncCallback::GetParameters([Out] MFASYNC_CALLBACK_FLAGS* pdwFlags,[Out] unsigned int* pdwQueue)
        /// </unmanaged>
        /// <unmanaged-short>IMFAsyncCallback::GetParameters</unmanaged-short>
        WorkQueueId WorkQueueId { get; }

        /// <summary>
        /// Called when an asynchronous operation is completed.
        /// </summary>
        /// <param name="asyncResult">
        /// Pointer to the <strong><see cref="AsyncResult"/></strong> interface. Pass this reference to the asynchronous
        /// <strong>End...</strong> method to complete the asynchronous call.
        /// </param>
        /// <returns>
        /// <p>
        /// The method returns an <strong><see cref="Result"/></strong>. Possible values include, but are not limited
        /// to, those in the following table.
        /// </p>
        /// <table><tr><th>Return code</th><th>Description</th></tr><tr><td><dl><dt><strong><see cref="Result.Ok"/></strong></dt></dl></td><td>
        /// <p>The method succeeded.</p>
        /// </td></tr></table>
        /// <p>?</p>
        /// </returns>
        /// <remarks>
        /// <p>Within your implementation of <strong>Invoke</strong>, call the corresponding <strong>End...</strong> method.</p>
        /// </remarks>
        /// <msdn-id>bb970360</msdn-id>
        /// <unmanaged>HRESULT IMFAsyncCallback::Invoke([In, Optional] IMFAsyncResult* pAsyncResult)</unmanaged>
        /// <unmanaged-short>IMFAsyncCallback::Invoke</unmanaged-short>
        void Invoke(AsyncResult asyncResult);
    }
}
