// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="VariantType.cs" company="Justin Rockwood">
//   Copyright (c) Justin Rockwood. All Rights Reserved. Licensed under the Apache License, Version 2.0. See
//   LICENSE.txt in the project root for license information.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------------

namespace SharpMediaFoundation.Interop
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Defines the different types of <see cref="ConstPropVariant"/> or <see cref="PropVariant"/>.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum VariantType : short
    {
        None = 0,
        Short = 2,
        Int32 = 3,
        Float = 4,
        Double = 5,
        IUnknown = 13,
        UByte = 17,
        UShort = 18,
        UInt32 = 19,
        Int64 = 20,
        UInt64 = 21,
        String = 31,
        Guid = 72,
        Blob = 0x1000 + 17,
        StringArray = 0x1000 + 31
    }
}
