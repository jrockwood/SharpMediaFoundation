// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncResult.cs" company="Justin Rockwood">
//   Copyright (c) Justin Rockwood. All Rights Reserved. Licensed under the Apache License, Version 2.0. See
//   LICENSE.txt in the project root for license information.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------------

namespace SharpMediaFoundation
{
    using System;
    using System.Runtime.InteropServices;
    using SharpGen.Runtime;

    public partial class AsyncResult
    {
        //// ===========================================================================================================
        //// Member Variables
        //// ===========================================================================================================

        private object? _state;
        private bool _isStateVerified;

        //// ===========================================================================================================
        //// Properties
        //// ===========================================================================================================

        /// <summary>
        /// Gets the state object specified by the caller in the asynchronous <strong>Begin</strong> method. If the
        /// value is not <strong><c>null</c></strong>, the caller must dispose.
        /// </summary>
        /// <value>The state.</value>
        /// <remarks>
        /// <p>
        /// The caller of the asynchronous method specifies the state object, and can use it for any caller-defined
        /// purpose. The state object can be <strong><c>null</c></strong>. If the state object is
        /// <strong><c>null</c></strong>, <strong>GetState</strong> returns <strong>E_POINTER</strong>.
        /// </p>
        /// <p>
        /// If you are implementing an asynchronous method, set the state object on the through the <em>punkState</em>
        /// parameter of the <strong><see cref="MediaFactory.CreateAsyncResult"/></strong> function.
        /// </p>
        /// </remarks>
        /// <unmanaged>HRESULT IMFAsyncResult::GetState([Out] IUnknown** ppunkState)</unmanaged>
        /// <unmanaged-short>IMFAsyncResult::GetState</unmanaged-short>
        public object? State
        {
            get
            {
                if (!_isStateVerified)
                {
                    GetState(out IntPtr statePtr);
                    if (statePtr != IntPtr.Zero)
                    {
                        _state = Marshal.GetObjectForIUnknown(statePtr);
                        Marshal.Release(statePtr);
                    }
                    _isStateVerified = true;
                }
                return _state;
            }
        }

        /// <summary>
        /// Gets or sets the status of the asynchronous operation.
        /// </summary>
        /// <value>
        /// <p>
        /// The method returns an <strong><see cref="Result"/></strong>. Possible values include, but are not limited
        /// to, those in the following table.
        /// </p>
        /// <table><tr><th>Return code</th><th>Description</th></tr><tr><td><dl><dt><strong><see cref="Result.Ok"/></strong></dt></dl></td><td>
        /// <p>The operation completed successfully.</p>
        /// </td></tr></table>
        /// <p>?</p>
        /// </value>
        /// <unmanaged>HRESULT IMFAsyncResult::GetStatus()</unmanaged>
        /// <unmanaged-short>IMFAsyncResult::GetStatus</unmanaged-short>
        public Result Status
        {
            get => GetStatus();
            set => SetStatus(value);
        }

        /// <summary>
        /// Returns an object associated with the asynchronous operation. The type of object, if any, depends on the
        /// asynchronous method that was called.
        /// </summary>
        /// <value>
        /// Receives a reference to the object's <strong><see cref="ComObject"/></strong> interface. If no object is
        /// associated with the operation, this parameter receives the value <strong><c>null</c></strong>. If the value
        /// is not <strong><c>null</c></strong>, the caller must release the interface.
        /// </value>
        /// <remarks>
        /// <p>
        /// Typically, this object is used by the component that implements the asynchronous method. It provides a way
        /// for the function that invokes the callback to pass information to the asynchronous <strong>End...</strong>
        /// method that completes the operation.
        /// </p>
        /// <p>
        /// If you are implementing an asynchronous method, you can set the object through the <em>punkObject</em>
        /// parameter of the <strong><see cref="MediaFactory.CreateAsyncResult"/></strong> function.
        /// </p>
        /// <p>
        /// If the asynchronous result object's internal <strong><see cref="ComObject"/></strong> reference is
        /// <strong><c>null</c></strong>, the method returns <strong>E_POINTER</strong>.
        /// </p>
        /// </remarks>
        /// <unmanaged>HRESULT IMFAsyncResult::GetObjectW([Out] IUnknown** ppObject)</unmanaged>
        /// <unmanaged-short>IMFAsyncResult::GetObjectW</unmanaged-short>
        public IUnknown? PrivateObject
        {
            get
            {
                GetObject(out IUnknown privateObject);
                return privateObject;
            }
        }
    }
}
