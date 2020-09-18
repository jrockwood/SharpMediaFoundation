// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="UnmanagedNameAttribute.cs" company="Justin Rockwood">
//   Copyright (c) Justin Rockwood. All Rights Reserved. Licensed under the Apache License, Version 2.0. See
//   LICENSE.txt in the project root for license information.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------------

namespace SharpMediaFoundation.Interop
{
    using System;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum)]
    internal sealed class UnmanagedNameAttribute : Attribute
    {
        public UnmanagedNameAttribute(string unmanagedName)
        {
            UnmanagedName = unmanagedName;
        }

        public string UnmanagedName { get; }

        public override string ToString()
        {
            return UnmanagedName;
        }
    }
}
