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
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Salesforce.SDK.SmartStore.Store;
using Salesforce.SDK.SmartSync.Manager;
using Salesforce.SDK.SmartSync.Util;

namespace Salesforce.SDK.SmartSync.Model
{
    /// <summary>
    ///     Target for sync u i.e. set of objects to download from server
    /// </summary>
    public abstract class SyncTarget
    {
        public enum QueryTypes
        {
            Mru,
            Sosl,
            Soql,
            Custom
        }

        public const string WindowsImpl = "windowsImpl";
        public const string WindowsImplType = "windowsImplType";

        public QueryTypes QueryType { internal set; get; }
        public int TotalSize { internal set; get; } // set during fetch

        /// <summary>
        ///     Build SyncTarget from json
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static SyncTarget FromJson(JObject target)
        {
            if (target == null) return null;
            var tempTarget = target.ExtractValue<string>(Constants.QueryType);
            var queryType = (QueryTypes) Enum.Parse(typeof (QueryTypes), tempTarget);
            switch (queryType)
            {
                case QueryTypes.Mru:
                    return MruSyncTarget.FromJson(target);
                case QueryTypes.Soql:
                    return SoqlSyncTarget.FromJson(target);
                case QueryTypes.Sosl:
                    return SoslSyncTarget.FromJson(target);
                default:
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
                                if (type.GetTypeInfo().IsSubclassOf(typeof (SyncTarget)))
                                {
                                    MethodInfo method = type.GetTypeInfo().GetDeclaredMethod("FromJson");
                                    return (SyncTarget) method.Invoke(type, new object[] {target});
                                }
                            }
                            catch (Exception)
                            {
                                throw new SmartStoreException("Invalid SyncTarget");
                            }
                        }
                    }
                    break;
            }
            throw new SmartStoreException("Could not generate SyncTarget from json target");
        }

        /// <summary>
        /// </summary>
        /// <returns>json representation of target</returns>
        public abstract JObject AsJson();

        public abstract Task<JArray> StartFetch(SyncManager syncManager, long maxTimeStamp);

        public abstract Task<JArray> ContinueFetch(SyncManager syncManager);

    }
}