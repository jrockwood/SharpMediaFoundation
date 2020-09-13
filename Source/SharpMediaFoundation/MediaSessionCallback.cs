// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="MediaSessionCallback.cs" company="Justin Rockwood">
//   Copyright (c) Justin Rockwood. All Rights Reserved. Licensed under the Apache License, Version 2.0. See
//   LICENSE.txt in the project root for license information.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------------

namespace SharpMediaFoundation
{
    using System;

    /// <summary>
    /// Specialized callback for <see cref="MediaSession"/>. This callback automatically starts the callback on the
    /// session, handles <see cref="MediaEventGenerator.EndGetEvent"/> on invoke, dispatch the asynchronous event to an
    /// external action to be implemented by the client and stops the callback when the event <see
    /// cref="MediaEventTypes.SessionClosed"/> is received.
    /// </summary>
    public class MediaSessionCallback : AsyncCallbackBase
    {
        //// ===========================================================================================================
        //// Member Variables
        //// ===========================================================================================================

        private readonly MediaSession _session;
        private readonly Action<MediaEvent> _eventCallback;

        //// ===========================================================================================================
        //// Constructors
        //// ===========================================================================================================

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaSessionCallback"/> class.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="eventCallback">
        /// The event callback. The object <see cref="MediaEvent"/> must be disposed by the callback when finished with it.
        /// </param>
        public MediaSessionCallback(MediaSession session, Action<MediaEvent> eventCallback)
        {
            _session = session;
            _eventCallback = eventCallback;

            // Subscribe to next events automatically
            session.BeginGetEvent(this, null);
        }

        //// ===========================================================================================================
        //// Methods
        //// ===========================================================================================================

        public override void Invoke(AsyncResult asyncResult)
        {
            // EndGetEvent mandatory
            var evt = _session.EndGetEvent(asyncResult);

            var typeInfo = evt.TypeInfo;
            if (typeInfo != MediaEventTypes.SessionClosed)
            {
                // If not closed, continnue to subscribe to next events
                _session.BeginGetEvent(this, null);
            }

            // Call the callback
            _eventCallback(evt);
        }
    }
}
