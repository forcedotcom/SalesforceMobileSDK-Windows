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
    public sealed class SoqlSyncDownTarget
    {
        private SDK.SmartSync.Model.SoqlSyncDownTarget _soqlSyncDownTarget;

        public SoqlSyncDownTarget()
        {

        }

        public SoqlSyncDownTarget(string query)
        {
            _soqlSyncDownTarget = new SDK.SmartSync.Model.SoqlSyncDownTarget(query);
        }

        public static string FromJson(string target)
        {
            var jObject = JsonConvert.DeserializeObject<JObject>(target);
            var syncDownTarget = SDK.SmartSync.Model.SoqlSyncDownTarget.FromJson(jObject);
            return JsonConvert.SerializeObject(syncDownTarget);
        }

        public string AsJson()
        {
            var jObject = _soqlSyncDownTarget.AsJson();
            return JsonConvert.SerializeObject(jObject);
        }

        public string StartFetch(SyncManager syncManager, long maxTimeStamp)
        {
            var manager = JsonConvert.SerializeObject(syncManager);
            var target = Task.Run(async () => await _soqlSyncDownTarget.StartFetch(JsonConvert.DeserializeObject<SDK.SmartSync.Manager.SyncManager>(manager),
                maxTimeStamp)).AsAsyncOperation();
            var array = target.GetResults();
            return array.ToString();
        }

        public string ContinueFetch(SyncManager syncManager)
        {
            var manager = JsonConvert.SerializeObject(syncManager);
            var target = Task.Run(async () => await _soqlSyncDownTarget.ContinueFetch(JsonConvert.DeserializeObject<SDK.SmartSync.Manager.SyncManager>(manager))).AsAsyncOperation();
            var array = target.GetResults();
            return array.ToString();
        }
    }
}
