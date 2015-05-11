using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Salesforce.SDK.Auth;

namespace Salesforce.SDK.Hybrid.SmartSync.Models
{
    public sealed class MruSyncDownTarget
    {
        private SDK.SmartSync.Model.MruSyncDownTarget _mruSyncDownTarget;

        public MruSyncDownTarget(string target)
        {
            _mruSyncDownTarget = new SDK.SmartSync.Model.MruSyncDownTarget(JsonConvert.DeserializeObject<JObject>(target));
        }

        public string AsJson()
        {
            var target = _mruSyncDownTarget.AsJson();
            return JsonConvert.SerializeObject(target);
        }

        /// <summary>
        ///     Build SyncTarget for mru target
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="fieldList"></param>
        /// <returns></returns>
        public static string TargetForMruSyncDown(string objectType, IList<string> fieldList)
        {
            var target = SDK.SmartSync.Model.MruSyncDownTarget.TargetForMruSyncDown(objectType, fieldList.ToList());
            return JsonConvert.SerializeObject(target);
        }

        public string StartFetch(SyncManager syncManager, long maxTimeStamp)
        {
            var manager = JsonConvert.SerializeObject(syncManager);
            var target = Task.Run(async () => await _mruSyncDownTarget.StartFetch(JsonConvert.DeserializeObject<SDK.SmartSync.Manager.SyncManager>(manager),
                maxTimeStamp)).AsAsyncOperation();
            var array = target.GetResults();
            return array.ToString();
        }

        public string ContinueFetch(SyncManager syncManager)
        {
            var manager = JsonConvert.SerializeObject(syncManager);
            var result = _mruSyncDownTarget.ContinueFetch(JsonConvert.DeserializeObject<SDK.SmartSync.Manager.SyncManager>(manager));
            return result.Result.ToString();
        }
    }
}
