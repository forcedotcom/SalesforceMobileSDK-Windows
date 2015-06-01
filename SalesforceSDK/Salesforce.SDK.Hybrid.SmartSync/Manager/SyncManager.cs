using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Salesforce.SDK.Hybrid.Auth;
using Salesforce.SDK.Rest;
using Salesforce.SDK.SmartSync.Model;
using Salesforce.SDK.SmartSync.Util;

namespace Salesforce.SDK.Hybrid.SmartSync
{
    public sealed class SyncManager
    {
        private SDK.SmartSync.Manager.SyncManager _syncManager = SDK.SmartSync.Manager.SyncManager.GetInstance();
        
        public static SyncManager GetInstance()
        {
            return GetInstance(null, null);
        }

        public static SyncManager GetInstance(Account account, string communityId)
        {
            return GetInstance(account, communityId, null);
        }

        public static SyncManager GetInstance(Account account, string communityId, SmartStore.SmartStore smartStore)
        {
            var nativeAccount = JsonConvert.SerializeObject(account);
            var nativeSmartStore = JsonConvert.SerializeObject(smartStore);
            var nativeSyncManager = SDK.SmartSync.Manager.SyncManager.GetInstance(
                JsonConvert.DeserializeObject<SDK.Auth.Account>(nativeAccount), communityId, JsonConvert.DeserializeObject<SDK.SmartStore.Store.SmartStore>(nativeSmartStore));

            var nativeJson = JsonConvert.SerializeObject(nativeSyncManager);
            return JsonConvert.DeserializeObject<SyncManager>(nativeJson);
        }

        public static void Reset()
        {
            SDK.SmartSync.Manager.SyncManager.Reset();
        }

        public Models.SyncState GetSyncStatus(long syncId)
        {
            var state = _syncManager.GetSyncStatus(syncId);
            var syncState = JsonConvert.SerializeObject(state);
            return JsonConvert.DeserializeObject<Models.SyncState>(syncState);
        }

        public Models.SyncState SyncDown(string target, string soupName, string callback,
            Models.SyncOptions options)
        {
            var soqlsyncDownTarget = JObject.Parse(target);
            var soqlsyncDown = new SoqlSyncDownTarget(soqlsyncDownTarget);
            SyncDownTarget syncDown = soqlsyncDown;
            var action = JsonConvert.DeserializeObject<Action<SyncState>>(callback);
            var syncOptions = JsonConvert.SerializeObject(options);
            var state = _syncManager.SyncDown(syncDown, soupName, action, SyncOptions.FromJson(JObject.Parse(syncOptions)));
            var syncState = JsonConvert.SerializeObject(state);
            return JsonConvert.DeserializeObject<Models.SyncState>(syncState);
        }

        public Models.SyncState SyncUp(Models.SyncUpTarget target, Models.SyncOptions options, string soupName, string callback)
        {
            var syncUp = JsonConvert.SerializeObject(target);
            var action = JsonConvert.DeserializeObject<Action<SyncState>>(callback);
            var syncOptions = JsonConvert.SerializeObject(options);
            var state = _syncManager.SyncUp(JsonConvert.DeserializeObject<SyncUpTarget>(syncUp), SyncOptions.FromJson(JObject.Parse(syncOptions)), soupName, action);
            var syncState = JsonConvert.SerializeObject(state);
            return JsonConvert.DeserializeObject<Models.SyncState>(syncState);
        }

        public Models.SyncState ReSync(long syncId, string callback)
        {
            var action = JsonConvert.DeserializeObject<Action<SyncState>>(callback);
            var state = _syncManager.ReSync(syncId, action);
            var syncState = JsonConvert.SerializeObject(state);
            return JsonConvert.DeserializeObject<Models.SyncState>(syncState);
        }

        public void RunSync(Models.SyncState sync, string callback)
        {
            var action = JsonConvert.DeserializeObject<Action<SyncState>>(callback);
            var state = JsonConvert.SerializeObject(sync);
            _syncManager.RunSync(JsonConvert.DeserializeObject<SyncState>(state), action);
        }

        public static IList<string> Pluck(string jArray, string key)
        {
            var array = JsonConvert.DeserializeObject<JToken>(jArray);
            var list = SDK.SmartSync.Manager.SyncManager.Pluck<string>(array, key);
            return list;
        }

        public IAsyncOperation<Rest.RestResponse> SendRestRequest(Rest.RestRequest request)
        {
            var restRequest = JsonConvert.SerializeObject(request);
            return Task.Run(async () =>
            {
                var response =
                    await _syncManager.SendRestRequest(JsonConvert.DeserializeObject<RestRequest>(restRequest));
                var restResponse = JsonConvert.SerializeObject(response);
                return JsonConvert.DeserializeObject<Rest.RestResponse>(restResponse);
            }).AsAsyncOperation();
        }
    }
}
