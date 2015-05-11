using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Salesforce.SDK.Hybrid.SmartSync.Models
{
    public sealed class SoslSyncDownTarget
    {
        private SDK.SmartSync.Model.SoslSyncDownTarget _soslSyncDownTarget;

        public SoslSyncDownTarget(string query)
        {
            _soslSyncDownTarget = new SDK.SmartSync.Model.SoslSyncDownTarget(query);
        }

        public string AsJson()
        {
            return JsonConvert.SerializeObject(_soslSyncDownTarget.AsJson());
        }

        public string StartFetch(SyncManager syncManager, long maxTimeStamp)
        {
            var manager = JsonConvert.SerializeObject(syncManager);
            var target = Task.Run(async () => await _soslSyncDownTarget.StartFetch(JsonConvert.DeserializeObject<SDK.SmartSync.Manager.SyncManager>(manager),
                maxTimeStamp)).AsAsyncOperation();
            var array = target.GetResults();
            return array.ToString();
        }

        public string ContinueFetch(SyncManager syncManager)
        {
            var manager = JsonConvert.SerializeObject(syncManager);
            var result = _soslSyncDownTarget.ContinueFetch(JsonConvert.DeserializeObject<SDK.SmartSync.Manager.SyncManager>(manager));
            return result.Result.ToString();
        }

    }
}
