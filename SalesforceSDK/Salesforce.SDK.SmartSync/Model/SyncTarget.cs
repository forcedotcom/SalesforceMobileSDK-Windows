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
using Newtonsoft.Json.Linq;
using Salesforce.SDK.SmartSync.Util;

namespace Salesforce.SDK.SmartSync.Model
{
    /// <summary>
    ///     Target for sync u i.e. set of objects to download from server
    /// </summary>
    public class SyncTarget
    {
        public enum QueryTypes
        {
            Mru,
            Sosl,
            Soql
        }

        private SyncTarget(QueryTypes queryType, string query, List<string> fieldList, string objectType)
        {
            QueryType = queryType;
            Query = query;
            FieldList = fieldList;
            ObjectType = objectType;
        }


        public QueryTypes QueryType { private set; get; }
        public string Query { private set; get; }
        public List<string> FieldList { private set; get; }
        public string ObjectType { private set; get; }

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
            var query = target.ExtractValue<string>(Constants.Query);
            var jFieldList = target.ExtractValue<JArray>(Constants.FieldList);
            var fieldList = new List<string>();
            if (jFieldList != null)
            {
                fieldList = jFieldList.ToObject<List<string>>();
            }
            var objectType = target.ExtractValue<string>(Constants.SObjectType);
            return new SyncTarget(queryType, query, fieldList, objectType);
        }

        /// <summary>
        /// </summary>
        /// <returns>json representation of target</returns>
        public JObject AsJson()
        {
            var target = new JObject {{Constants.QueryType, QueryType.ToString()}};
            if (!String.IsNullOrWhiteSpace(Query)) target.Add(Constants.Query, Query);
            if (FieldList != null) target.Add(Constants.FieldList, new JArray(FieldList));
            if (!String.IsNullOrWhiteSpace(ObjectType)) target.Add(Constants.SObjectType, ObjectType);
            return target;
        }

        /// <summary>
        ///     Build SyncTarget for soql target
        /// </summary>
        /// <param name="soql"></param>
        /// <returns></returns>
        public static SyncTarget TargetForSOQLSyncDown(string soql)
        {
            return new SyncTarget(QueryTypes.Soql, soql, null, null);
        }

        /// <summary>
        ///     Build SyncTarget for sosl target
        /// </summary>
        /// <param name="sosl"></param>
        /// <returns></returns>
        public static SyncTarget TargetForSOSLSyncDown(string sosl)
        {
            return new SyncTarget(QueryTypes.Sosl, sosl, null, null);
        }

        /// <summary>
        ///     Build SyncTarget for mru target
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="fieldList"></param>
        /// <returns></returns>
        public static SyncTarget TargetForMRUSyncDown(string objectType, List<string> fieldList)
        {
            return new SyncTarget(QueryTypes.Mru, null, fieldList, objectType);
        }
    }
}