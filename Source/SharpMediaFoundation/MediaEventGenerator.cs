// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="MediaEventGenerator.cs" company="Justin Rockwood">
//   Copyright (c) Justin Rockwood. All Rights Reserved. Licensed under the Apache License, Version 2.0. See
//   LICENSE.txt in the project root for license information.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------------

namespace SharpMediaFoundation
{
    using System;
    using System.Runtime.InteropServices;
    using SharpGen.Runtime;

    public partial class MediaEventGenerator
    {
        /// <summary>
        /// Retrieves the next event in the queue. This method is synchronous.
        /// </summary>
        /// <param name="isBlocking">
        /// <c>true</c> if the method blocks until the event generator queues an event, <c>false</c> otherwise.
        /// </param>
        /// <returns>
        /// a reference to the <strong><see cref="MediaEvent"/></strong> interface. The caller must release the interface.
        /// </returns>
        /// <remarks>
        /// <p>This method executes synchronously.</p>
        /// <p>
        /// If the queue already contains an event, the method returns <see cref="Result.Ok"/> immediately. If the queue
        /// does not contain an event, the behavior depends on the value of <em>dwFlags</em>:
        /// </p>
        /// <ul><li>
        /// <p>
        /// If <em>dwFlags</em> is 0, the method blocks indefinitely until a new event is queued, or until the event
        /// generator is shut down.
        /// </p>
        /// </li><li>
        /// <p>If <em>dwFlags</em> is MF_EVENT_FLAG_NO_WAIT, the method fails immediately with the return code
        /// <see cref="ResultCode.NoEventsAvailable"/>.</p>
        /// </li></ul>
        /// <p>
        /// This method returns <see cref="ResultCode.MultipleSubscribers"/> if you previously called <strong><see
        /// cref="BeginGetEvent(IAsyncCallback, object?)"/></strong> and have not yet called <strong><see cref="EndGetEvent"/></strong>.
        /// </p>
        /// </remarks>
        /// <unmanaged>
        /// HRESULT IMFMediaEventGenerator::GetEvent([In] unsigned int dwFlags,[Out] IMFMediaEvent** ppEvent)
        /// </unmanaged>
        /// <unmanaged-short>IMFMediaEventGenerator::GetEvent</unmanaged-short>
        public MediaEvent GetEvent(bool isBlocking)
        {
            GetEvent(isBlocking ? 0 : 1, out MediaEvent mediaEvent);
            return mediaEvent;
        }

        /// <summary>
        /// Begins an asynchronous request for the next event in the queue.
        /// </summary>
        /// <param name="callback">
        /// Pointer to the <strong><see cref="IAsyncCallback"/></strong> interface of a callback object. The client must
        /// implement this interface.
        /// </param>
        /// <param name="stateObject">
        /// A reference to a state object, defined by the caller. This parameter can be <strong><c>null</c></strong>.
        /// You can use this object to hold state information. The object is returned to the caller when the callback is invoked.
        /// </param>
        /// <remarks>
        /// <p>
        /// When a new event is available, the event generator calls the <strong><see
        /// cref="IAsyncCallback.Invoke"/></strong> method. The <strong>Invoke</strong> method should call <strong><see
        /// cref="EndGetEvent"/></strong> to get a reference to the <strong><see cref="MediaEvent"/></strong> interface,
        /// and use that interface to examine the event.
        /// </p>
        /// <p>
        /// Do not call <strong>BeginGetEvent</strong> a second time before calling <strong>EndGetEvent</strong>. While
        /// the first call is still pending, additional calls to the same object will fail. Also, the <strong><see
        /// cref="GetEvent(bool)"/></strong> method fails if an asynchronous request is still pending.
        /// </p>
        /// </remarks>
        /// <unmanaged>
        /// HRESULT IMFMediaEventGenerator::BeginGetEvent([In] IMFAsyncCallback* pCallback,[In] void* punkState)
        /// </unmanaged>
        /// <unmanaged-short>IMFMediaEventGenerator::BeginGetEvent</unmanaged-short>
        public void BeginGetEvent(IAsyncCallback callback, object? stateObject)
        {
            BeginGetEvent(callback, stateObject == null ? IntPtr.Zero : Marshal.GetIUnknownForObject(stateObject));
        }
    }
}
