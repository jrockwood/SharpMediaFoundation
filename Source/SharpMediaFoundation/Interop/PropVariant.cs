// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="PropVariant.cs" company="Justin Rockwood">
//   Copyright (c) Justin Rockwood. All Rights Reserved. Licensed under the Apache License, Version 2.0. See
//   LICENSE.txt in the project root for license information.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------------

namespace SharpMediaFoundation.Interop
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security;

    /// <summary>
    /// Represents a mutable representation of the native PROPVARIANT structure.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public class PropVariant : ConstPropVariant
    {
        //// ===========================================================================================================
        //// Constructors
        //// ===========================================================================================================

        public PropVariant()
            : base(VariantType.None)
        {
        }

        public PropVariant(string value)
            : base(VariantType.String)
        {
            ptr = Marshal.StringToCoTaskMemUni(value);
        }

        public PropVariant(string[] value)
            : base(VariantType.StringArray)
        {
            calpwstrVal.cElems = value.Length;
            calpwstrVal.pElems = Marshal.AllocCoTaskMem(IntPtr.Size * value.Length);

            for (int i = 0; i < value.Length; i++)
            {
                Marshal.WriteIntPtr(calpwstrVal.pElems, i * IntPtr.Size, Marshal.StringToCoTaskMemUni(value[i]));
            }
        }

        public PropVariant(byte value)
            : base(VariantType.UByte)
        {
            bVal = value;
        }

        public PropVariant(short value)
            : base(VariantType.Short)
        {
            iVal = value;
        }

        [CLSCompliant(false)]
        public PropVariant(ushort value)
            : base(VariantType.UShort)
        {
            uiVal = value;
        }

        public PropVariant(int value)
            : base(VariantType.Int32)
        {
            intValue = value;
        }

        [CLSCompliant(false)]
        public PropVariant(uint value)
            : base(VariantType.UInt32)
        {
            uintVal = value;
        }

        public PropVariant(float value)
            : base(VariantType.Float)
        {
            fltVal = value;
        }

        public PropVariant(double value)
            : base(VariantType.Double)
        {
            doubleValue = value;
        }

        public PropVariant(long value)
            : base(VariantType.Int64)
        {
            longValue = value;
        }

        [CLSCompliant(false)]
        public PropVariant(ulong value)
            : base(VariantType.UInt64)
        {
            ulongValue = value;
        }

        public PropVariant(Guid value)
            : base(VariantType.Guid)
        {
            ptr = Marshal.AllocCoTaskMem(Marshal.SizeOf(value));
            Marshal.StructureToPtr(value, ptr, fDeleteOld: false);
        }

        public PropVariant(byte[] value)
            : base(VariantType.Blob)
        {
            blobValue.cbSize = value.Length;
            blobValue.pBlobData = Marshal.AllocCoTaskMem(value.Length);
            Marshal.Copy(value, 0, blobValue.pBlobData, value.Length);
        }

        public PropVariant(object? value)
            : base(VariantType.IUnknown)
        {
            if (value == null)
            {
                ptr = IntPtr.Zero;
            }
            else if (Marshal.IsComObject(value))
            {
                ptr = Marshal.GetIUnknownForObject(value);
            }
            else
            {
                type = VariantType.Blob;

                blobValue.cbSize = Marshal.SizeOf(value);
                blobValue.pBlobData = Marshal.AllocCoTaskMem(blobValue.cbSize);

                Marshal.StructureToPtr(value, blobValue.pBlobData, false);
            }
        }

        public PropVariant(IntPtr value)
            : base(VariantType.None)
        {
            Marshal.PtrToStructure(value, this);
        }

        public PropVariant(ConstPropVariant value)
        {
            PropVariantCopy(this, value);
        }

        ~PropVariant()
        {
            Dispose(false);
        }

        //// ===========================================================================================================
        //// Methods
        //// ===========================================================================================================

        public void Clear()
        {
            PropVariantClear(this);
        }

        protected override void Dispose(bool disposing)
        {
            Clear();
            if (disposing)
            {
                GC.SuppressFinalize(this);
            }
        }

        [SuppressUnmanagedCodeSecurity]
        [DllImport("ole32.dll", ExactSpelling = true, PreserveSig = false)]
        private static extern void PropVariantClear(
            [In, MarshalAs(UnmanagedType.LPStruct)]
            PropVariant pvar);
    }
}
