// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncCallbackShadow.cs" company="Justin Rockwood">
//   Copyright (c) Justin Rockwood. All Rights Reserved. Licensed under the Apache License, Version 2.0. See
//   LICENSE.txt in the project root for license information.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------------

namespace SharpMediaFoundation
{
    using System;
    using System.Runtime.InteropServices;
    using SharpGen.Runtime;

    internal class AsyncCallbackShadow : ComObjectShadow
    {
        private static readonly AsyncCallbackVtbl s_vtbl = new AsyncCallbackVtbl();

        /// <summary>
        /// Return a pointer to the unmanaged version of this callback.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <returns>A pointer to a shadow c++ callback</returns>
        public static IntPtr ToIntPtr(IAsyncCallback callback)
        {
            return ToCallbackPtr<IAsyncCallback>(callback);
        }

        protected class AsyncCallbackVtbl : ComObjectVtbl
        {
            public AsyncCallbackVtbl() : base(2)
            {
                AddMethod(new GetParametersDelegate(GetParametersImpl));
                AddMethod(new InvokeDelegate(InvokeImpl));
            }

            /// <unmanaged>HRESULT IMFAsyncCallback::GetParameters([Out] MFASYNC_CALLBACK_FLAGS* pdwFlags,[Out] unsigned int* pdwQueue)</unmanaged>
            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            private delegate int GetParametersDelegate(IntPtr thisPtr, out AsyncCallbackFlags flags, out WorkQueueId workQueueId);

            private static int GetParametersImpl(IntPtr thisPtr, out AsyncCallbackFlags flags, out WorkQueueId workQueueId)
            {
                flags = AsyncCallbackFlags.None;
                workQueueId = WorkQueueId.Standard;
                try
                {
                    var shadow = ToShadow<AsyncCallbackShadow>(thisPtr);
                    var callback = (IAsyncCallback)shadow.Callback;
                    workQueueId = callback.WorkQueueId;
                    flags = callback.Flags;
                }
                catch (Exception exception)
                {
                    return (int)Result.GetResultFromException(exception);
                }
                return Result.Ok.Code;
            }

            /// <unmanaged>HRESULT IMFAsyncCallback::Invoke([In, Optional] IMFAsyncResult* pAsyncResult)</unmanaged>
            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            private delegate int InvokeDelegate(IntPtr thisPtr, IntPtr asyncResult);

            private static int InvokeImpl(IntPtr thisPtr, IntPtr asyncResultPtr)
            {
                AsyncResult? asyncResult = null;
                try
                {
                    var shadow = ToShadow<AsyncCallbackShadow>(thisPtr);
                    var callback = (IAsyncCallback)shadow.Callback;
                    asyncResult = new AsyncResult(asyncResultPtr);
                    callback.Invoke(asyncResult);
                }
                catch (Exception exception)
                {
                    return (int)Result.GetResultFromException(exception);
                }
                finally
                {
                    // Clear the NativePointer to make sure that there will be no false indication from ObjectTracker
                    if (asyncResult != null)
                    {
                        asyncResult.NativePointer = IntPtr.Zero;
                    }
                }
                return Result.Ok.Code;
            }
        }

        protected override CppObjectVtbl Vtbl => s_vtbl;
    }
}
