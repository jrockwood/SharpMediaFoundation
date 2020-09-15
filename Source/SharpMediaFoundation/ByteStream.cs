// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="ByteStream.cs" company="Justin Rockwood">
//   Copyright (c) Justin Rockwood. All Rights Reserved. Licensed under the Apache License, Version 2.0. See
//   LICENSE.txt in the project root for license information.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------------

namespace SharpMediaFoundation
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using SharpGen.Runtime;
    using SharpGen.Runtime.Win32;

    public partial class ByteStream
    {
        //// ===========================================================================================================
        //// Member Variables
        //// ===========================================================================================================

        private readonly Stream? _sourceStream;
        private readonly ComStream? _comStream;
        private ComStreamProxy? _streamProxy;

        //// ===========================================================================================================
        //// Constructors
        //// ===========================================================================================================

        /// <summary>
        /// Instantiates a new instance <see cref="ByteStream"/> from a <see cref="Stream"/>.
        /// </summary>
        /// <unmanaged>HRESULT MFCreateMFByteStreamOnStreamEx([In] IUnknown* punkStream,[Out] IMFByteStream** ppByteStream)</unmanaged>
        /// <unmanaged-short>MFCreateMFByteStreamOnStreamEx</unmanaged-short>
        public ByteStream(Stream sourceStream)
        {
            _sourceStream = sourceStream;
            _streamProxy = new ComStreamProxy(sourceStream);
            MediaFactory.CreateMFByteStreamOnStream(_streamProxy, this);
        }

        /// <summary>
        /// Instantiates a new instance <see cref="ByteStream"/> from a byte array.
        /// </summary>
        /// <unmanaged>HRESULT MFCreateMFByteStreamOnStreamEx([In] IUnknown* punkStream,[Out] IMFByteStream** ppByteStream)</unmanaged>
        /// <unmanaged-short>MFCreateMFByteStreamOnStreamEx</unmanaged-short>
        public ByteStream(byte[] sourceStream)
            : this(new MemoryStream(sourceStream))
        {
        }

        /// <summary>
        /// Instantiates a new instance <see cref="ByteStream"/> from a <see cref="Stream"/>.
        /// </summary>
        /// <unmanaged>HRESULT MFCreateMFByteStreamOnStreamEx([In] IUnknown* punkStream,[Out] IMFByteStream** ppByteStream)</unmanaged>
        /// <unmanaged-short>MFCreateMFByteStreamOnStreamEx</unmanaged-short>
        public ByteStream(ComStream sourceStream)
        {
            _comStream = sourceStream;
            MediaFactory.CreateMFByteStreamOnStream(sourceStream, this);
        }

        //// ===========================================================================================================
        //// Properties
        //// ===========================================================================================================

        /// <summary>
        /// Retrieves the characteristics of the byte stream.
        /// </summary>
        /// <returns>The capabilities of the stream.</returns>
        /// <unmanaged>HRESULT IMFByteStream::GetCapabilities([Out] unsigned int* pdwCapabilities)</unmanaged>
        /// <unmanaged-short>IMFByteStream::GetCapabilities</unmanaged-short>
        public int Capabilities
        {
            get
            {
                GetCapabilities_(out int result);
                return result;
            }
        }

        /// <summary>
        /// Retrieves the length of the stream.
        /// </summary>
        /// <returns>The length of the stream, in bytes. If the length is unknown, this value is -1.</returns>
        /// <unmanaged>HRESULT IMFByteStream::GetLength([Out] unsigned longlong* pqwLength)</unmanaged>
        /// <unmanaged-short>IMFByteStream::GetLength</unmanaged-short>
        public long Length
        {
            get
            {
                GetLength_(out ulong length);
                return (long)length;
            }

            set => SetLength_((ulong)value);
        }

        /// <summary>
        /// Retrieves the current read or write position in the stream.
        /// </summary>
        /// <returns>The current position, in bytes.</returns>
        /// <remarks>
        /// The methods that update the current position are <strong>Read</strong>, <strong>BeginRead</strong>,
        /// <strong>Write</strong>, <strong>BeginWrite</strong>, <strong>SetCurrentPosition</strong>, and <strong>Seek</strong>.
        /// </remarks>
        /// <unmanaged>HRESULT IMFByteStream::GetCurrentPosition([Out] unsigned longlong* pqwPosition)</unmanaged>
        /// <unmanaged-short>IMFByteStream::GetCurrentPosition</unmanaged-short>
        public long CurrentPosition
        {
            get
            {
                GetCurrentPosition_(out ulong position);
                return (long)position;
            }
            set => SetCurrentPosition_((ulong)value);
        }

        /// <summary>
        /// Queries whether the current position has reached the end of the stream.
        /// </summary>
        /// <returns>true if the end of the stream has been reached</returns>
        /// <unmanaged>HRESULT IMFByteStream::IsEndOfStream([Out] BOOL* pfEndOfStream)</unmanaged>
        /// <unmanaged-short>IMFByteStream::IsEndOfStream</unmanaged-short>
        public bool IsEndOfStream => IsEndOfStream_;

        //// ===========================================================================================================
        //// Methods
        //// ===========================================================================================================

        /// <summary>
        /// Reads data from the stream.
        /// </summary>
        /// <param name="buffer">Pointer to a buffer that receives the data. The caller must allocate the buffer.</param>
        /// <param name="offset">Offset into the buffer.</param>
        /// <param name="count">Size of the buffer in bytes.</param>
        /// <returns>The number of bytes that are copied into the buffer</returns>
        /// <remarks>
        /// <p>
        /// This method reads at most <em>cb</em> bytes from the current position in the stream and copies them into the
        /// buffer provided by the caller. The number of bytes that were read is returned in the <em>pcbRead</em>
        /// parameter. The method does not return an error code on reaching the end of the file, so the application
        /// should check the value in <em>pcbRead</em> after the method returns.
        /// </p>
        /// <p>This method is synchronous. It blocks until the read operation completes.</p>
        /// </remarks>
        /// <unmanaged>
        /// HRESULT IMFByteStream::Read([Out, Buffer] unsigned char* pb,[In] unsigned int cb,[Out] unsigned int* pcbRead)
        /// </unmanaged>
        /// <unmanaged-short>IMFByteStream::Read</unmanaged-short>
        public unsafe int Read(byte[] buffer, int offset, int count)
        {
            uint result;
            fixed (void* ptr = &buffer[offset])
            {
                Read_((IntPtr)ptr, (uint)count, out result);
            }

            return (int)result;
        }

        /// <summary>
        /// Begins an asynchronous read operation from the stream.
        /// </summary>
        /// <param name="buffer">Pointer to a buffer that receives the data. The caller must allocate the buffer.</param>
        /// <param name="offset">The offset in the buffer to begin reading from.</param>
        /// <param name="count">Size of the buffer in bytes.</param>
        /// <param name="callback">
        /// Pointer to the <strong><see cref="IAsyncCallback"/></strong> interface of a callback object.
        /// The caller must implement this interface.
        /// </param>
        /// <param name="context">
        /// Pointer to the <strong><see cref="ComObject"/></strong> interface of a state object, defined by the
        /// caller. This parameter can be <strong><c>null</c></strong>. You can use this object to hold state
        /// information. The object is returned to the caller when the callback is invoked.
        /// </param>
        /// <returns>
        /// If this method succeeds, it returns <strong><see cref="Result.Ok"/></strong>. Otherwise, it returns
        /// an <strong><see cref="Result"/></strong> error code.
        /// </returns>
        /// <remarks>
        /// <p>
        /// When all of the data has been read into the buffer, the callback object's <strong><see
        /// cref="IAsyncCallback.Invoke"/></strong> method is called. At that point, the
        /// application should call <strong><see cref="EndRead"/></strong> to
        /// complete the asynchronous request.
        /// </p>
        /// <p>Do not read from, write to, free, or reallocate the buffer while an asynchronous read is pending.</p>
        /// </remarks>
        /// <unmanaged>
        /// HRESULT IMFByteStream::BeginRead([Out, Buffer] unsigned char* pb,[In] unsigned int cb,[In] IMFAsyncCallback*
        /// pCallback,[In] IUnknown* punkState)
        /// </unmanaged>
        /// <unmanaged-short>IMFByteStream::BeginRead</unmanaged-short>
        public unsafe void BeginRead(byte[] buffer, int offset, int count, IAsyncCallback callback, object? context)
        {
            fixed (void* ptr = &buffer[offset])
            {
                BeginRead_((IntPtr)ptr, (uint)count, callback, context != null ? Marshal.GetIUnknownForObject(context) : IntPtr.Zero);
            }
        }

        /// <summary>
        /// Completes an asynchronous read operation.
        /// </summary>
        /// <param name="asyncResult">
        /// Pointer to the <strong><see cref="AsyncResult"/></strong> interface. Pass in the same reference that your
        /// callback object received in the <strong><see cref="IAsyncCallback.Invoke"/></strong> method.
        /// </param>
        /// <returns>The number of bytes that were read</returns>
        /// <remarks>
        /// Call this method after the <strong><see cref="BeginRead"/></strong> method completes asynchronously.
        /// </remarks>
        /// <unmanaged>HRESULT IMFByteStream::EndRead([In] IMFAsyncResult* pResult,[Out] unsigned int* pcbRead)</unmanaged>
        /// <unmanaged-short>IMFByteStream::EndRead</unmanaged-short>
        public int EndRead(AsyncResult asyncResult)
        {
            EndRead_(asyncResult, out uint result);
            return (int)result;
        }

        /// <summary>
        /// Writes data to the stream.
        /// </summary>
        /// <param name="buffer">
        /// Pointer to a buffer that contains the data to write.
        /// </param>
        /// <param name="offset">The offset within the buffer to begin writing at.</param>
        /// <param name="count">
        /// Size of the buffer in bytes.
        /// </param>
        /// <returns>The number of bytes that are written.</returns>
        /// <remarks>
        /// <p>
        /// This method writes the contents of the <em>pb</em> buffer to the stream, starting at the current stream
        /// position. The number of bytes that were written is returned in the <em>pcbWritten</em> parameter.
        /// </p>
        /// <p>This method is synchronous. It blocks until the write operation completes.</p>
        /// </remarks>
        /// <unmanaged>
        /// HRESULT IMFByteStream::Write([In, Buffer] const unsigned char* pb,[In] unsigned int cb,[Out] unsigned int* pcbWritten)
        /// </unmanaged>
        /// <unmanaged-short>IMFByteStream::Write</unmanaged-short>
        public unsafe int Write(byte[] buffer, int offset, int count)
        {
            uint result;
            fixed (void* ptr = &buffer[offset])
            {
                Write_((IntPtr)ptr, (uint)count, out result);
            }

            return (int)result;
        }

        /// <summary>
        /// Begins an asynchronous write operation to the stream.
        /// </summary>
        /// <param name="buffer">Pointer to a buffer containing the data to write.</param>
        /// <param name="offset">The offset within the buffer to begin writing at.</param>
        /// <param name="count">Size of the buffer in bytes.</param>
        /// <param name="callback">
        /// Pointer to the <strong><see cref="IAsyncCallback"/></strong> interface of a callback object. The caller must
        /// implement this interface.
        /// </param>
        /// <param name="context">
        /// Pointer to the <strong><see cref="ComObject"/></strong> interface of a state object, defined by the caller.
        /// This parameter can be <strong><c>null</c></strong>. You can use this object to hold state information. The
        /// object is returned to the caller when the callback is invoked.
        /// </param>
        /// <returns>
        /// If this method succeeds, it returns <strong><see cref="Result.Ok"/></strong>. Otherwise, it returns an
        /// <strong><see cref="Result"/></strong> error code.
        /// </returns>
        /// <remarks>
        /// <p>
        /// When all of the data has been written to the stream, the callback object's <strong><see
        /// cref="IAsyncCallback.Invoke"/></strong> method is called. At that point, the application should call
        /// <strong><see cref="EndWrite"/></strong> to complete the asynchronous request.
        /// </p>
        /// <p>Do not reallocate, free, or write to the buffer while an asynchronous write is still pending.</p>
        /// </remarks>
        /// <unmanaged>
        /// HRESULT IMFByteStream::BeginWrite([In, Buffer] const unsigned char* pb,[In] unsigned int cb,[In]
        /// IMFAsyncCallback* pCallback,[In] IUnknown* punkState)
        /// </unmanaged>
        /// <unmanaged-short>IMFByteStream::BeginWrite</unmanaged-short>
        public unsafe void BeginWrite(byte[] buffer, int offset, int count, IAsyncCallback callback, object? context)
        {
            fixed (void* ptr = &buffer[offset])
            {
                BeginWrite_((IntPtr)ptr, (uint)count, callback, context != null ? Marshal.GetIUnknownForObject(context) : IntPtr.Zero);
            }
        }

        /// <summary>
        /// Completes an asynchronous write operation.
        /// </summary>
        /// <param name="asyncResult">
        /// Pointer to the <strong><see cref="AsyncResult"/></strong> interface. Pass in the same reference that your
        /// callback object received in the <strong><see cref="IAsyncCallback.Invoke"/></strong> method.
        /// </param>
        /// <returns>The number of bytes that were written</returns>
        /// <remarks>
        /// Call this method when the <strong><see cref="BeginWrite"/></strong> method completes asynchronously.
        /// </remarks>
        /// <unmanaged>HRESULT IMFByteStream::EndWrite([In] IMFAsyncResult* pResult,[Out] unsigned int* pcbWritten)</unmanaged>
        /// <unmanaged-short>IMFByteStream::EndWrite</unmanaged-short>
        public int EndWrite(AsyncResult asyncResult)
        {
            EndWrite_(asyncResult, out uint result);
            return (int)result;
        }

        /// <summary>
        /// Moves the current position in the stream by a specified offset.
        /// </summary>
        /// <param name="seekOrigin">
        /// Specifies the origin of the seek as a member of the <strong><see
        /// cref="ByteStreamSeekOrigin"/></strong> enumeration. The offset is calculated
        /// relative to this position.
        /// </param>
        /// <param name="seekOffset">
        /// Specifies the new position, as a byte offset from the seek origin.
        /// </param>
        /// <param name="seekFlags">
        /// <p>Specifies zero or more flags. The following flags are defined.</p>
        /// <table><tr><th>Value</th><th>Meaning</th></tr><tr><td><dl><dt><strong>MFBYTESTREAM_SEEK_FLAG_CANCEL_PENDING_IO</strong></dt></dl></td><td>
        /// <p>All pending I/O requests are canceled after the seek request completes successfully.</p>
        /// </td></tr></table>
        /// </param>
        /// <returns>The new position after the seek</returns>
        /// <unmanaged>
        /// HRESULT IMFByteStream::Seek([In] MFBYTESTREAM_SEEK_ORIGIN SeekOrigin,[In] longlong llSeekOffset,[In]
        /// unsigned int dwSeekFlags,[Out] unsigned longlong* pqwCurrentPosition)
        /// </unmanaged>
        /// <unmanaged-short>IMFByteStream::Seek</unmanaged-short>
        public long Seek(ByteStreamSeekOrigin seekOrigin, long seekOffset, ByteStreamSeekFlags seekFlags)
        {
            Seek_(seekOrigin, seekOffset, seekFlags, out ulong result);
            return (long)result;
        }

        /// <summary>
        /// Clears any internal buffers used by the stream. If you are writing to the stream, the buffered data is
        /// written to the underlying file or device.
        /// </summary>
        /// <returns>
        /// If this method succeeds, it returns <strong><see cref="Result.Ok"/></strong>. Otherwise, it returns an
        /// <strong><see cref="Result"/></strong> error code.
        /// </returns>
        /// <remarks>If the byte stream is read-only, this method has no effect.</remarks>
        /// <unmanaged>HRESULT IMFByteStream::Flush()</unmanaged>
        /// <unmanaged-short>IMFByteStream::Flush</unmanaged-short>
        public void Flush()
        {
            Flush_();
        }

        /// <summary>
        /// Closes the stream and releases any resources associated with the stream, such as sockets or file handles.
        /// This method also cancels any pending asynchronous I/O requests.
        /// </summary>
        /// <returns>
        /// If this method succeeds, it returns <strong><see cref="Result.Ok"/></strong>. Otherwise, it returns an
        /// <strong><see cref="Result"/></strong> error code.
        /// </returns>
        /// <unmanaged>HRESULT IMFByteStream::Close()</unmanaged>
        /// <unmanaged-short>IMFByteStream::Close</unmanaged-short>
        public void Close()
        {
            Close_();
        }

        protected override unsafe void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            _streamProxy?.Dispose();
            _streamProxy = null;
        }
    }
}
