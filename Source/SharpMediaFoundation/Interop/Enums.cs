// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="Enums.cs" company="Justin Rockwood">
//   Copyright (c) Justin Rockwood. All Rights Reserved. Licensed under the Apache License, Version 2.0. See
//   LICENSE.txt in the project root for license information.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------------

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global

namespace SharpMediaFoundation.Interop
{
    [UnmanagedName("MF_ATTRIBUTE_TYPE"), HeaderFile("mfobjects.h")]
    public enum MFAttributeType
    {
        None = 0x0,
        Blob = 0x1011,
        Double = 0x5,
        Guid = 0x48,
        IUnknown = 13,
        String = 0x1f,
        UInt32 = 0x13,
        UInt64 = 0x15
    }

    [UnmanagedName("MF_ATTRIBUTES_MATCH_TYPE"), HeaderFile("mfobjects.h")]
    public enum MFAttributesMatchType
    {
        OurItems,
        TheirItems,
        AllItems,
        Intersection,
        Smaller,
    }
}
