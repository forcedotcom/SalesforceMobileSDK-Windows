using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Salesforce.SDK.Rest;
using Salesforce.SDK.SmartStore.Store;
using Salesforce.SDK.SmartSync.Manager;
using Salesforce.SDK.SmartSync.Util;

namespace Salesforce.SDK.SmartSync.Model
{
    public class SyncUpTarget : SyncTarget
    {
        public const string WindowsImpl = "windowsImpl";
        public const string WindowsImplType = "windowsImplType";

        public SyncUpTarget(JObject target) : base(target)
        {
        }

        public SyncUpTarget()
        {

        }

        public static SyncUpTarget FromJSON(JObject target)
        {
            if (target == null)
            {
                return null;
            }
            // Default sync up target
            JToken impl;
            if (target.TryGetValue(WindowsImpl, out impl))
            {
                JToken implType;
                if (target.TryGetValue(WindowsImplType, out implType))
                {
                    try
                    {
                        Assembly assembly = Assembly.Load(new AssemblyName(impl.ToObject<string>()));
                        Type type = assembly.GetType(implType.ToObject<string>());
                        if (type.GetTypeInfo().IsSubclassOf(typeof(SyncTarget)))
                        {
                            return new SyncUpTarget(target);
                        }
                    }
                    catch (Exception)
                    {
                        throw new SmartStoreException("Invalid SyncUpTarget");
                    }
                }
            }
            throw new SmartStoreException("Could not generate SyncUpTarget from json target");
        }

        public override JObject AsJson()
        {
            var target = new JObject();
            target[WindowsImpl] = GetType().GetTypeInfo().Assembly.FullName;
            target[WindowsImplType] = GetType().GetTypeInfo().FullName;
            return target;
        }

        public async Task<String> CreateOnServerAsync(SyncManager syncManager, String objectType, Dictionary<String, Object> fields)
        {
            JToken id;
            var request = RestRequest.GetRequestForCreate(syncManager.ApiVersion, objectType, fields);
            var response = await syncManager.SendRestRequest(request);
            var responseObject = response.AsJObject;
            responseObject.TryGetValue(Constants.Lid, out id);
            if (id != null)
            {
                return response.Success ? id.ToString() : null;
            }
            return null;
        }

        public async Task<bool> DeleteOnServer(SyncManager syncManager, String objectType, String objectId)
        {
            var request = RestRequest.GetRequestForDelete(syncManager.ApiVersion, objectType, objectId);
            var response = await syncManager.SendRestRequest(request);

            return response.Success;
        }

        public async Task<bool> UpdateOnServer(SyncManager syncManager, String objectType, String objectId, Dictionary<String, Object> fields)
        {
           var request = RestRequest.GetRequestForUpdate(syncManager.ApiVersion, objectType, objectId, fields);
           var response = await syncManager.SendRestRequest(request);

            return response.Success;
        }

        public async Task<String> FetchLastModifiedDate(SyncManager syncManager, String objectType, String objectId)
        {
            String query = SOQLBuilder.GetInstanceWithFields(Constants.LastModifiedDate)
                .From(objectType)
                .Where(Constants.Id + " = '" + objectId + "'")
                .Build();

            var lastModResponse = await syncManager.SendRestRequest(RestRequest.GetRequestForQuery(syncManager.ApiVersion, query));
            if (lastModResponse.Success)
            {
                return
                    lastModResponse.AsJObject.ExtractValue<JArray>(Constants.Records)[0].Value<JObject>()
                        .ExtractValue<string>(Constants.LastModifiedDate);
            }
            return null;
        }

        public HashSet<String> GetIdsOfRecordsToSyncUp(SyncManager syncManager, String soupName)
        {
            return syncManager.GetDirtyRecordIds(soupName, SmartStore.Store.SmartStore.SoupEntryId);
        }
    }
}
