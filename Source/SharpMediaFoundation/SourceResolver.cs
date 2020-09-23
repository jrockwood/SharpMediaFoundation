// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="SourceResolver.cs" company="Justin Rockwood">
//   Copyright (c) Justin Rockwood. All Rights Reserved. Licensed under the Apache License, Version 2.0. See
//   LICENSE.txt in the project root for license information.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------------

namespace SharpMediaFoundation
{
    using System.IO;
    using SharpGen.Runtime;

    /// <summary>
    /// Creates a media source from a URL or a byte stream.
    /// </summary>
    public partial class SourceResolver
    {
        /// <summary>
        /// Creates a <see cref="MediaSource"/> from the specified URL.
        /// </summary>
        /// <param name="url">The URL of the media file to open.</param>
        /// <returns>A <see cref="MediaSource"/> containing the media file's data.</returns>
        /// <remarks>
        /// This is an aggregation method that invokes <see cref="CreateObjectFromUrl(string,SourceResolverFlags)"/>
        /// behind the scenes and then performs a QueryInterface for the <see cref="MediaSource"/> interface.
        /// </remarks>
        public MediaSourceEx CreateMediaSource(string url)
        {
            using var source = new ComObject(CreateObjectFromUrl(url, SourceResolverFlags.MediaSource));
            return source.QueryInterface<MediaSourceEx>();
        }

        /// <summary>
        /// Creates a <see cref="MediaSource"/> from the specified stream.
        /// </summary>
        /// <param name="stream">The stream of the media file to open.</param>
        /// <param name="url">The URL of the media file to open.</param>
        /// <returns>A <see cref="MediaSource"/> containing the media file's data.</returns>
        /// <remarks>
        /// This is an aggregation method that invokes <see
        /// cref="CreateObjectFromStream(ByteStream,string,SourceResolverFlags)"/> behind the scenes and then performs a
        /// QueryInterface for the <see cref="MediaSource"/> interface.
        /// </remarks>
        public MediaSourceEx CreateMediaSource(Stream stream, string url)
        {
            var byteStream = new ByteStream(stream);
            return CreateMediaSource(byteStream, url);
        }

        /// <summary>
        /// Creates a <see cref="MediaSource"/> from the specified stream.
        /// </summary>
        /// <param name="stream">The stream of the media file to open.</param>
        /// <param name="url">The URL of the media file to open.</param>
        /// <returns>A <see cref="MediaSource"/> containing the media file's data.</returns>
        /// <remarks>
        /// This is an aggregation method that invokes <see
        /// cref="CreateObjectFromStream(ByteStream,string,SourceResolverFlags)"/> behind the scenes and then performs a
        /// QueryInterface for the <see cref="MediaSource"/> interface.
        /// </remarks>
        public MediaSourceEx CreateMediaSource(ByteStream stream, string url)
        {
            using var source = new ComObject(CreateObjectFromStream(stream, url, SourceResolverFlags.MediaSource));
            return source.QueryInterface<MediaSourceEx>();
        }

        public IUnknown CreateObjectFromUrl(string url, SourceResolverFlags flags)
        {
            return CreateObjectFromUrl(url, flags, null, out _);
        }

        public IUnknown CreateObjectFromUrl(string url, SourceResolverFlags flags, out ObjectType objectType)
        {
            return CreateObjectFromUrl(url, flags, null, out objectType);
        }

        public IUnknown CreateObjectFromUrl(
            string url,
            SourceResolverFlags flags,
            ComObject? propertyStore,
            out ObjectType objectType)
        {
            CreateObjectFromURL(url, (int)flags, propertyStore, out objectType, out IUnknown result);
            return result;
        }

        public IUnknown CreateObjectFromStream(ByteStream stream, string url, SourceResolverFlags flags)
        {
            return CreateObjectFromStream(stream, url, flags, null, out _);
        }

        public IUnknown CreateObjectFromStream(
            ByteStream stream,
            string url,
            SourceResolverFlags flags,
            out ObjectType objectType)
        {
            return CreateObjectFromStream(stream, url, flags, null, out objectType);
        }

        public IUnknown CreateObjectFromStream(
            ByteStream stream,
            string url,
            SourceResolverFlags flags,
            ComObject? propertyStore,
            out ObjectType objectType)
        {
            CreateObjectFromByteStream(stream, url, (int)flags, propertyStore, out objectType, out IUnknown result);
            return result;
        }
    }
}
