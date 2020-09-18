// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="NativeMethods.cs" company="Justin Rockwood">
//   Copyright (c) Justin Rockwood. All Rights Reserved. Licensed under the Apache License, Version 2.0. See
//   LICENSE.txt in the project root for license information.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------------

namespace SharpMediaFoundation.Interop
{
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Contains definitions for interop methods.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    internal static class NativeMethods
    {
        //// ===========================================================================================================
        //// Constants
        //// ===========================================================================================================

        public const uint MF_VERSION = (MF_SDK_VERSION << 16) | MF_API_VERSION;
        private const uint MF_SDK_VERSION = 0x0002;
        private const uint MF_API_VERSION = 0x0070;

        public const int MFSTARTUP_NOSOCKET = 0x1;
        public const int MFSTARTUP_LITE = MFSTARTUP_NOSOCKET;
        public const int MFSTARTUP_FULL = 0;

        //// ===========================================================================================================
        //// Functions
        //// ===========================================================================================================

        private const string MfPlat = "Mfplat.dll";

        [DllImport(MfPlat, CallingConvention = CallingConvention.StdCall)]
        public static extern int MFShutdown();

        [DllImport(MfPlat, CallingConvention = CallingConvention.StdCall)]
        public static extern int MFStartup(uint version, int dwFlags);
    }
}
