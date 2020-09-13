// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="ClassNamePlaceholder.cs" company="Justin Rockwood">
//   Copyright (c) Justin Rockwood. All Rights Reserved. Licensed under the Apache License, Version 2.0. See
//   LICENSE.txt in the project root for license information.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------------

namespace SharpMediaFoundation
{
    using SharpGen.Runtime;

    public static class MediaManager
    {
        //// ===========================================================================================================
        //// Member Variables
        //// ===========================================================================================================

        private static bool s_hasStarted;

        //// ===========================================================================================================
        //// Methods
        //// ===========================================================================================================

        /// <summary>
        /// Initializes Microsoft Media Foundation.
        /// </summary>
        /// <param name="useLightVersion">
        /// If true, do not initialize the sockets library, else full initialization. Default is false.
        /// </param>
        /// <unmanaged>HRESULT MFStartup([In] unsigned int Version,[In] unsigned int dwFlags)</unmanaged>
        /// <unmanaged-short>MFStartup</unmanaged-short>
        /// <remarks>
        /// <p>
        /// An application must call this function before using Media Foundation. Before your application quits, call
        /// <strong><see cref="MediaFactory.Shutdown"/></strong> once for every previous call to <strong><see cref="MediaFactory.Startup"/></strong>.
        /// </p>
        /// <p>
        /// Do not call <strong><see cref="MediaFactory.Startup"/></strong> or <strong><see
        /// cref="MediaFactory.Shutdown"/></strong> from work queue threads. For more information about work queues, see
        /// Work Queues.
        /// </p>
        /// </remarks>
        public static void Startup(bool useLightVersion = false)
        {
            if (s_hasStarted)
            {
                return;
            }

            MediaFactory.Startup(MediaFactory.Version, useLightVersion ? 1 : 0);
            s_hasStarted = useLightVersion;
        }

        /// <summary>
        /// Shuts down the Microsoft Media Foundation platform. Call this function once for every call to <strong><see
        /// cref="MediaFactory.Startup"/></strong>. Do not call this function from work queue threads.
        /// </summary>
        /// <returns>
        /// <p>
        /// If this function succeeds, it returns <strong><see cref="Result.Ok"/></strong>. Otherwise, it returns an
        /// <strong><see cref="Result"/></strong> error code.
        /// </p>
        /// </returns>
        /// <unmanaged>HRESULT MFShutdown()</unmanaged>
        /// <unmanaged-short>MFShutdown</unmanaged-short>
        public static void Shutdown()
        {
            if (!s_hasStarted)
            {
                return;
            }

            MediaFactory.Shutdown();
            s_hasStarted = false;
        }
    }
}
