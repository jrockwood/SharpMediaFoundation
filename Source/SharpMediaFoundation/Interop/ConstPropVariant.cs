// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="ConstPropVariant.cs" company="Justin Rockwood">
//   Copyright (c) Justin Rockwood. All Rights Reserved. Licensed under the Apache License, Version 2.0. See
//   LICENSE.txt in the project root for license information.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------------

namespace SharpMediaFoundation.Interop
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Text;

    /// <summary>
    /// Represents a constant representation of the native PROPVARIANT structure.
    /// </summary>
    /// <remarks>
    /// This is used for [In] parameters, which is important since for [In] parameters you must NOT clear the
    /// PropVariant. The caller will need to do that. To store a copy of a <see cref="ConstPropVariant"/>, store it in
    /// <see cref="PropVariant"/> using <see cref="PropVariant(ConstPropVariant)"/>. If you try to store the <see
    /// cref="ConstPropVariant"/> when the caller frees its copy, yours will no longer be valid.
    /// </remarks>
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [StructLayout(LayoutKind.Explicit)]
    public class ConstPropVariant : IDisposable, IEquatable<ConstPropVariant>
    {
        //// ===========================================================================================================
        //// Member Variables
        //// ===========================================================================================================

        [FieldOffset(0)]
        protected VariantType type;

        [FieldOffset(2)]
        protected short reserved1;

        [FieldOffset(4)]
        protected short reserved2;

        [FieldOffset(6)]
        protected short reserved3;

        [FieldOffset(8)]
        protected short iVal;

        [CLSCompliant(false)]
        [FieldOffset(8)]
        protected ushort uiVal;

        [FieldOffset(8)]
        protected byte bVal;

        [FieldOffset(8)]
        protected int intValue;

        [CLSCompliant(false)]
        [FieldOffset(8)]
        protected uint uintVal;

        [FieldOffset(8)]
        protected float fltVal;

        [FieldOffset(8)]
        protected long longValue;

        [CLSCompliant(false)]
        [FieldOffset(8)]
        protected ulong ulongValue;

        [FieldOffset(8)]
        protected double doubleValue;

        [FieldOffset(8)]
        protected Blob blobValue;

        [FieldOffset(8)]
        protected IntPtr ptr;

        [FieldOffset(8)]
        protected CalpWstr calpwstrVal;

        //// ===========================================================================================================
        //// Constructors
        //// ===========================================================================================================

        public ConstPropVariant(VariantType variantType = VariantType.None)
        {
            type = variantType;
        }

        //// ===========================================================================================================
        //// Operators
        //// ===========================================================================================================

        public static bool operator ==(ConstPropVariant? left, ConstPropVariant? right)
        {
            return left?.Equals(right) ?? right == null;
        }

        public static bool operator !=(ConstPropVariant? left, ConstPropVariant? right)
        {
            return !(left == right);
        }

        public static explicit operator string?(ConstPropVariant variant)
        {
            return variant.GetString();
        }

        public static explicit operator string[](ConstPropVariant variant)
        {
            return variant.GetStringArray();
        }

        public static explicit operator byte(ConstPropVariant variant)
        {
            return variant.GetUByte();
        }

        public static explicit operator short(ConstPropVariant variant)
        {
            return variant.GetShort();
        }

        [CLSCompliant(false)]
        public static explicit operator ushort(ConstPropVariant variant)
        {
            return variant.GetUShort();
        }

        public static explicit operator int(ConstPropVariant variant)
        {
            return variant.GetInt();
        }

        [CLSCompliant(false)]
        public static explicit operator uint(ConstPropVariant variant)
        {
            return variant.GetUInt();
        }

        public static explicit operator float(ConstPropVariant variant)
        {
            return variant.GetFloat();
        }

        public static explicit operator double(ConstPropVariant variant)
        {
            return variant.GetDouble();
        }

        public static explicit operator long(ConstPropVariant variant)
        {
            return variant.GetLong();
        }

        [CLSCompliant(false)]
        public static explicit operator ulong(ConstPropVariant variant)
        {
            return variant.GetULong();
        }

        public static explicit operator Guid(ConstPropVariant variant)
        {
            return variant.GetGuid();
        }

        public static explicit operator byte[](ConstPropVariant variant)
        {
            return variant.GetBlob();
        }

        // We aren't adding implicit casts since perf is likely to be better recycling the PropVariant, and the only way
        // to support Implicit is to create a new PropVariant. Also, since we can't free the previous instance,
        // IUnknowns will linger until the GC cleans up. Not what we want.

        //// ===========================================================================================================
        //// Properties
        //// ===========================================================================================================

        public VariantType VariantType => type;

        //// ===========================================================================================================
        //// Structs
        //// ===========================================================================================================

        [StructLayout(LayoutKind.Sequential), UnmanagedName("BLOB")]
        protected struct Blob
        {
            public int cbSize;
            public IntPtr pBlobData;
        }

        [StructLayout(LayoutKind.Sequential), UnmanagedName("CALPWSTR")]
        protected struct CalpWstr
        {
            public int cElems;
            public IntPtr pElems;
        }

        //// ===========================================================================================================
        //// Methods
        //// ===========================================================================================================

        public MFAttributeType ToMFAttributeType()
        {
            var e = new Exception($"Variant type {type} does not have a {nameof(MFAttributeType)} equivalent.");

            return type switch
            {
                VariantType.None => MFAttributeType.None,
                VariantType.Short => throw e,
                VariantType.Int32 => throw e,
                VariantType.Float => throw e,
                VariantType.Double => MFAttributeType.Double,
                VariantType.IUnknown => MFAttributeType.IUnknown,
                VariantType.UByte => throw e,
                VariantType.UShort => throw e,
                VariantType.UInt32 => MFAttributeType.UInt32,
                VariantType.Int64 => throw e,
                VariantType.UInt64 => MFAttributeType.UInt64,
                VariantType.String => MFAttributeType.String,
                VariantType.Guid => MFAttributeType.Guid,
                VariantType.Blob => MFAttributeType.Blob,
                VariantType.StringArray => throw e,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public string[] GetStringArray()
        {
            ThrowIfNotType(VariantType.StringArray);

            int count = calpwstrVal.cElems;
            string[] array = new string[count];
            for (int i = 0; i < count; i++)
            {
                array[i] = Marshal.PtrToStringUni(Marshal.ReadIntPtr(calpwstrVal.pElems), i * IntPtr.Size);
            }

            return array;
        }

        public string? GetString()
        {
            ThrowIfNotType(VariantType.String);
            return Marshal.PtrToStringUni(ptr);
        }

        public byte GetUByte()
        {
            ThrowIfNotType(VariantType.UByte);
            return bVal;
        }

        public short GetShort()
        {
            ThrowIfNotType(VariantType.Short);
            return iVal;
        }

        [CLSCompliant(false)]
        public ushort GetUShort()
        {
            ThrowIfNotType(VariantType.UShort);
            return uiVal;
        }

        public int GetInt()
        {
            ThrowIfNotType(VariantType.Int32);
            return intValue;
        }

        [CLSCompliant(false)]
        public uint GetUInt()
        {
            ThrowIfNotType(VariantType.UInt32);
            return uintVal;
        }

        public long GetLong()
        {
            ThrowIfNotType(VariantType.Int64);
            return longValue;
        }

        [CLSCompliant(false)]
        public ulong GetULong()
        {
            ThrowIfNotType(VariantType.UInt64);
            return ulongValue;
        }

        public float GetFloat()
        {
            ThrowIfNotType(VariantType.Float);
            return fltVal;
        }

        public double GetDouble()
        {
            ThrowIfNotType(VariantType.Double);
            return doubleValue;
        }

        public Guid GetGuid()
        {
            ThrowIfNotType(VariantType.Guid);
            return Marshal.PtrToStructure<Guid>(ptr);
        }

        public byte[] GetBlob()
        {
            ThrowIfNotType(VariantType.Blob);

            byte[] blob = new byte[blobValue.cbSize];
            if (blobValue.cbSize > 0)
            {
                Marshal.Copy(blobValue.pBlobData, blob, 0, blobValue.cbSize);
            }

            return blob;
        }

        public T GetBlob<T>() where T : struct
        {
            ThrowIfNotType(VariantType.Blob);

            if (blobValue.cbSize != Marshal.SizeOf<T>())
            {
                throw new InvalidOperationException("Size of the structure doesn't match the size of stored value");
            }

            return Marshal.PtrToStructure<T>(blobValue.pBlobData);
        }

        public object? GetIUnknown()
        {
            ThrowIfNotType(VariantType.IUnknown);
            return ptr == IntPtr.Zero ? null : Marshal.GetObjectForIUnknown(ptr);
        }

        public void Copy(PropVariant destination)
        {
            destination.Clear();
            PropVariantCopy(destination, this);
        }

        public override string ToString()
        {
            string s;

            switch (type)
            {
                case VariantType.None:
                    s = "<Empty>";
                    break;

                case VariantType.Short:
                    s = GetShort().ToString(CultureInfo.InvariantCulture);
                    break;

                case VariantType.Int32:
                    s = GetInt().ToString(CultureInfo.InvariantCulture);
                    break;

                case VariantType.Float:
                    s = GetFloat().ToString(CultureInfo.InvariantCulture);
                    break;

                case VariantType.Double:
                    s = GetDouble().ToString(CultureInfo.InvariantCulture);
                    break;

                case VariantType.IUnknown:
                    s = GetIUnknown()?.ToString() ?? "<null IUnknown>";
                    break;

                case VariantType.UByte:
                    s = GetUByte().ToString(CultureInfo.InvariantCulture);
                    break;

                case VariantType.UShort:
                    s = GetUShort().ToString(CultureInfo.InvariantCulture);
                    break;

                case VariantType.UInt32:
                    s = GetUInt().ToString(CultureInfo.InvariantCulture);
                    break;

                case VariantType.Int64:
                    s = GetLong().ToString(CultureInfo.InvariantCulture);
                    break;

                case VariantType.UInt64:
                    s = GetULong().ToString(CultureInfo.InvariantCulture);
                    break;

                case VariantType.String:
                    s = GetString() ?? "<null string>";
                    break;

                case VariantType.Guid:
                    s = GetGuid().ToString();
                    break;

                case VariantType.Blob:
                    {
                        byte[] blob = GetBlob();
                        int maxBytesToShow = Math.Min(16, blob.Length);
                        if (maxBytesToShow == 0)
                        {
                            s = "<Empty Blob>";
                        }
                        else
                        {
                            var builder = new StringBuilder();
                            for (int i = 0; i < maxBytesToShow; i++)
                            {
                                if (i > 0)
                                {
                                    builder.Append(",");
                                }

                                builder.Append(blob[i].ToString("x2", CultureInfo.InvariantCulture));
                            }

                            if (blob.Length > maxBytesToShow)
                            {
                                builder.Append("...");
                            }

                            s = builder.ToString();
                        }
                    }
                    break;

                case VariantType.StringArray:
                    {
                        var builder = new StringBuilder();
                        foreach (string entry in GetStringArray())
                        {
                            if (builder.Length > 0)
                            {
                                builder.Append(", ");
                            }

                            builder.Append("\"").Append(entry).Append("\"");
                        }

                        s = builder.ToString();
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            return s;
        }

        public override int GetHashCode()
        {
            int hash = type switch
            {
                VariantType.None => 0,
                VariantType.Short => GetShort().GetHashCode(),
                VariantType.Int32 => GetInt().GetHashCode(),
                VariantType.Float => GetFloat().GetHashCode(),
                VariantType.Double => GetDouble().GetHashCode(),
                VariantType.IUnknown => GetIUnknown()?.GetHashCode() ?? "IUnknown".GetHashCode(),
                VariantType.UByte => GetUByte().GetHashCode(),
                VariantType.UShort => GetUShort().GetHashCode(),
                VariantType.UInt32 => GetUInt().GetHashCode(),
                VariantType.Int64 => GetLong().GetHashCode(),
                VariantType.UInt64 => GetULong().GetHashCode(),
                VariantType.String => GetString()?.GetHashCode() ?? "null".GetHashCode(),
                VariantType.Guid => GetGuid().GetHashCode(),
                VariantType.Blob => GetBlob().GetHashCode(),
                VariantType.StringArray => GetStringArray().GetHashCode(),
                _ => throw new ArgumentOutOfRangeException()
            };

            return hash;
        }

        public override bool Equals(object? obj)
        {
            return obj is PropVariant other && Equals(other);
        }

        [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
        public bool Equals(ConstPropVariant? other)
        {
            if (type != other?.type)
            {
                return false;
            }

            static bool AreArraysEqual<T>(T[] b1, T[] b2) where T : notnull
            {
                if (b1.Length != b2.Length)
                {
                    return false;
                }

                for (int i = 0; i < b1.Length; i++)
                {
                    if (!b1[i].Equals(b2[i]))
                    {
                        return false;
                    }
                }

                return true;
            }

            return type switch
            {
                VariantType.None => true,
                VariantType.Short => GetShort() == other.GetShort(),
                VariantType.Int32 => GetInt() == other.GetInt(),
                VariantType.Float => GetFloat() == other.GetFloat(),
                VariantType.Double => GetDouble() == other.GetDouble(),
                VariantType.IUnknown => GetIUnknown() == other.GetIUnknown(),
                VariantType.UByte => GetUByte() == other.GetUByte(),
                VariantType.UShort => GetUShort() == other.GetShort(),
                VariantType.UInt32 => GetUInt() == other.GetUInt(),
                VariantType.Int64 => GetLong() == other.GetLong(),
                VariantType.UInt64 => GetULong() == other.GetULong(),
                VariantType.String => GetString() == other.GetString(),
                VariantType.Guid => GetGuid() == other.GetGuid(),
                VariantType.Blob => AreArraysEqual(GetBlob(), other.GetBlob()),
                VariantType.StringArray => AreArraysEqual(GetStringArray(), other.GetStringArray()),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            // A ConstPropVariant must NOT call PropVariantClear. That would release the caller's copy of the data. If
            // we are a PropVariant, the PropVariant.Dispose gets called as well, which DOES do a PropVariantClear.
            type = VariantType.None;
#if DEBUG
            longValue = 0;
#endif
        }

        [SuppressUnmanagedCodeSecurity]
        [DllImport("ole32.dll", ExactSpelling = true, PreserveSig = false)]
        protected static extern void PropVariantCopy(
            [Out, MarshalAs(UnmanagedType.LPStruct)]
            PropVariant pvarDest,
            [In, MarshalAs(UnmanagedType.LPStruct)]
            ConstPropVariant pvarSource);

        private void ThrowIfNotType(VariantType expectedType)
        {
            if (type != expectedType)
            {
                throw new InvalidOperationException($"The variant structure does not contain a value of type {type}");
            }
        }
    }
}
