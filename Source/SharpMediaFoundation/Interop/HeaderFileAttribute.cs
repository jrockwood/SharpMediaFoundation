// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="HeaderFileAttribute.cs" company="Justin Rockwood">
//   Copyright (c) Justin Rockwood. All Rights Reserved. Licensed under the Apache License, Version 2.0. See
//   LICENSE.txt in the project root for license information.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------------

namespace SharpMediaFoundation.Interop
{
    using System;

    /// <summary>
    /// Attribute to indicate which header in the Media Foundation the definition comes from. Informational only.
    /// </summary>
    public sealed class HeaderFileAttribute : Attribute
    {
        public HeaderFileAttribute(string headerFile)
        {
            HeaderFile = headerFile;
        }

        public string HeaderFile { get; }
    }
}
