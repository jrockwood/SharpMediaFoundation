// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="IMFMediaEvent.cs" company="Justin Rockwood">
//   Copyright (c) Justin Rockwood. All Rights Reserved. Licensed under the Apache License, Version 2.0. See
//   LICENSE.txt in the project root for license information.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------------

namespace SharpMediaFoundation.Interop
{
    using System.Runtime.InteropServices;
    using System.Security;

    [ComImport, SuppressUnmanagedCodeSecurity]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("DF598932-F10C-4E39-BBA2-C308F101DAA3")]
    public interface IMFMediaEvent : IMFAttributes
    {
    }
}
