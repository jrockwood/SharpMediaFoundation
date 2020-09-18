// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="PropVariantMarshaler.cs" company="Justin Rockwood">
//   Copyright (c) Justin Rockwood. All Rights Reserved. Licensed under the Apache License, Version 2.0. See
//   LICENSE.txt in the project root for license information.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------------

namespace SharpMediaFoundation.Interop
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Custom marshaler to marshal <see cref="PropVariant"/> objects on parameters that <strong>output</strong> them.
    /// </summary>
    /// <remarks>
    /// <para>
    /// When defining parameters that use this marshaler, you must always declare them as both [In] and [Out]. This will
    /// result in *both* <see cref="MarshalManagedToNative"/> and <see cref="MarshalNativeToManaged"/> being called.
    /// Since the order they are called depends on exactly what's happening, <see cref="Layer.Stage"/> lets us know which
    /// way things are being called.
    /// </para>
    /// <para><strong>Managed calling unmanaged</strong></para>
    /// <para>
    /// In this case, <see cref="MarshalManagedToNative"/> is called first with <see cref="Layer.Stage"/> == 0. When <see
    /// cref="MarshalManagedToNative"/> is called, we store the managed object (so we know where to copy it back to),
    /// then we clear the variant, allocate some COM memory and pass a pointer to the COM memory to the native code.
    /// When the native code is done, <see cref="MarshalNativeToManaged"/> gets called (<see cref="Layer.Stage"/> == 1)
    /// with the pointer to the COM memory. At that point, we copy the contents back into the (saved) managed object.
    /// After that, <see cref="CleanUpNativeData"/> gets called and we release the COM memory.
    /// </para>
    /// <para><strong>Unmanaged calling managed</strong></para>
    /// <para>
    /// In this case, <see cref="MarshalNativeToManaged"/> is called first. We store the native pointer (so we know
    /// where to copy the result back to), then we create a managed <see cref="PropVariant"/> and copy the native value
    /// into it. When the managed code is done, <see cref="MarshalManagedToNative"/> gets called with the managed
    /// <see cref="PropVariant"/> we created. At that point, we copy the contents of the managed <see cref="PropVariant"/>
    /// back into the (saved) native pointer.
    /// </para>
    /// <para><strong>Multi-threading</strong></para>
    /// <para>
    /// When marshaling from managed to native, the first thing that happens is that .NET creates an instance of the
    /// <see cref="PropVariantMarshaler"/> class. It then calls <see cref="MarshalManagedToNative"/> to send you the
    /// managed object into which the unmanaged data will eventually be stored. However it doesn't pass the managed
    /// object again when <see cref="MarshalNativeToManaged"/> eventually gets called. No problem, you assume, I'll just
    /// store it as a data member and use it when <see cref="MarshalNativeToManaged"/> get called. Yeah, about that...
    /// First of all, if several threads all start calling a method that uses <see cref="PropVariantMarshaler"/>, .NET
    /// WILL create multiple instances of this class. However, it will then DESTRUCT all of them except 1, which it will
    /// use from every thread. Unless you are aware of this behavior and take precautions, having multiple thread using
    /// the same instance results in chaos.
    /// </para>
    /// <para>
    /// Also be aware that if two different methods both use <see cref="PropVariantMarshaler"/> (say <see cref="IMFAttributes.GetItem"/>
    /// and <see cref="IMFAttributes.GetItemByIndex"/>), .NET may use the same instance for both methods. Unless they
    /// each have a unique <see cref="MarshalCookie"/> string. Using a unique <see cref="MarshalCookie"/> doesn't help
    /// with multi-threading, but it does help keep the marshaling from one method call from interfering with another.
    /// </para>
    /// <para><strong>Recursion</strong></para>
    /// <para>
    /// If managed code calls unmanaged code through <see cref="PropVariantMarshaler"/>, then that unmanaged code in
    /// turn calls <see cref="IMFAttributes.GetItem"/> against a managed object, what happens? .NET will use a single
    /// instance of <see cref="PropVariantMarshaler"/> to handle both calls, even if the actual <see cref="PropVariant"/>
    /// used for the second call is a different instance of the <see cref="PropVariant"/> class. It can also use the
    /// same managed thread id all the way through (so using a simple ThreadStatic is not sufficient to keep track of
    /// this). So if you see a call to <see cref="MarshalNativeToManaged"/> right after a call to <see cref="MarshalManagedToNative"/>,
    /// it might be the second half of a 'normal' bit of marshaling, or it could be the start of a nested call from
    /// unmanaged back into managed.
    /// </para>
    /// <para>
    /// There are 2 ways to detect nesting:
    /// 1. If the pNativeData sent to <see cref="MarshalNativeToManaged"/> *isn't* the one you returned from
    ///    <see cref="MarshalManagedToNative"/>, then you are nesting.
    /// 2. <see cref="Layer.Stage"/> starts at 0. <see cref="MarshalManagedToNative"/> increments it to 1. Then <see
    ///    cref="MarshalNativeToManaged"/> increments it to 2. For non-nested, that should be the end. So if <see
    ///    cref="MarshalManagedToNative"/> gets called with <see cref="Layer.Stage"/> == 2, we are nesting.
    /// </para>
    /// <para>
    /// <strong>Warning!</strong> You cannot assume that both marshaling routines will always get called. For example
    /// if calling from unmanaged to managed, <see cref="MarshalNativeToManaged"/> will get called, but if the managed
    /// code throws an exception, <see cref="MarshalManagedToNative"/> will not. This can be a problem since .NET REUSES
    /// instances of the marshaler. So it is essential that class members always get cleaned up in <see cref="CleanUpManagedData"/>
    /// and <see cref="CleanUpNativeData"/>.
    /// </para>
    /// <para>
    /// All this helps explain the otherwise inexplicable complexity of this class: it uses a ThreadStatic variable to
    /// keep instance data from one thread from interfering with another thread, nests instances of MyProps, and uses two
    /// different methods to check for recursion (which in theory could be nested quite deeply).
    /// </para>
    /// </remarks>
    internal class PropVariantMarshaler : ICustomMarshaler
    {
        //// ===========================================================================================================
        //// Member Variables
        //// ===========================================================================================================

        /// <summary>
        /// Max number of arguments in a single method cal that can use <see cref="PropVariantMarshaler"/>.
        /// </summary>
        private const int MaxArgs = 2;

        private readonly int _index;

        //// ===========================================================================================================
        //// Constructors
        //// ===========================================================================================================

        private PropVariantMarshaler(string marshalCookie)
        {
            MarshalCookie = marshalCookie;

            int len = marshalCookie.Length;

            // On methods that have more than one PropVariantMarshaler on a single method, the cookie is in the form
            // `InterfaceName.MethodName.0` and `InterfaceName.MethodName.1`.
            if (marshalCookie[len - 2] != '.')
            {
                _index = 0;
            }
            else
            {
                _index = int.Parse(marshalCookie.Substring(len - 1));
                Debug.Assert(_index < MaxArgs);
            }
        }

        public string MarshalCookie { get; }

        //// ===========================================================================================================
        //// Methods
        //// ===========================================================================================================

        public IntPtr MarshalManagedToNative(object? managedObj)
        {
            // Nulls don't invoke custom marshaling.
            Debug.Assert(managedObj != null);

            var top = Layer.GetTop(_index);

            switch (top.Stage)
            {
                case 0:
                    {
                        // We are just starting a "Managed calling unmanaged" call.

                        // Cast the object back to a PropVariant and save it for use in MarshalNativeToManaged.
                        top.Variant = managedObj as PropVariant;

                        // This could happen if (somehow) managedObj isn't a PropVariant. During normal marshaling, the
                        // custom marshaler doesn't get called if the parameter is null.
                        Debug.Assert(top.Variant != null);

                        // Release any memory currently allocated in the PropVariant. In theory, the (managed) caller
                        // should have done this before making the call that got us here, but .NET programmers don't
                        // generally think that way. To avoid leaks, do it for them.
                        top.Variant?.Clear();

                        // Create an appropriately sized buffer (varies from x86 to x64).
                        int size = GetNativeDataSize();
                        top.Ptr = top.Alloc(size);

                        // Copy in the (empty) PropVariant. In theory, we could just zero out the first two bytes (the
                        // VariantType), but since PropVariantClear wipes the whole struct, that's what we do here to be safe.
                        Marshal.StructureToPtr(top.Variant, top.Ptr, fDeleteOld: false);

                        break;
                    }

                case 1:
                    {
                        if (!ReferenceEquals(top.Variant, managedObj))
                        {
                            // If we get here, we have already received a call to MarshalNativeToManaged where we
                            // created a PropVariant and stored it into top.Variant. But the object we just got passed
                            // here isn't the same one. Therefore, instead of being the second half of an "Unmanaged
                            // calling managed" (as Stage led us to believe), this is really the first half of a nested
                            // "Managed calling unmanaged" (see Recursion in the comments at the top of the class). Add
                            // another layer.
                            Layer.AddLayer(_index);

                            // Try this call again now that we have fixed s_currentProps.
                            return MarshalManagedToNative(managedObj);
                        }

                        // This is (probably) the second half of "Unmanaged calling managed." However, it could be the
                        // first half of a nested usage of PropVariants. If it is a nested, we'll eventually figure that
                        // out in case 2. Copy the data from the managed object into the native pointer that we received
                        // in MarshalNativeToManaged.
                        Marshal.StructureToPtr(top.Variant, top.Ptr, fDeleteOld: false);

                        break;
                    }

                case 2:
                    {
                        // Apparently this is part 3 of a two part call, which means we are doing a nested call.
                        // Normally, we would catch the fact that this is a nested call with the ReferenceEquals check
                        // above. However, if the same PropVariant instance is being passed through again, we end up
                        // here. So, add a layer.
                        Layer.SplitLayer(_index);

                        // Try this call again now that we have fixed s_currentProps.
                        return MarshalManagedToNative(managedObj);
                    }

                default:
                    Environment.FailFast(
                        "Something horrible has happened, probably due to marshaling of nested PropVariant calls.");
                    break;
            }

            top.StageCompleted();

            return top.Ptr;
        }

        public object? MarshalNativeToManaged(IntPtr pNativeData)
        {
            // Nulls don't invoke custom marshaling.
            Debug.Assert(pNativeData != IntPtr.Zero);

            var top = Layer.GetTop(_index);

            switch (top.Stage)
            {
                case 0:
                    {
                        // We are just starting a "Unmanaged calling managed" call.

                        // Caller should have cleared variant before calling us. Might be acceptable for types *other*
                        // than IUnknown, String, Blob and StringArray, but it is still bad design. We're checking for
                        // it, but we work around it.

                        // Read the 16bit VariantType.
                        Debug.Assert(Marshal.ReadInt16(pNativeData) == 0);

                        // Create an empty managed PropVariant without using pNativeData.
                        top.Variant = new PropVariant();

                        // Save the pointer for use in MarshalManagedToNative.
                        top.Ptr = pNativeData;

                        break;
                    }

                case 1:
                    {
                        if (top.Ptr != pNativeData)
                        {
                            // If we get here, we have already received a call to MarshalManagedToNative where we
                            // created an IntPtr and stored it into top.Ptr. But the value we just got passed here isn't
                            // the same one. Therefore instead of being the second half of a "Managed calling unmanaged"
                            // (as Stage led us to believe) this is really the first half of a nested "Unmanaged calling
                            // managed" (see Recursion in the comments at the top of this class). Add another layer.
                            Layer.AddLayer(_index);

                            // Try this call again now that we have fixed s_currentProps.
                            return MarshalNativeToManaged(pNativeData);
                        }

                        // This is (probably) the second half of "Managed calling unmanaged." However, it could be the
                        // first half of a nested usage of PropVariants. If it is a nested, we'll eventually figure that
                        // out in case 2.

                        // Copy the data from the native pointer into the managed object that we received in MarshalManagedToNative.
                        Marshal.PtrToStructure(pNativeData, top.Variant);

                        break;
                    }

                case 2:
                    {
                        // Apparently this is 'part 3' of a 2 part call. Which means we are doing a nested call.
                        // Normally we would catch the fact that this is a nested call with the (top.Ptr != pNativeData)
                        // check above. However, if the same PropVariant instance is being passed thru again, we end up
                        // here. So, add a layer.
                        Layer.SplitLayer(_index);

                        // Try this call again now that we have fixed s_currentProps.
                        return MarshalNativeToManaged(pNativeData);
                    }

                default:
                    Environment.FailFast(
                        "Something horrible has happened, probably due to marshaling of nested PropVariant calls.");
                    break;
            }

            top.StageCompleted();

            return top.Variant;
        }

        public void CleanUpManagedData(object managedObj)
        {
            // Note that if there are nested calls, one of the Cleanup*Data methods will be called at the end of each pair:
            // MarshalNativeToManaged
            // MarshalManagedToNative
            // CleanUpManagedData
            //
            // or for recursion:
            //
            // MarshalManagedToNative 1
            // MarshalNativeToManaged 2
            // MarshalManagedToNative 2
            // CleanUpManagedData     2
            // MarshalNativeToManaged 1
            // CleanUpNativeData      1

            // Clear() either pops an entry, or clears the values for the next call.
            var top = Layer.GetTop(_index);
            top.Clear(_index);
        }

        public void CleanUpNativeData(IntPtr pNativeData)
        {
            // Clear() either pops an entry, or clears the values for the next call.
            var top = Layer.GetTop(_index);
            top.Clear(_index);
        }

        public int GetNativeDataSize()
        {
            // The number of bytes to marshal. Size varies between x86 and x64.
            return Marshal.SizeOf<PropVariant>();
        }

        // This method is called by interop to create the custom marshaler. The (optional) cookie is the value specified
        // in MarshalCookie="asdf", or "" if none is specified.
        private static ICustomMarshaler GetInstance(string cookie)
        {
            return new PropVariantMarshaler(cookie);
        }

        //// ===========================================================================================================
        //// Classes
        //// ===========================================================================================================

        private class Layer
        {
            [ThreadStatic]
            private static Layer[]? s_currentLayers;

            private static Layer[] CurrentLayers => s_currentLayers ?? throw new InvalidOperationException();

            private Layer? _parent;
            private bool _isAllocated;

            public PropVariant? Variant { get; set; }
            public IntPtr Ptr { get; set; }

            public int Stage { get; private set; }

            public void StageCompleted()
            {
                Stage++;
            }

            public static Layer AddLayer(int index)
            {
                var layer = new Layer
                {
                    _parent = CurrentLayers[index]
                };
                CurrentLayers[index] = layer;

                return layer;
            }

            public static void SplitLayer(int index)
            {
                Layer top = AddLayer(index);
                Layer parent = top._parent ?? throw new InvalidOperationException();

                top.Stage = 1;
                top.Ptr = parent.Ptr;
                top.Variant = parent.Variant;

                parent.Stage = 1;
            }

            public static Layer GetTop(int index)
            {
                // If the member hasn't been initialized, do it now. We can't do this in the PropVariantMarshaler
                // constructor since it may have been called on a different thread.
                if (s_currentLayers == null)
                {
                    s_currentLayers = new Layer[MaxArgs];
                    for (int i = 0; i < MaxArgs; i++)
                    {
                        s_currentLayers[i] = new Layer();
                    }
                }

                return s_currentLayers[index];
            }

            public void Clear(int index)
            {
                if (_isAllocated)
                {
                    Marshal.FreeCoTaskMem(Ptr);
                    _isAllocated = false;
                }

                if (_parent == null)
                {
                    // Never delete the last entry.
                    Stage = 0;
                    Variant = null;
                    Ptr = IntPtr.Zero;
                }
                else
                {
                    Variant = null;
                    CurrentLayers[index] = _parent;
                }
            }

            public IntPtr Alloc(int size)
            {
                IntPtr p = Marshal.AllocCoTaskMem(size);
                _isAllocated = true;
                return p;
            }
        }
    }
}
