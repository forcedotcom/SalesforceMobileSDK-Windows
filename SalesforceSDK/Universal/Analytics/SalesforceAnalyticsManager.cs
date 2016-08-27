using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.System.Profile;
using Windows.UI.Xaml.Media;
using Newtonsoft.Json.Linq;
using Salesforce.SDK.Analytics.Manager;
using Salesforce.SDK.Analytics.Model;
using Salesforce.SDK.Analytics.Store;
using Salesforce.SDK.Analytics.Transform;
using Salesforce.SDK.App;
using Salesforce.SDK.Auth;
using Salesforce.SDK.Core;
using Salesforce.SDK.Hybrid;
using Salesforce.SDK.Settings;

namespace Salesforce.SDK.Universal.Analytics
{
    /// <summary>
    /// This class contains APIs that can be used to interact with
    /// the SalesforceAnalytics library
    /// </summary>
    public class SalesforceAnalyticsManager
    {
        private static Dictionary<string, SalesforceAnalyticsManager> Instances;

        private AnalyticsManager analyticsManager;
        private EventStoreManager eventStoreManager;

        private static IApplicationInformationService ApplicationInformationService =
            SDKServiceLocator.Get<IApplicationInformationService>();

        private Dictionary<ITransform, IAnalyticsPublisher> remotes;

        /// <summary>
        /// Returns the instance of this class associated with this user account
        /// </summary>
        /// <param name="account"></param>
        /// <returns>Instance of this class</returns>
        public static SalesforceAnalyticsManager GetInstance(Account account)
        {
            return GetInstance(account, null);
        }

        /// <summary>
        /// Returns the instance of this class associated with this user and community
        /// </summary>
        /// <param name="account"></param>
        /// <param name="communityId"></param>
        /// <returns>Instance of this class</returns>
        public static SalesforceAnalyticsManager GetInstance(Account account, string communityId)
        {
            if (account == null)
            {
                account = AccountManager.GetAccount();
            }
            if (account == null)
            {
                return null;
            }
            var uniqueId = account.UserId;
            if (Account.InternalCommunityId.Equals(communityId))
            {
                communityId = null;
            }
            if (!string.IsNullOrWhiteSpace(communityId))
            {
                uniqueId = uniqueId + communityId;
            }
            SalesforceAnalyticsManager instance = null;
            if (Instances == null)
            {
                Instances = new Dictionary<string, SalesforceAnalyticsManager>();
                instance = new SalesforceAnalyticsManager(account, communityId);
                Instances.Add(uniqueId, instance);
            }
            else
            {
                instance = Instances[uniqueId];
            }
            if (instance == null)
            {
                instance = new SalesforceAnalyticsManager(account, communityId);
                Instances.Add(uniqueId, instance);
            }
            return instance;
        }

        /// <summary>
        /// Resets the instance of this class associated with this user account
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static async Task ResetAsync(Account account)
        {
            await ResetAsync(account, null);
        }

        /// <summary>
        /// Resets the instance of this class associated with this user and community.
        /// </summary>
        /// <param name="account"></param>
        /// <param name="communityId"></param>
        /// <returns></returns>
        public static async Task ResetAsync(Account account, String communityId)
        {
            if (account == null)
            {
                account = AccountManager.GetAccount();
            }
            if (account != null)
            {
                var uniqueId = account.UserId;
                if (Account.InternalCommunityId.Equals(communityId))
                {
                    communityId = null;
                }
                if (!string.IsNullOrWhiteSpace(communityId))
                {
                    uniqueId = uniqueId + communityId;
                }
                if (Instances != null)
                {
                    var manager = Instances[uniqueId];
                    if (manager != null)
                    {
                        await manager.analyticsManager.ResetAsync();
                    }
                    Instances.Remove(uniqueId);
                }
            }
        }

        /// <summary>
        /// Returns an instance of event store manager.
        /// </summary>
        /// <returns>Event store manager</returns>
        public EventStoreManager GetEventStoreManager()
        {
            return eventStoreManager;
        }

        /// <summary>
        /// Returns an instance of analytics manager.
        /// </summary>
        /// <returns>Analytics manager.</returns>
        public AnalyticsManager GetAnalyticsManager()
        {
            return analyticsManager;
        }

        /// <summary>
        /// Disables or enables logging of events. If logging is disabled, no events
        /// will be stored. However, publishing of events is still possible.
        /// </summary>
        /// <param name="enabled"></param>
        public void DisableEnableLogging(bool enabled)
        {
            eventStoreManager.DisableEnableLogging(enabled);
        }

        /// <summary>
        /// Returns whether logging is enabled or disabled
        /// </summary>
        /// <returns>True - if logging is enabled, False - otherwise</returns>
        public bool IsLoggingEnabled()
        {
            return eventStoreManager.IsLoggingEnabled();
        }

        /// <summary>
        /// Publishes all stored events to all registered network endpoints after
        /// applying the required event format transforms. Stored events will be
        /// deleted if publishing was successful for all registered endpoints.
        /// This method should NOT be called from the main thread.
        /// </summary>
        /// <returns></returns>
        public async Task PublishAllEventsAsync()
        {
            var events = await eventStoreManager.FetchAllEventsAsync();
            await PublishEventsAsync(events);
        }

        /// <summary>
        /// Publishes a list of events to all registered network endpoints after
        /// applying the required event format transforms. Stored events will be
        /// deleted if publishing was successful for all registered endpoints.
        /// This method should NOT be called from the main thread.
        /// </summary>
        /// <param name="events"></param>
        /// <returns></returns>
        public async Task PublishEventsAsync(List<InstrumentationEvent> events)
        {
            if (events == null || events.Count == 0)
            {
                return;
            }
            var eventIds = new List<string>();
            bool success = true;
            var remoteKeySet = remotes.Keys;
            foreach (var key in remoteKeySet)
            {
                ITransform transformer = new AiltnTransform();
                var eventsArray = new JArray();
                foreach (var instrumentationEvent in events)
                {
                    eventIds.Add(instrumentationEvent.EventId);
                    var eventJson = transformer.Transform(instrumentationEvent);
                    if (eventJson != null)
                    {
                        eventsArray.Add(eventJson);
                    }
                }
                var networkPublisher = new AILTNPublisher();
                bool networkSuccess = await networkPublisher.PublishAsync(eventsArray);

                /*
                 * Updates the success flag only if all previous requests have been
                 * successful. This ensures that the operation is marked success only
                 * if all publishers are successful.
                 */
                if (success)
                {
                    success = networkSuccess;
                }
            }

            if (success)
            {
                await eventStoreManager.DeleteEventsAsync(eventIds);
            }
        }

        /// <summary>
        /// Publishes an event to all registered network endpoints after
        /// applying the required event format transforms. Stored event will be
        /// deleted if publishing was successful for all registered endpoints.
        /// This method should NOT be called from the main thread
        /// </summary>
        /// <param name="instrumentationEvent"></param>
        /// <returns></returns>
        public async Task PublishEvent(InstrumentationEvent instrumentationEvent)
        {
            if (instrumentationEvent == null)
            {
                return;
            }
            var events = new List<InstrumentationEvent>();
            events.Add(instrumentationEvent);
            await PublishEventsAsync(events);
        }

        /// <summary>
        /// Adds a remote publisher to publish events to.
        /// </summary>
        /// <param name="transformer"></param>
        /// <param name="publisher"></param>
        public void AddRemotePublisher(ITransform transformer, IAnalyticsPublisher publisher)
        {
            if (transformer == null || publisher == null)
            {
                throw new Exception("Invalid transformer and/or publisher");
            }
            remotes.Add(transformer, publisher);
        }

        private SalesforceAnalyticsManager(Account account, string communityId)
        {
            var deviceAppAttributes = BuildDeviceAppAttributes();
            analyticsManager = new AnalyticsManager(account.GetCommunityLevelFileNameSuffix(), deviceAppAttributes,
                PincodeManager.RetrievePinCodeHash());
            eventStoreManager = (EventStoreManager) analyticsManager.GetEventStoreManager();
            var remotes = new Dictionary<ITransform, IAnalyticsPublisher>();
            var transformer = new AiltnTransform();
            var publisher = new AILTNPublisher();
            remotes.Add(transformer, publisher);
        }

        private DeviceAppAttributes BuildDeviceAppAttributes()
        {
            string appVersion = "";
            var appName = ApplicationInformationService.GetApplicationDisplayNameAsync().Result;
            var packageVersion = Package.Current.Id.Version;
            appVersion = packageVersion.Major + "." + packageVersion.Minor + "." +
                         packageVersion.Build;
            var osVersion = GetDeviceFamilyVersion(AnalyticsInfo.VersionInfo.DeviceFamilyVersion);
            var osName = "windows";
            var appType = ApplicationInformationService.GetAppType();
            var mobileSdkVersion = SDKManager.SDK_VERSION;
            var deviceModel = new EasClientDeviceInformation().SystemProductName;
            ;
            var deviceId = new EasClientDeviceInformation().Id.ToString();
            var clientId = BootConfig.GetBootConfig().Result.ClientId;
            return new DeviceAppAttributes(appVersion, appName, osVersion, osName, appType,
                mobileSdkVersion, deviceModel, deviceId, clientId);
        }

        private string GetDeviceFamilyVersion(string version)
        {
            //Voodoo magic to parse OS version and build from DeviceFamilyVersion
            ulong v = ulong.Parse(version);
            ulong v1 = (v & 0xFFFF000000000000L) >> 48;
            ulong v2 = (v & 0x0000FFFF00000000L) >> 32;
            ulong v3 = (v & 0x00000000FFFF0000L) >> 16;
            ulong v4 = v & 0x000000000000FFFFL;
            return $"{v1}.{v2}.{v3}.{v4}";
        }
    }
}
