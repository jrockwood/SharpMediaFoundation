// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceProvider.cs" company="Justin Rockwood">
//   Copyright (c) Justin Rockwood. All Rights Reserved. Licensed under the Apache License, Version 2.0. See
//   LICENSE.txt in the project root for license information.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------------

namespace SharpMediaFoundation
{
    using System;
    using SharpGen.Runtime;

    public partial class ServiceProvider
    {
        /// <summary>
        /// Retrieves a service interface.
        /// </summary>
        /// <typeparam name="T">Type of the interface to retrieve</typeparam>
        /// <param name="guidService">
        /// The service identifier (SID) of the service. For a list of service identifiers, see Service Interfaces.
        /// </param>
        /// <returns>An instance of T if the service is supported</returns>
        /// <exception cref="SharpGenException">if the service is not supported</exception>
        /// <unmanaged>
        /// HRESULT IMFGetService::GetService([In] const GUID&amp; guidService,[In] const GUID&amp; riid,[Out] void** ppvObject)
        /// </unmanaged>
        /// <unmanaged-short>IMFGetService::GetService</unmanaged-short>
        public T GetService<T>(Guid guidService) where T : ComObject
        {
            return FromPointer<T>(GetService(guidService, typeof(T).GUID));
        }
    }
}
