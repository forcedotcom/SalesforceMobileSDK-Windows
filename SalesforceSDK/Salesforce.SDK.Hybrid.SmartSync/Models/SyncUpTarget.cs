using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Salesforce.SDK.Hybrid.SmartSync.Models
{
    public sealed class SyncUpTarget
    {
         private SDK.SmartSync.Model.SyncUpTarget _syncUpTarget;

        public SyncUpTarget(string target)
        {
            _syncUpTarget = new SDK.SmartSync.Model.SyncUpTarget(JsonConvert.DeserializeObject<JObject>(target));
        }

        public static SyncUpTarget FromJSON(string target)
        {
            var nativeSyncUpTarget = SDK.SmartSync.Model.SyncUpTarget.FromJSON(JsonConvert.DeserializeObject<JObject>(target));
            var syncUpTarget = JsonConvert.SerializeObject(nativeSyncUpTarget);
            return JsonConvert.DeserializeObject<SyncUpTarget>(syncUpTarget);
        }

        public IAsyncOperation<String> CreateOnServerAsync(SyncManager syncManager, String objectType,
            IDictionary<String, Object> fields)
        {
            var manager = JsonConvert.SerializeObject(syncManager);
            var fieldList = new Dictionary<String, Object>(fields);
            return Task.Run(async() => await _syncUpTarget.CreateOnServerAsync(JsonConvert.DeserializeObject<SDK.SmartSync.Manager.SyncManager>(manager), objectType, fieldList)).AsAsyncOperation();
        }

        public IAsyncOperation<bool> DeleteOnServer(SyncManager syncManager, String objectType, String objectId)
        {
            var manager = JsonConvert.SerializeObject(syncManager);
            return
                Task.Run(
                    async () =>
                        await
                            _syncUpTarget.DeleteOnServer(
                                JsonConvert.DeserializeObject<SDK.SmartSync.Manager.SyncManager>(manager), objectType,
                                objectId)).AsAsyncOperation();
        }

        public IAsyncOperation<bool> UpdateOnServer(SyncManager syncManager, String objectType, String objectId,
            IDictionary<String, Object> fields)
        {
            var manager = JsonConvert.SerializeObject(syncManager);
            var fieldList = new Dictionary<String, Object>(fields);
            return
                Task.Run(
                    async () =>
                        await
                            _syncUpTarget.UpdateOnServer(
                                JsonConvert.DeserializeObject<SDK.SmartSync.Manager.SyncManager>(manager), objectType,
                                objectId, fieldList)).AsAsyncOperation();
        }

        public IAsyncOperation<String> FetchLastModifiedDate(SyncManager syncManager, String objectType, String objectId)
        {
            var manager = JsonConvert.SerializeObject(syncManager);
            return
                Task.Run(
                    async () =>
                        await
                            _syncUpTarget.FetchLastModifiedDate(
                                JsonConvert.DeserializeObject<SDK.SmartSync.Manager.SyncManager>(manager), objectType,
                                objectId)).AsAsyncOperation();
        }

        public object GetIdsOfRecordsToSyncUp(SyncManager syncManager, String soupName)
        {
            var manager = JsonConvert.SerializeObject(syncManager);
            return _syncUpTarget.GetIdsOfRecordsToSyncUp(
                JsonConvert.DeserializeObject<SDK.SmartSync.Manager.SyncManager>(manager), soupName);
        }
    }
}
