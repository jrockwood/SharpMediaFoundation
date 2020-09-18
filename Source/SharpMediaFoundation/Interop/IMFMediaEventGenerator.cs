// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="IMFMediaEventGenerator.cs" company="Justin Rockwood">
//   Copyright (c) Justin Rockwood. All Rights Reserved. Licensed under the Apache License, Version 2.0. See
//   LICENSE.txt in the project root for license information.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------------

namespace SharpMediaFoundation.Interop
{
    using System.Runtime.InteropServices;
    using System.Security;

    /// <summary>
    /// Retrieves events from any Media Foundation object that generates events.
    /// </summary>
    [ComImport, SuppressUnmanagedCodeSecurity]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("2CD0BD52-BCD5-4B89-B62C-EADC0C031E7D")]
    public interface IMFMediaEventGenerator
    {
        int GetEvent(int dwFlags, out IMFMediaEvent ppEvent);
    }
}
