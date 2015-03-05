/*
 * Copyright (c) 2015, salesforce.com, inc.
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
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Web.Http;
using Newtonsoft.Json.Linq;
using Salesforce.SDK.Net;
using Salesforce.SDK.Rest;
using Salesforce.SDK.SmartSync.Manager;
using Salesforce.SDK.SmartSync.Model;
using Salesforce.SDK.SmartSync.Util;

namespace NoteSync.Data
{
    public class ContentSoqlSyncTarget : SoqlSyncTarget
    {
        public const string RequestTemplate = "<?xml version=\"1.0\"?>" +
                                              "<se:Envelope xmlns:se=\"http://schemas.xmlsoap.org/soap/envelope/\">" +
                                              "<se:Header xmlns:sfns=\"urn:partner.soap.sforce.com\">" +
                                              "<sfns:SessionHeader><sessionId>{0}</sessionId></sfns:SessionHeader>" +
                                              "</se:Header>" +
                                              "<se:Body>{1}</se:Body>" +
                                              "</se:Envelope>";

        public const string QueryTemplate =
            "<query xmlns=\"urn:partner.soap.sforce.com\" xmlns:ns1=\"sobject.partner.soap.sforce.com\">" +
            "<queryString>{0}</queryString></query>";

        public const string QueryMoreTemplate =
            "<queryMore xmlns=\"urn:partner.soap.sforce.com\" xmlns:ns1=\"sobject.partner.soap.sforce.com\">\n" +
            "        <queryLocator>{0}</queryLocator>\n" +
            "    </queryMore>";

        public const string Result = "result";
        public const string Records = "records";
        public const string Sf = "sf:";
        public const string QueryLocator = "queryLocator";
        public const string Size = "size";
        public const string Done = "done";
        public const string Type = "type";

        private string _queryLocator;

        public ContentSoqlSyncTarget(String query) : base(query)
        {
            QueryType = QueryTypes.Custom;
            _queryLocator = null;
        }

        public new static SyncTarget FromJson(JObject target)
        {
            if (target == null)
                return null;

            var query = target.ExtractValue<string>(Constants.Query);
            return new ContentSoqlSyncTarget(query);
        }

        /// <summary>
        ///     Build SyncTarget for soql target
        /// </summary>
        /// <param name="soql"></param>
        /// <returns></returns>
        public new static ContentSoqlSyncTarget TargetForSOQLSyncDown(string soql)
        {
            return new ContentSoqlSyncTarget(soql);
        }

        /// <summary>
        ///     return json representation of target
        /// </summary>
        /// <returns></returns>
        public override JObject AsJson()
        {
            JObject target = base.AsJson();
            target[WindowsImpl] = GetType().GetTypeInfo().Assembly.FullName;
            target[WindowsImplType] = GetType().GetTypeInfo().FullName;
            return target;
        }

        public override async Task<JArray> StartFetch(SyncManager syncManager, long maxTimeStamp)
        {
            string queryToRun = maxTimeStamp > 0 ? AddFilterForReSync(Query, maxTimeStamp) : Query;
            await syncManager.SendRestRequest(RestRequest.GetRequestForResources("v33.0"));
            // cheap call to refresh session
            RestRequest request = BuildQueryRequest(syncManager.RestClient.AccessToken, queryToRun);
            RestResponse response = await syncManager.SendRestRequest(request);
            JArray records = ParseSoapResponse(response);

            return records;
        }

        public override async Task<JArray> ContinueFetch(SyncManager syncManager)
        {
            if (String.IsNullOrEmpty(_queryLocator))
            {
                return null;
            }
            RestRequest request = BuildQueryMoreRequest(syncManager.RestClient.AccessToken, _queryLocator);
            RestResponse response = await syncManager.SendRestRequest(request);
            JArray records = ParseSoapResponse(response);
            return records;
        }

        private RestRequest BuildQueryRequest(string sessionId, string query)
        {
            return BuildSoapRequest(sessionId, String.Format(QueryTemplate, query));
        }

        private RestRequest BuildQueryMoreRequest(string sessionId, string locator)
        {
            return BuildSoapRequest(sessionId, String.Format(QueryMoreTemplate, locator));
        }

        /**
     *
     * @param sessionId
     * @param body
     * @return request for soap call
     * @throws UnsupportedEncodingException
     */

        private RestRequest BuildSoapRequest(string sessionId, string body)
        {
            var customHeaders = new Dictionary<String, String>();
            customHeaders["SOAPAction"] = "\"\"";

            string entity = String.Format(RequestTemplate, sessionId, body);

            return new RestRequest(HttpMethod.Post, "/services/Soap/u/33.0", entity, ContentTypeValues.Xml,
                customHeaders);
        }

        private JArray ParseSoapResponse(RestResponse response)
        {
            var records = new JArray();
            XDocument doc = XDocument.Parse(response.AsString);
            XElement qLocator = doc.Descendants().FirstOrDefault(n => QueryLocator.Equals(n.Name.LocalName));
            _queryLocator = (qLocator != null ? qLocator.Value : null);

            XElement xmlRecords = doc.Descendants().FirstOrDefault(n => Records.Equals(n.Name.LocalName));
            if (xmlRecords != null)
            {
                var jRecord = new JObject();
                foreach (XNode next in xmlRecords.DescendantNodes())
                {
                    var record = next as XElement;

                    if (record != null)
                    {
                        if (Type.Equals(record.Name.LocalName))
                        {
                            var attrType = new JObject {{Type, record.Value}};
                            jRecord[Constants.Attributes] = attrType;
                        }
                        else
                        {
                            jRecord[record.Name.LocalName] = record.Value;
                        }
                    }
                }
                records.Add(jRecord);
            }
            return records;
        }
    }
}