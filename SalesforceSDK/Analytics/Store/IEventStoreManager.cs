﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Salesforce.SDK.Analytics.Model;

namespace Salesforce.SDK.Analytics.Store
{
    public interface IEventStoreManager
    {
     /// <summary>
     /// Stores an event to the filesystem. A combination of event's unique ID and
     ///filename suffix is used to generate a unique filename per event.
     /// </summary>
     /// <param name="instrumentationEvent"></param>
     /// <returns></returns>
        Task StoreEventAsync(InstrumentationEvent instrumentationEvent);

        /// <summary>
        /// Stores a list of events to the filesystem
        /// </summary>
        /// <param name="instrumentationEvents"></param>
        /// <returns></returns>
        Task StoreEventsAsync(List<InstrumentationEvent> instrumentationEvents);

        /// <summary>
        /// Returns a specific event stored on the filesystem
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        Task<InstrumentationEvent> FetchEventAsync(string eventId);

        /// <summary>
        /// Returns all the events stored on the filesystem for that unique identifier
        /// </summary>
        /// <returns></returns>
        Task<List<InstrumentationEvent>> FetchAllEventsAsync();

        /// <summary>
        /// Deletes a specific event stored on the filesystem
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns>true if successful, false otherwise</returns>
        Task<bool> DeleteEventAsync(string eventId);

        /// <summary>
        /// Deletes all the events stored on the filesystem for that unique identifier
        /// </summary>
        /// <param name="eventIds"></param>
        /// <returns></returns>
        Task DeleteEventsAsync(List<string> eventIds);

        /// <summary>
        /// Deletes all the events stored on the filesystem
        /// </summary>
        /// <returns></returns>
        Task DeleteAllEventsAsync();

        /// <summary>
        /// Changes the encryption key to a new value. Fetches all stored events
        /// and re-encrypts them with the new encryption key
        /// </summary>
        /// <param name="oldKey"></param>
        /// <param name="newKey"></param>
        /// <returns></returns>
        Task ChangeEncryptionKeyAsync(string oldKey, string newKey);

        /// <summary>
        /// Disables or enables logging of events. If logging is disabled, no events
        /// will be stored. However, publishing of events is still possible
        /// </summary>
        /// <param name="enabled"></param>
        void DisableEnableLogging(bool enabled);

        /// <summary>
        /// Returns whether logging is enabled or disabled
        /// </summary>
        /// <returns></returns>
        bool IsLoggingEnabled();

        /// <summary>
        /// Sets the maximum number of events that can be stored
        /// </summary>
        /// <param name="maxEvents"></param>
        void SetMaxEvents(int maxEvents);
    }
}