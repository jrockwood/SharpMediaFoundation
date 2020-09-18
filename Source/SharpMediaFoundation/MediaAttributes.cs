// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="MediaAttributes.cs" company="Justin Rockwood">
//   Copyright (c) Justin Rockwood. All Rights Reserved. Licensed under the Apache License, Version 2.0. See
//   LICENSE.txt in the project root for license information.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------------

namespace SharpMediaFoundation
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using SharpGen.Runtime;
    using SharpGen.Runtime.Win32;

    public partial class MediaAttributes
    {
        //// ===========================================================================================================
        //// Constructors
        //// ===========================================================================================================

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaAttributes"/> class.
        /// </summary>
        /// <param name="initialSizeInBytes">
        /// The initial number of elements allocated for the attribute store. The attribute store grows as needed.
        /// Default is 0
        /// </param>
        /// <remarks>
        /// <p>
        /// Attributes are used throughout Microsoft Media Foundation to configure objects, describe media formats,
        /// query object properties, and other purposes. For more information, see Attributes in Media Foundation.
        /// </p>
        /// <p>For a complete list of all the defined attribute GUIDs in Media Foundation, see Media Foundation Attributes.</p>
        /// </remarks>
        /// <unmanaged>HRESULT MFCreateAttributes([Out] IMFAttributes** ppMFAttributes,[In] unsigned int cInitialSize)</unmanaged>
        /// <unmanaged-short>MFCreateAttributes</unmanaged-short>
        public MediaAttributes(int initialSizeInBytes = 0)
            : base(IntPtr.Zero)
        {
            MediaFactory.CreateAttributes(this, initialSizeInBytes);
        }

        //// ===========================================================================================================
        //// Methods
        //// ===========================================================================================================

        /// <summary>
        /// Gets an item value
        /// </summary>
        /// <param name="guidKey">GUID of the key.</param>
        /// <returns>The value associated to this key.</returns>
        /// <unmanaged>HRESULT IMFAttributes::GetItem([In] const GUID&amp; guidKey,[In] void* pValue)</unmanaged>
        /// <unmanaged-short>IMFAttributes::GetItem</unmanaged-short>
        public object Get(Guid guidKey)
        {
            var itemType = GetItemType(guidKey);
            return itemType switch
            {
                AttributeType.UInt32 => Get<int>(guidKey),
                AttributeType.UInt64 => Get<long>(guidKey),
                AttributeType.Double => Get<double>(guidKey),
                AttributeType.Guid => Get<Guid>(guidKey),
                AttributeType.Blob => Get<byte[]>(guidKey),
                AttributeType.String => Get<string>(guidKey),
                AttributeType.IUnknown => Get<ComObject>(guidKey),
                _ => throw new ArgumentException("The type of the value is not supported"),
            };
        }

        /// <summary>
        /// Retrieves an attribute at the specified index.
        /// </summary>
        /// <param name="index">
        /// Index of the attribute to retrieve. To get the number of attributes, call <strong><see cref="GetCount"/></strong>.
        /// </param>
        /// <param name="guidKey">
        /// Receives the <see cref="System.Guid"/> that identifies this attribute.
        /// </param>
        /// <returns>The value associated to this index</returns>
        /// <remarks>
        /// <p>To enumerate all of an object's attributes in a thread-safe way, do the following:</p>
        /// <ol><li>
        /// <p>
        /// Call <strong><see cref="LockStore"/></strong> to prevent another thread from adding or
        /// deleting attributes.
        /// </p>
        /// </li><li>
        /// <p>Call <strong><see cref="GetCount"/></strong> to find the number of attributes.</p>
        /// </li><li>
        /// <p>Call <strong>GetItemByIndex</strong> to get each attribute by index.</p>
        /// </li><li>
        /// <p>Call <strong><see cref="UnlockStore"/></strong> to unlock the attribute store.</p>
        /// </li></ol>
        /// </remarks>
        /// <unmanaged>
        /// HRESULT IMFAttributes::GetItemByIndex([In] unsigned int unIndex,[Out] GUID* pguidKey,[InOut, Optional]
        /// PROPVARIANT* pValue)
        /// </unmanaged>
        /// <unmanaged-short>IMFAttributes::GetItemByIndex</unmanaged-short>
        public object GetByIndex(int index, out Guid guidKey)
        {
            GetItemByIndex(index, out guidKey, IntPtr.Zero);
            return Get(guidKey);
        }

        /// <summary>
        /// Gets an item value
        /// </summary>
        /// <param name="guidKey">GUID of the key.</param>
        /// <returns>The value associated to this key.</returns>
        /// <unmanaged>HRESULT IMFAttributes::GetItem([In] const GUID&amp; guidKey,[In] void* pValue)</unmanaged>
        /// <unmanaged-short>IMFAttributes::GetItem</unmanaged-short>
        public unsafe T Get<T>(Guid guidKey)
        {
            // Perform conversions to supported types
            // int
            // long
            // string
            // byte[]
            // double
            // ComObject
            // Guid

            if (typeof(T) == typeof(int) ||
                typeof(T) == typeof(bool) ||
                typeof(T) == typeof(byte) ||
                typeof(T) == typeof(uint) ||
                typeof(T) == typeof(short) ||
                typeof(T) == typeof(ushort) ||
                typeof(T) == typeof(byte) ||
                typeof(T) == typeof(sbyte))
            {
                return (T)Convert.ChangeType(GetInt(guidKey), typeof(T));
            }

            if (typeof(T).GetTypeInfo().IsEnum)
            {
                return (T)Enum.ToObject(typeof(T), GetInt(guidKey));
            }

            if (typeof(T) == typeof(IntPtr))
            {
                return (T)(object)new IntPtr(GetLong(guidKey));
            }

            if (typeof(T) == typeof(long) || typeof(T) == typeof(ulong))
            {
                return (T)Convert.ChangeType(GetLong(guidKey), typeof(T));
            }

            if (typeof(T) == typeof(Guid))
            {
                return (T)(object)GetGuid(guidKey);
            }

            if (typeof(T) == typeof(string))
            {
                int length = GetStringLength(guidKey);
                char* wstr = stackalloc char[length + 1];
                GetString(guidKey, new IntPtr(wstr), length + 1, IntPtr.Zero);
                return (T)(object)Marshal.PtrToStringUni(new IntPtr(wstr));
            }

            if (typeof(T) == typeof(double) || typeof(T) == typeof(float))
            {
                return (T)Convert.ChangeType(GetDouble(guidKey), typeof(T));
            }

            if (typeof(T) == typeof(byte[]))
            {
                int length = GetBlobSize(guidKey);
                byte[] buffer = new byte[length];
                fixed (void* pBuffer = buffer)
                {
                    GetBlob(guidKey, (IntPtr)pBuffer, buffer.Length, IntPtr.Zero);
                }

                return (T)(object)buffer;
            }

            if (typeof(T).GetTypeInfo().IsValueType)
            {
                int length = GetBlobSize(guidKey);
                if (length != Marshal.SizeOf<T>())
                {
                    throw new ArgumentException("Size of the structure doesn't match the size of stored value");
                }

                var value = default(T);
                void* objPtr = Unsafe.AsPointer(ref value!);
                GetBlob(guidKey, (IntPtr)objPtr, Unsafe.SizeOf<T>(), IntPtr.Zero);

                return value;
            }

            if (typeof(T) == typeof(ComObject))
            {
                GetUnknown(guidKey, typeof(IUnknown).GUID, out IntPtr ptr);
                return (T)(object)new ComObject(ptr);
            }

            if (typeof(T).GetTypeInfo().IsSubclassOf(typeof(ComObject)))
            {
                GetUnknown(guidKey, typeof(T).GUID, out IntPtr ptr);
                using var tempObject = new ComObject(ptr);
                tempObject.QueryInterface(typeof(T).GUID, out IntPtr parentPtr);
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                return parentPtr == IntPtr.Zero ? (T)((object)null)! : (T)Activator.CreateInstance(typeof(T), parentPtr);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CS8603 // Possible null reference return.
            }

            throw new ArgumentException("The type of the value is not supported");
        }

        /// <summary>
        /// Gets an item value
        /// </summary>
        /// <param name="guidKey">GUID of the key.</param>
        /// <returns>The value associated to this key.</returns>
        /// <msdn-id>ms704598</msdn-id>
        /// <unmanaged>HRESULT IMFAttributes::GetItem([In] const GUID&amp; guidKey,[In] void* pValue)</unmanaged>
        /// <unmanaged-short>IMFAttributes::GetItem</unmanaged-short>
        public T Get<T>(MediaAttributeKey<T> guidKey)
        {
            return Get<T>(guidKey.Guid);
        }

        /// <summary>
        /// Adds an attribute value with a specified key.
        /// </summary>
        /// <param name="guidKey">
        /// A <see cref="Guid"/> that identifies the value to set. If this key already exists, the method overwrites the
        /// old value.
        /// </param>
        /// <param name="value">
        /// A <strong><see cref="Variant"/></strong> that contains the attribute value. The method copies the value. The
        /// <strong><see cref="Variant"/></strong> type must be one of the types listed in the <strong><see
        /// cref="AttributeType"/></strong> enumeration.
        /// </param>
        /// <returns>
        /// <p>
        /// The method returns an <strong><see cref="Result"/></strong>. Possible values include, but are not limited
        /// to, those in the following table.
        /// </p>
        /// <table><tr><th>Return code</th><th>Description</th></tr><tr><td><dl><dt><strong><see cref="Result.Ok"/></strong></dt></dl></td><td>
        /// <p>The method succeeded.</p>
        /// </td></tr><tr><td><dl><dt><strong>E_OUTOFMEMORY</strong></dt></dl></td><td>
        /// <p>Insufficient memory.</p>
        /// </td></tr><tr><td><dl><dt><strong>MF_E_INVALIDTYPE</strong></dt></dl></td><td>
        /// <p>Invalid attribute type.</p>
        /// </td></tr></table>
        /// </returns>
        /// <remarks>
        /// This method checks whether the <strong><see cref="Variant"/></strong> type is one of the attribute types
        /// defined in <strong><see cref="AttributeType"/></strong>, and fails if an unsupported type is used. However,
        /// this method does not check whether the <strong><see cref="Variant"/></strong> is the correct type for the
        /// specified attribute <see cref="Guid"/>. (There is no programmatic way to associate attribute GUIDs with
        /// property types.) For a list of Media Foundation attributes and their data types, see Media Foundation Attributes.
        /// </remarks>
        /// <unmanaged>
        /// HRESULT IMFAttributes::SetItem([In] const GUID&amp; guidKey,[In] const PROPVARIANT&amp; Value)
        /// </unmanaged>
        /// <unmanaged-short>IMFAttributes::SetItem</unmanaged-short>
        public unsafe void Set<T>(Guid guidKey, T value)
        {
            // Perform conversions to supported types
            // int
            // long
            // string
            // byte[]
            // double
            // ComObject
            // Guid

            if (typeof(T) == typeof(int) ||
                typeof(T) == typeof(bool) ||
                typeof(T) == typeof(byte) ||
                typeof(T) == typeof(uint) ||
                typeof(T) == typeof(short) ||
                typeof(T) == typeof(ushort) ||
                typeof(T) == typeof(byte) ||
                typeof(T) == typeof(sbyte) ||
                typeof(T).GetTypeInfo().IsEnum)
            {
                Set(guidKey, Convert.ToInt32(value));
                return;
            }

            if (typeof(T) == typeof(long) || typeof(T) == typeof(ulong))
            {
                Set(guidKey, Convert.ToInt64(value));
                return;
            }

            if (typeof(T) == typeof(IntPtr))
            {
                Set(guidKey, ((IntPtr)(object)value!).ToInt64());
                return;
            }

            if (typeof(T) == typeof(Guid))
            {
                Set(guidKey, (Guid)(object)value!);
                return;
            }

            if (typeof(T) == typeof(string))
            {
                Set(guidKey, value?.ToString());
                return;
            }

            if (typeof(T) == typeof(double) || typeof(T) == typeof(float))
            {
                Set(guidKey, Convert.ToDouble(value));
                return;
            }

            if (typeof(T) == typeof(byte[]))
            {
                byte[]? arrayValue = ((byte[])(object)value!);
                fixed (void* pBuffer = arrayValue)
                {
                    SetBlob(guidKey, (IntPtr)pBuffer, arrayValue.Length);
                }

                return;
            }

            if (typeof(T).GetTypeInfo().IsValueType)
            {
                void* pObj = Unsafe.AsPointer(ref value);
                SetBlob(guidKey, (IntPtr)pObj, Unsafe.SizeOf<T>());
                return;
            }

            if (typeof(T) == typeof(ComObject) || typeof(IUnknown).GetTypeInfo().IsAssignableFrom(typeof(T).GetTypeInfo()))
            {
                Set(guidKey, ((IUnknown)(object)value!));
                return;
            }

            throw new ArgumentException("The type of the value is not supported");
        }

        /// <summary>
        /// Adds an attribute value with a specified key.
        /// </summary>
        /// <param name="guidKey">
        /// A <see cref="Guid"/> that identifies the value to set. If this key already exists, the method overwrites the
        /// old value.
        /// </param>
        /// <param name="value">
        /// A <strong><see cref="Variant"/></strong> that contains the attribute value. The method copies the value. The
        /// <strong><see cref="Variant"/></strong> type must be one of the types listed in the <strong><see
        /// cref="AttributeType"/></strong> enumeration.
        /// </param>
        /// <returns>
        /// <p>
        /// The method returns an <strong><see cref="Result"/></strong>. Possible values include, but are not limited
        /// to, those in the following table.
        /// </p>
        /// <table><tr><th>Return code</th><th>Description</th></tr><tr><td><dl><dt><strong><see cref="Result.Ok"/></strong></dt></dl></td><td>
        /// <p>The method succeeded.</p>
        /// </td></tr><tr><td><dl><dt><strong>E_OUTOFMEMORY</strong></dt></dl></td><td>
        /// <p>Insufficient memory.</p>
        /// </td></tr><tr><td><dl><dt><strong>MF_E_INVALIDTYPE</strong></dt></dl></td><td>
        /// <p>Invalid attribute type.</p>
        /// </td></tr></table>
        /// </returns>
        /// <remarks>
        /// This method checks whether the <strong><see cref="Variant"/></strong> type is one of the attribute types
        /// defined in <strong><see cref="AttributeType"/></strong>, and fails if an unsupported type is used. However,
        /// this method does not check whether the <strong><see cref="Variant"/></strong> is the correct type for the
        /// specified attribute <see cref="Guid"/>. (There is no programmatic way to associate attribute GUIDs with
        /// property types.) For a list of Media Foundation attributes and their data types, see Media Foundation Attributes.
        /// </remarks>
        /// <unmanaged>
        /// HRESULT IMFAttributes::SetItem([In] const GUID&amp; guidKey,[In] const PROPVARIANT&amp; Value)
        /// </unmanaged>
        /// <unmanaged-short>IMFAttributes::SetItem</unmanaged-short>
        public unsafe void Set<T>(MediaAttributeKey<T> guidKey, T value)
        {
            Set(guidKey.Guid, value);
        }
    }
}
