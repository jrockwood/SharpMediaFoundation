// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkQueueId.cs" company="Justin Rockwood">
//   Copyright (c) Justin Rockwood. All Rights Reserved. Licensed under the Apache License, Version 2.0. See
//   LICENSE.txt in the project root for license information.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------------

namespace SharpMediaFoundation
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// A Work Queue Identifier
    /// </summary>
    /// <msdn-id>ms703102</msdn-id>
    /// <unmanaged>Work Queue Identifiers</unmanaged>
    /// <unmanaged-short>Work Queue Identifiers</unmanaged-short>
    [StructLayout(LayoutKind.Sequential)]
    public struct WorkQueueId : IEquatable<WorkQueueId>
    {
        //// ===========================================================================================================
        //// Member Variables
        //// ===========================================================================================================

        /// <summary>
        /// The default queue associated to the <see cref="WorkQueueType.Standard"/>.
        /// </summary>
        public static readonly WorkQueueId Standard = new WorkQueueId(WorkQueueType.Standard);

        /// <summary>
        /// The identifier.
        /// </summary>
        public readonly int Id;

        //// ===========================================================================================================
        //// Constructors
        //// ===========================================================================================================

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkQueueId"/> struct.
        /// </summary>
        /// <param name="id">The id.</param>
        public WorkQueueId(int id)
        {
            Id = id;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkQueueId"/> struct.
        /// </summary>
        /// <param name="id">The id.</param>
        public WorkQueueId(WorkQueueType id)
        {
            Id = (int)id;
        }

        //// ===========================================================================================================
        //// Operators
        //// ===========================================================================================================

        /// <summary>
        /// Performs an implicit conversion from <see cref="int"/> to <see cref="WorkQueueId"/>.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator WorkQueueId(int id)
        {
            return new WorkQueueId(id);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="WorkQueueType"/> to <see cref="WorkQueueId"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator WorkQueueId(WorkQueueType type)
        {
            return new WorkQueueId(type);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="WorkQueueId"/> to <see cref="int"/>.
        /// </summary>
        /// <param name="workQueueId">The work queue Id.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator int(WorkQueueId workQueueId)
        {
            return workQueueId.Id;
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="WorkQueueId"/> to <see cref="WorkQueueType"/>.
        /// </summary>
        /// <param name="workQueueId">The work queue Id.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator WorkQueueType(WorkQueueId workQueueId)
        {
            return (WorkQueueType)workQueueId.Id;
        }

        //// ===========================================================================================================
        //// Methods
        //// ===========================================================================================================

        public override string ToString()
        {
            return $"Id: {Id} (Type: {(WorkQueueType)Id}";
        }

        //// ===========================================================================================================
        //// Equality Methods
        //// ===========================================================================================================

        public bool Equals(WorkQueueId other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object? obj)
        {
            return obj is WorkQueueId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public static bool operator ==(WorkQueueId left, WorkQueueId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(WorkQueueId left, WorkQueueId right)
        {
            return !left.Equals(right);
        }
    }
}
