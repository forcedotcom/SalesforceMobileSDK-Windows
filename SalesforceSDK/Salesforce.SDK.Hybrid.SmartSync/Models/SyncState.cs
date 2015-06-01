using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Salesforce.SDK.Auth;
using Salesforce.SDK.SmartSync.Model;
using Salesforce.SDK.SmartSync.Util;

namespace Salesforce.SDK.Hybrid.SmartSync.Models
{
    public sealed class SyncState
    {
        private SDK.SmartSync.Model.SyncState NativeSyncState
        {
            get { return new SDK.SmartSync.Model.SyncState(); }
        }


        public long Id 
        {
            get { return NativeSyncState.Id; }
        }
        public SyncTypes SyncType
        {
            get
            {
                var nativeSyncType = NativeSyncState.SyncType;
                var hybridSyncType = JsonConvert.SerializeObject(nativeSyncType);
                return JsonConvert.DeserializeObject<SyncTypes>(hybridSyncType);
            }
        }
        //public SyncTarget Target { private set; get; }
        public SyncOptions Options {
            get
            {
                var nativeSyncOptions = NativeSyncState.Options;
                var hybridSyncOptions = JsonConvert.SerializeObject(nativeSyncOptions);
                return JsonConvert.DeserializeObject<SyncOptions>(hybridSyncOptions);
            }
        }
        public String SoupName {
            get { return NativeSyncState.SoupName; }
        }
        public SyncStatusTypes Status {
            get
            {
                var nativeStatus = NativeSyncState.Status;
                var hybridStatus = JsonConvert.SerializeObject(nativeStatus);
                return JsonConvert.DeserializeObject<SyncStatusTypes>(hybridStatus);

            }
        }
        public int Progress {
            get { return NativeSyncState.Progress; }
        }
        public int TotalSize {
            get { return NativeSyncState.TotalSize; }
        }
        public long MaxTimeStamp {
            get { return NativeSyncState.MaxTimeStamp; }
        }

        public MergeModeOptions MergeMode
        {
            get
            {
                var mode = NativeSyncState.MergeMode;
                MergeModeOptions mergeMode;
                Enum.TryParse(mode.ToString(), out mergeMode);
                return mergeMode;
            }
        }

        public static void SetupSyncsSoupIfNeeded(SmartStore.SmartStore store)
        {
            var nativeStore = JsonConvert.SerializeObject(store);
            SDK.SmartSync.Model.SyncState.SetupSyncsSoupIfNeeded(JsonConvert.DeserializeObject<SDK.SmartStore.Store.SmartStore>(nativeStore));
        }

        public static SyncState CreateSyncDown(SmartStore.SmartStore store, string target,
            string soupName, SyncOptions options)
        {
            var nativeStore = JsonConvert.SerializeObject(store);
            var jObject = JsonConvert.DeserializeObject<SDK.SmartSync.Model.SyncDownTarget>(target);
            var nativeOptions = JsonConvert.SerializeObject(options);
            var state = SDK.SmartSync.Model.SyncState.CreateSyncDown(
                JsonConvert.DeserializeObject<SDK.SmartStore.Store.SmartStore>(nativeStore), jObject, soupName, JsonConvert.DeserializeObject<SDK.SmartSync.Model.SyncOptions>(nativeOptions));
            var syncState = JsonConvert.SerializeObject(state);
            return JsonConvert.DeserializeObject<SyncState>(syncState);
        }

        public static SyncState CreateSyncUp(SmartStore.SmartStore store, SyncUpTarget target, SyncOptions options,
            string soupName)
        {
            var nativeStore = JsonConvert.SerializeObject(store);
            var syncUpTarget = JsonConvert.SerializeObject(target);
            var nativeOptions = JsonConvert.SerializeObject(options);
            var state =
                SDK.SmartSync.Model.SyncState.CreateSyncUp(
                    JsonConvert.DeserializeObject<SDK.SmartStore.Store.SmartStore>(nativeStore),
                    JsonConvert.DeserializeObject<SDK.SmartSync.Model.SyncUpTarget>(syncUpTarget),
                    JsonConvert.DeserializeObject<SDK.SmartSync.Model.SyncOptions>(nativeOptions), soupName);
            var syncState = JsonConvert.SerializeObject(state);
            return JsonConvert.DeserializeObject<SyncState>(syncState);
        }

        public static SyncState FromJson(string sync)
        {
            var jObject = JsonConvert.DeserializeObject<JObject>(sync);
            var state = SDK.SmartSync.Model.SyncState.FromJson(jObject);
            var syncState = JsonConvert.SerializeObject(state);
            return JsonConvert.DeserializeObject<SyncState>(syncState);
        }

        public static SyncState ById(SmartStore.SmartStore store, long id)
        {
            var nativeStore = JsonConvert.SerializeObject(store);
            var state = SDK.SmartSync.Model.SyncState.ById(
                JsonConvert.DeserializeObject<SDK.SmartStore.Store.SmartStore>(nativeStore), id);
            var nativeState = JsonConvert.SerializeObject(state);
            return JsonConvert.DeserializeObject<SyncState>(nativeState);
        }

        public string AsJson()
        {
            var jObject = NativeSyncState.AsJson();
            return JsonConvert.SerializeObject(jObject);
        }

        public void Save(SmartStore.SmartStore store)
        {
            var nativeStore = JsonConvert.SerializeObject(store);
            NativeSyncState.Save(JsonConvert.DeserializeObject<SDK.SmartStore.Store.SmartStore>(nativeStore));
        }
    }

    public enum MergeModeOptions
    {
        None,
        Overwrite,
        LeaveIfChanged
    }

    public enum SyncStatusTypes
    {
        New,
        Running,
        Done,
        Failed
    }

    public enum SyncTypes
    {
        SyncDown,
        SyncUp
    }
}
