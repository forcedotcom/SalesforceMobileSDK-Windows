using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Salesforce.SDK.SmartSync.Util;

namespace Salesforce.SDK.Hybrid.SmartSync.Models
{
    public sealed class SyncOptions
    {
        [JsonProperty]
        internal MergeModeOptions MergeMode;

        [JsonProperty]
        internal List<string> FieldList;
        
        
        public static SyncOptions FromJson(string options)
        {
            var jObject = JsonConvert.DeserializeObject<JObject>(options);
            var nativeSyncOptions = SDK.SmartSync.Model.SyncOptions.FromJson(jObject);
            var syncOptions = JsonConvert.SerializeObject(nativeSyncOptions);
            return JsonConvert.DeserializeObject<SyncOptions>(syncOptions);
        }

        public static SyncOptions OptionsForSyncUp(IList<string> fieldList, MergeModeOptions mergeMode)
        {
            var nativeMergeMode = JsonConvert.SerializeObject(mergeMode);
            var nativeSyncOptions = SDK.SmartSync.Model.SyncOptions.OptionsForSyncUp(fieldList.ToList(), JsonConvert.DeserializeObject<SDK.SmartSync.Model.SyncState.MergeModeOptions>(nativeMergeMode));
            var syncOptions = JsonConvert.SerializeObject(nativeSyncOptions);
            return JsonConvert.DeserializeObject<SyncOptions>(syncOptions);
        }

        public static SyncOptions OptionsForSyncDown(MergeModeOptions mergeMode)
        {
            var nativeMergeMode = JsonConvert.SerializeObject(mergeMode);
            var nativeSyncOptions = SDK.SmartSync.Model.SyncOptions.OptionsForSyncDown(JsonConvert.DeserializeObject<SDK.SmartSync.Model.SyncState.MergeModeOptions>(nativeMergeMode));
            var syncOptions = JsonConvert.SerializeObject(nativeSyncOptions);
            return JsonConvert.DeserializeObject<SyncOptions>(syncOptions);
        }
    }
}
