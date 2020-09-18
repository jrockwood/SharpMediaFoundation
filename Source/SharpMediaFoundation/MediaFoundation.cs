// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="MediaFoundation.cs" company="Justin Rockwood">
//   Copyright (c) Justin Rockwood. All Rights Reserved. Licensed under the Apache License, Version 2.0. See
//   LICENSE.txt in the project root for license information.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------------

namespace SharpMediaFoundation
{
    /// <summary>
    /// Contains startup and shutdown logic for the Microsoft Media Foundation.
    /// </summary>
    /// <remarks>
    /// The application should call <see cref="Startup"/> once before using any Media Foundation functionality and call
    /// <see cref="Shutdown"/> once before exiting. This class ensures that startup and shutdown are called only once.
    /// This class is NOT thread-safe, so these methods should normally be called from the UI thread.
    /// </remarks>
    public static class MediaFoundation
    {
        //// ===========================================================================================================
        //// Member Variables
        //// ===========================================================================================================

        private static bool s_hasStarted;

        //// ===========================================================================================================
        //// Methods
        //// ===========================================================================================================

        /// <summary>
        /// Initializes Microsoft Media Foundation. Do not call this function from work queue threads.
        /// </summary>
        /// <param name="useLightVersion">
        /// Indicates whether full initialization occurs or if light initialization occurs, which skips initialization
        /// of the sockets library.
        /// </param>
        /// <remarks>
        /// An application must call this function before using Media Foundation. Before your application quits, call
        /// <see cref="Shutdown"/>. Do not call <see cref="Startup"/> or <see cref="Shutdown"/> from work queue threads.
        /// </remarks>
        public static void Startup(bool useLightVersion = false)
        {
            if (s_hasStarted)
            {
                return;
            }

            MediaFactory.Startup(MediaFactory.Version, useLightVersion ? 1 : 0);
            s_hasStarted = true;
        }

        /// <summary>
        /// Shuts down the Microsoft Media Foundation platform. Do not call this function from work queue threads.
        /// </summary>
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
