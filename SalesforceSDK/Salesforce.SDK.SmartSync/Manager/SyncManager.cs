/*
 * Copyright (c) 2014, salesforce.com, inc.
 * All rights reserved.
 * Redistribution and use of this software in source and binary forms, with or
 * without modification, are permitted provided that the following conditions
 * are met:
 * - Redistributions of source code must retain the above copyright notice, this
 * list of conditions and the following disclaimer.
 * - Redistributions in binary form must reproduce the above copyright notice,
 * this list of conditions and the following disclaimer in the documentation
 * and/or other materials provided with the distribution.
 * - Neither the name of salesforce.com, inc. nor the names of its contributors
 * may be used to endorse or promote products derived from this software without
 * specific prior written permission of salesforce.com, inc.
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Web.Http;
using Newtonsoft.Json.Linq;
using Salesforce.SDK.App;
using Salesforce.SDK.Auth;
using Salesforce.SDK.Rest;
using Salesforce.SDK.SmartStore.Store;
using Salesforce.SDK.SmartSync.Model;
using Salesforce.SDK.SmartSync.Util;

namespace Salesforce.SDK.SmartSync.Manager
{
    public class SyncManager
    {
        public const string Local = "__local__";
        public const string LocallyCreated = "__locally_created__";
        public const string LocallyUpdated = "__locally_updated__";
        public const string LocallyDeleted = "__locally_deleted__";
        private static volatile Dictionary<string, SyncManager> _instances;
        private static readonly object Synclock = new Object();
        private readonly string _apiVersion;
        private readonly RestClient _restClient;
        private readonly SmartStore.Store.SmartStore _smartStore;

        private SyncManager(Account account, string communityId)
        {
            _smartStore = new SmartStore.Store.SmartStore();
            _restClient = SalesforceApplication.GlobalClientManager.GetRestClient();
            _apiVersion = ApiVersionStrings.VersionNumber;
            SyncState.SetupSyncsSoupIfNeeded(_smartStore);
        }

        /// <summary>
        ///     Returns the instance of this class associated with this user.
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static SyncManager GetInstance(Account account)
        {
            return GetInstance(account, null);
        }

        /// <summary>
        ///     Returns the instance of this class associated with this user and community.
        /// </summary>
        /// <param name="account"></param>
        /// <param name="communityId"></param>
        /// <returns></returns>
        public static SyncManager GetInstance(Account account, string communityId)
        {
            if (account == null)
            {
                account = AccountManager.GetAccount();
            }
            if (account == null)
            {
                return null;
            }
            string uniqueId = Constants.GenerateAccountCommunityId(account, communityId);
            lock (Synclock)
            {
                SyncManager instance = null;
                if (_instances != null)
                {
                    if (_instances.TryGetValue(uniqueId, out instance))
                    {
                        SyncState.SetupSyncsSoupIfNeeded(instance._smartStore);
                        return instance;
                    }
                    instance = new SyncManager(account, communityId);
                    _instances.Add(uniqueId, instance);
                }
                else
                {
                    _instances = new Dictionary<string, SyncManager>();
                    instance = new SyncManager(account, communityId);
                    _instances.Add(uniqueId, instance);
                }
                SyncState.SetupSyncsSoupIfNeeded(instance._smartStore);
                return instance;
            }
        }

        /// <summary>
        ///     Resets the Sync manager associated with this user.
        /// </summary>
        /// <param name="account"></param>
        public static void Reset(Account account)
        {
            Reset(account, null);
        }

        /// <summary>
        ///     Resets the Sync manager associated with this user and community.
        /// </summary>
        /// <param name="account"></param>
        /// <param name="communityId"></param>
        public static void Reset(Account account, string communityId)
        {
            if (account == null)
            {
                account = AccountManager.GetAccount();
            }
            if (account != null)
            {
                lock (Synclock)
                {
                    SyncManager instance = GetInstance(account, communityId);
                    if (instance == null) return;
                    _instances.Remove(Constants.GenerateAccountCommunityId(account, communityId));
                }
            }
        }

        /// <summary>
        ///     Get details of a sync state.
        /// </summary>
        /// <param name="syncId"></param>
        /// <returns></returns>
        public SyncState GetSyncStatus(long syncId)
        {
            return SyncState.ById(_smartStore, syncId);
        }

        /// <summary>
        ///     Create and run a sync down.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="soupName"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public SyncState SyncDown(SyncTarget target, string soupName, Action<SyncState> callback)
        {
            SyncState sync = SyncState.CreateSyncDown(_smartStore, target, soupName);
            RunSync(sync, callback);
            return sync;
        }

        /// <summary>
        ///     Create and run a sync up.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="soupName"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public SyncState SyncUp(SyncOptions options, string soupName, Action<SyncState> callback)
        {
            SyncState sync = SyncState.CreateSyncUp(_smartStore, options, soupName);
            RunSync(sync, callback);
            return sync;
        }

        private async void RunSync(SyncState sync, Action<SyncState> callback)
        {
            UpdateSync(sync, SyncState.SyncStatusTypes.Running, 0, -1 /* don't change */, callback);
           try
                {
                    switch (sync.SyncType)
                    {
                        case SyncState.SyncTypes.SyncDown:
                            await SyncDown(sync, callback);
                            break;
                        case SyncState.SyncTypes.SyncUp:
                            await SyncUp(sync, callback);
                            break;
                    }
                    UpdateSync(sync, SyncState.SyncStatusTypes.Done, 100, -1 /* don't change */, callback);
                }
                catch (Exception)
                {
                    Debug.WriteLine("SmartSyncManager:runSync, Error during sync: " + sync.Id);
                    UpdateSync(sync, SyncState.SyncStatusTypes.Failed, -1 /* don't change */, -1 /* don't change */, callback);
                }
        }

        private async Task<int> SyncUp(SyncState sync, Action<SyncState> callback)
        {
            if (sync == null)
                throw new SmartStoreException("SyncState sync was null");
            QuerySpec querySpec = QuerySpec.BuildExactQuerySpec(sync.SoupName, Local, "True", 2000);
            JArray records = _smartStore.Query(querySpec, 0);
            int totalSize = records.Count;
            int progress = 0;
            UpdateSync(sync, SyncState.SyncStatusTypes.Running, 0, totalSize, callback);
            _smartStore.Database.BeginTransaction();
            for (int i = 0; i < totalSize; i++)
            {
                var record = records[i].Value<JObject>();
                SyncAction action;
                JToken hasAction;

                if (record.TryGetValue(LocallyDeleted, out hasAction) && hasAction.Value<bool>())
                {
                    action = SyncAction.Delete;
                }
                else if (record.TryGetValue(LocallyCreated, out hasAction) && hasAction.Value<bool>())
                {
                    action = SyncAction.Create;
                }
                else if (record.TryGetValue(LocallyUpdated, out hasAction) && hasAction.Value<bool>())
                {
                    action = SyncAction.Update;
                }
                else continue;

                var objectType = SmartStore.Store.SmartStore.Project(record, Constants.SobjectType).ToString();
                var objectId = record.ExtractValue<string>(Constants.Id);

                var fields = new Dictionary<string, object>();

                if (SyncAction.Create == action || SyncAction.Update == action)
                {
                    foreach (string fieldName in sync.Options.FieldList)
                    {
                        if (!fieldName.Equals(Constants.Id))
                        {
                            fields.Add(fieldName, record[fieldName]);
                        }
                    }
                }

                RestRequest request = null;

                switch (action)
                {
                    case SyncAction.Create:
                        request = RestRequest.GetRequestForCreate(_apiVersion, objectType, fields);
                        break;
                    case SyncAction.Update:
                        request = RestRequest.GetRequestForUpdate(_apiVersion, objectType, objectId, fields);
                        break;
                    case SyncAction.Delete:
                        request = RestRequest.GetRequestForDelete(_apiVersion, objectType, objectId);
                        break;
                }


                RestResponse response = await _restClient.SendAsync(request);

                if (response.Success)
                {
                    if (SyncAction.Create == action)
                    {
                        record[Constants.Id] = response.AsJObject[Constants.Id];
                    }
                    record[Local] = false;
                    record[LocallyCreated] = false;
                    record[LocallyUpdated] = false;
                    record[LocallyUpdated] = false;

                    if (SyncAction.Delete == action)
                    {
                        _smartStore.Delete(sync.SoupName,
                            new[] { record.ExtractValue<long>(SmartStore.Store.SmartStore.SoupEntryId) }, false);
                    }
                    else
                    {
                        _smartStore.Update(sync.SoupName, record, record.ExtractValue<long>(SmartStore.Store.SmartStore.SoupEntryId), false);
                    }
                }
                else if (HttpStatusCode.NotFound == response.StatusCode)
                {
                    _smartStore.Delete(sync.SoupName,
                            new[] { record.ExtractValue<long>(SmartStore.Store.SmartStore.SoupEntryId) }, false);
                }

                progress = (i + 1)*100/totalSize;
                if (progress < 100)
                {
                    UpdateSync(sync, SyncState.SyncStatusTypes.Running, progress, -1 /* don't change */, callback);
                }
            }
            _smartStore.Database.CommitTransaction();
            return progress;
        }

        private async Task<bool> SyncDown(SyncState sync, Action<SyncState> callback)
        {
            switch (sync.Target.QueryType)
            {
                case SyncTarget.QueryTypes.Mru:
                    await SyncDownMru(sync, callback);
                    break;
                case SyncTarget.QueryTypes.Soql:
                    await SyncDownSoql(sync, callback);
                    break;
                case SyncTarget.QueryTypes.Sosl:
                    await SyncDownSosl(sync, callback);
                    break;
            }
            return true;
        }

        private async Task<int> SyncDownMru(SyncState sync, Action<SyncState> callback)
        {
            SyncTarget target = sync.Target;
            // Get recent items ids from server
            RestRequest request = RestRequest.GetRequestForMetadata(_apiVersion, target.ObjectType);
            RestResponse response = await _restClient.SendAsync(request);
            List<string> recentItems = Pluck<string>(response.AsJObject.ExtractValue<JArray>(Constants.RecentItems),
                Constants.Id);

            // Building SOQL query to get requested at
            String soql =
                SOQLBuilder.GetInstanceWithFields(target.FieldList.ToArray())
                    .From(target.ObjectType)
                    .Where("Id IN ('" + String.Join("', '", recentItems) + "')")
                    .Build();

            // Get recent items attributes from server
            request = RestRequest.GetRequestForQuery(_apiVersion, soql);
            response = await _restClient.SendAsync(request);
            JObject responseJson = response.AsJObject;
            var records = responseJson.ExtractValue<JArray>(Constants.Records);
            int totalSize = records.Count;

            // Save to smartstore
            UpdateSync(sync, SyncState.SyncStatusTypes.Running, 0, totalSize, callback);
            if (totalSize > 0)
            {
                SaveRecordsToSmartStore(sync.SoupName, records);
            }
            return totalSize;
        }

        private async Task<bool> SyncDownSoql(SyncState sync, Action<SyncState> callback)
        {
            string soupName = sync.SoupName;
            SyncTarget target = sync.Target;
            string query = target.Query;
            RestRequest request = RestRequest.GetRequestForQuery(_apiVersion, query);

            // Call server
            RestResponse response;
            try
            {
                response = await _restClient.SendAsync(request);
            }
            catch (Exception)
            {
                UpdateSync(sync, SyncState.SyncStatusTypes.Failed, 0, -1, callback);
                return false;
            }
            JObject responseJson = response.AsJObject;

            int countSaved = 0;
            var totalSize = responseJson.ExtractValue<int>(Constants.TotalSize);
            UpdateSync(sync, SyncState.SyncStatusTypes.Running, 0, totalSize, callback);

            do
            {
                var records = responseJson.ExtractValue<JArray>(Constants.Records);
                // Save to smartstore
                SaveRecordsToSmartStore(soupName, records);
                countSaved += records.Count;

                // Update sync status
                if (countSaved < totalSize)
                {
                    UpdateSync(sync, SyncState.SyncStatusTypes.Running, countSaved * 100 / totalSize, -1 /* don't change */, callback);
                }


                // Fetch next records if any
                var nextRecordsUrl = responseJson.ExtractValue<string>(Constants.NextRecordsUrl);
                responseJson = null;
                if (!String.IsNullOrWhiteSpace(nextRecordsUrl))
                {
                    var result = await _restClient.SendAsync(HttpMethod.Get, nextRecordsUrl);
                    if (result != null)
                    {
                        responseJson = result.AsJObject;
                    }
                }
            } while (responseJson != null);
            return true;
        }

        private async Task<int> SyncDownSosl(SyncState sync, Action<SyncState> callback)
        {
            SyncTarget target = sync.Target;
            RestRequest request = RestRequest.GetRequestForSearch(_apiVersion, target.Query);

            // Call server
            RestResponse response = await _restClient.SendAsync(request);

            // Parse response
            JArray records = response.AsJArray;
            int totalSize = records.Count;

            // Save to smartstore
            UpdateSync(sync, SyncState.SyncStatusTypes.Running, 0, totalSize, callback);
            if (totalSize > 0)
            {
                SaveRecordsToSmartStore(sync.SoupName, records);
            }
            return totalSize;
        }

        private static List<T> Pluck<T>(IEnumerable<JToken> jArray, string key)
        {
            return jArray.Select(t => t.ToObject<JObject>().Value<T>(key)).ToList();
        }

        private void SaveRecordsToSmartStore(string soupName, IEnumerable<JToken> records)
        {
            _smartStore.Database.BeginTransaction();
            foreach (JObject record in records.Select(t => t.ToObject<JObject>()))
            {
                record[Local] = false;
                record[LocallyCreated] = false;
                record[LocallyUpdated] = false;
                record[LocallyUpdated] = false;
                _smartStore.Upsert(soupName, record, Constants.Id, false);
            }
            _smartStore.Database.CommitTransaction();
        }

        private void UpdateSync(SyncState sync, SyncState.SyncStatusTypes status, int progress, int totalSize, Action<SyncState> callback)
        {
            if (sync == null)
                return;
            sync.Status = status;
            if (progress != -1) sync.Progress = progress;
            if (totalSize != -1) sync.TotalSize = totalSize;
            try
            {
                sync.Save(_smartStore);
            }
            catch (SmartStoreException)
            {    
                sync.Status = SyncState.SyncStatusTypes.Failed;
            }

            if (callback == null) return;
            callback(sync);
        }

        private enum SyncAction
        {
            Create,
            Update,
            Delete
        }
    }
}