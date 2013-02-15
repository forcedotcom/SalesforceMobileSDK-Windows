/*
 * Copyright (c) 2013, salesforce.com, inc.
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

using Newtonsoft.Json;
using Salesforce.SDK.Net;
using System;
using System.Collections.Generic;
using System.Text;

namespace Salesforce.SDK.Rest
{
    public enum RestAction
    {
        VERSIONS,
        RESOURCES,
        DESCRIBE_GLOBAL,
        METADATA,
        DESCRIBE,
        CREATE,
        RETRIEVE,
        UPSERT,
        UPDATE,
        DELETE,
        QUERY,
        SEARCH,
        // Other
        MANUAL
    }

    static class Extensions
    {
        public static string Path(this RestAction action, params string[] args)
        {
            string format = "";
            switch (action)
            {
                case RestAction.VERSIONS: format = "/services/data/"; break;
                case RestAction.RESOURCES: format = "/services/data/{0}/"; break;
                case RestAction.DESCRIBE_GLOBAL: format = "/services/data/{0}/sobjects/"; break;
                case RestAction.METADATA: format = "/services/data/{0}/sobjects/{1}/"; break;
                case RestAction.DESCRIBE: format = "/services/data/{0}/sobjects/{1}/describe/"; break;
                case RestAction.CREATE: format = "/services/data/{0}/sobjects/{1}"; break;
                case RestAction.RETRIEVE: format = "/services/data/{0}/sobjects/{1}/{2}"; break;
                case RestAction.UPSERT: format = "/services/data/{0}/sobjects/{1}/{2}/{3}"; break;
                case RestAction.UPDATE: format = "/services/data/{0}/sobjects/{1}/{2}"; break;
                case RestAction.DELETE: format = "/services/data/{0}/sobjects/{1}/{2}"; break;
                case RestAction.QUERY: format = "/services/data/{0}/query"; break;
                case RestAction.SEARCH: format = "/services/data/{0}/search"; break;
            }

            return string.Format(format, args);
        }
    }

    /// <summary>
    /// RestRequest: Class to represent any REST request.   
    /// </summary>
    /// <remarks>
    /// The class offers factory methods to build RestRequest objects for all REST API actions:
    /// - versions
    /// - resources
    /// - describeGlobal
    /// - metadata
    /// - describe
    /// - create
    /// - retrieve
    /// - update
    /// - upsert
    /// - delete
    /// It also has constructors to build any arbitrary request.
    /// </remarks>
    public class RestRequest
    {

        private readonly RestMethod _method;
        public RestMethod Method
        {
            get { return _method; }
        }

        private readonly string _path;
        public string Path
        {
            get { return _path; }
        }

        private readonly string _requestBody;
        public string Body
        {
            get { return _requestBody; }
        }

        private readonly ContentType _contentType;
        public ContentType ContentType
        {
            get { return _contentType; }
        }

        private readonly Dictionary<string, string> _additionalHeaders;
        public Dictionary<string, string> AdditionalHeaders
        {
            get { return _additionalHeaders; }
        }

        /// <summary>
        /// Generic constructors for arbitrary requests.   
        /// </summary>
        /// <param name="method">The HTTP method for the request (GET/POST/DELETE etc)</param>
        /// <param name="path">The URI path, this will automatically be resolved against the users current instance host.</param>
        /// <param name="requestBody">The request body if there is one, can be null.</param>
        /// <param name="contentType">The content type.</param>
        /// <param name="additionalHeaders">Additional HTTP headers, can be null.</param>
        public RestRequest(RestMethod method, string path) : this(method, path, null, ContentType.NONE, null) { }
        public RestRequest(RestMethod method, string path, string requestBody) : this(method, path, requestBody, ContentType.FORM_URLENCODED, null) { }
        public RestRequest(RestMethod method, string path, string requestBody, ContentType contentType) : this(method, path, requestBody, contentType, null) { }

        public RestRequest(RestMethod method, string path, string requestBody, ContentType contentType, Dictionary<string, string> additionalHeaders)
        {
            _method = method;
            _path = path;
            _requestBody = requestBody;
            _contentType = contentType;
            _additionalHeaders = additionalHeaders;
        }


        public override string ToString()
        {
            return _method + " " + _path;
        }


        /// <summary>
        /// Request to get summary information about each Salesforce.com version currently available.
        /// See http://www.salesforce.com/us/developer/docs/api_rest/index_Left.htm#StartTopic=Content/resources_versions.htm
        /// </summary>
        /// <returns>A RestRequest</returns>
        /// 
        public static RestRequest GetRequestForVersions()
        {
            return new RestRequest(RestMethod.GET, RestAction.VERSIONS.Path());
        }

        /// <summary>
        /// Request to list available resources for the specified API version, including resource name and URI.
        /// See http://www.salesforce.com/us/developer/docs/api_rest/index_Left.htm#StartTopic=Content/resources_discoveryresource.htm
        /// <summary>
        /// <param name="apiVersion">API version e.g. v26.0</param>
        /// <returns>A RestRequest</returns>
        public static RestRequest GetRequestForResources(string apiVersion)
        {
            return new RestRequest(RestMethod.GET, RestAction.RESOURCES.Path(apiVersion));
        }

        /// <summary>
        /// Request to list the available objects and their metadata for your organization's data.
        /// See http://www.salesforce.com/us/developer/docs/api_rest/index_Left.htm#StartTopic=Content/resources_describeGlobal.htm
        /// <param name="apiVersion">API version e.g. v26.0</param>
        /// <returns>A RestRequest</returns>
        public static RestRequest GetRequestForDescribeGlobal(string apiVersion)
        {
            return new RestRequest(RestMethod.GET, RestAction.DESCRIBE_GLOBAL.Path(apiVersion));
        }

        /// <summary>
        /// Request to describe the individual metadata for the specified object.
        /// See http://www.salesforce.com/us/developer/docs/api_rest/index_Left.htm#StartTopic=Content/resources_sobject_basic_info.htm
        /// </summary>
        /// <param name="apiVersion">API version e.g. v26.0</param>
        /// <param name="objectType">Ojbect type</param>
        /// <returns>A RestRequest</returns>
        public static RestRequest GetRequestForMetadata(string apiVersion, string objectType)
        {
            return new RestRequest(RestMethod.GET, RestAction.METADATA.Path(apiVersion, objectType));
        }

        /// <summary>
        /// Request to completely describe the individual metadata at all levels for the specified object. 
        /// See http://www.salesforce.com/us/developer/docs/api_rest/index_Left.htm#StartTopic=Content/resources_sobject_describe.htm
        /// </summary>
        /// <param name="apiVersion">API version e.g. v26.0</param>
        /// <param name="objectType">Ojbect type</param>
        /// <returns>A RestRequest</returns>
        public static RestRequest GetRequestForDescribe(string apiVersion, string objectType)
        {
            return new RestRequest(RestMethod.GET, RestAction.DESCRIBE.Path(apiVersion, objectType));
        }

        /// <summary>
        /// Request to create a record. 
        /// See http://www.salesforce.com/us/developer/docs/api_rest/index_Left.htm#StartTopic=Content/resources_sobject_retrieve.htm
        /// </summary>
        /// <param name="apiVersion">API version e.g. v26.0</param>
        /// <param name="objectType">Ojbect type</param>
        /// <param name="fields">Fields</param>
        /// <returns>A RestRequest</returns>
        public static RestRequest GetRequestForCreate(string apiVersion, string objectType, Dictionary<string, object> fields)
        {
            string fieldsData = (fields == null ? null : JsonConvert.SerializeObject(fields));
            return new RestRequest(RestMethod.POST, RestAction.CREATE.Path(apiVersion, objectType), fieldsData, ContentType.JSON);
        }

        /// <summary>
        /// Request to retrieve a record by object id. 
        /// See http://www.salesforce.com/us/developer/docs/api_rest/index_Left.htm#StartTopic=Content/resources_sobject_retrieve.htm
        /// </summary>
        /// <param name="apiVersion">API version e.g. v26.0</param>
        /// <param name="objectType">Ojbect type</param>
        /// <param name="objectId">object id</param>
        /// <param name="fieldsList">Fields</param>
        /// <returns>A RestRequest</returns>
        public static RestRequest GetRequestForRetrieve(string apiVersion, string objectType, string objectId, string[] fieldList)
        {
            StringBuilder path = new StringBuilder(RestAction.RETRIEVE.Path(apiVersion, objectType, objectId));
            if (fieldList != null && fieldList.Length > 0)
            {
                path.Append("?fields=");
                path.Append(Uri.EscapeUriString(string.Join(",", fieldList)));
            }

            return new RestRequest(RestMethod.GET, path.ToString());
        }

        /// <summary>
        /// Request to update a record. 
        /// See http://www.salesforce.com/us/developer/docs/api_rest/index_Left.htm#StartTopic=Content/resources_sobject_retrieve.htm
        /// <param name="apiVersion">API version e.g. v26.0</param> 
        /// <param name="objectType">Ojbect type</param>
        /// <param name="objectId">object id</param>
        /// <param name="fields">Fields</param>
        /// <returns>A RestRequest</returns>
        public static RestRequest GetRequestForUpdate(string apiVersion, string objectType, string objectId, Dictionary<string, object> fields)
        {
            string fieldsData = (fields == null ? null : JsonConvert.SerializeObject(fields));
            return new RestRequest(RestMethod.PATCH, RestAction.UPDATE.Path(apiVersion, objectType, objectId), fieldsData, ContentType.JSON);
        }


        /// <summary>
        /// Request to upsert (update or insert) a record. 
        /// See http://www.salesforce.com/us/developer/docs/api_rest/index_Left.htm#StartTopic=Content/resources_sobject_retrieve.htm
        /// <param name="apiVersion">API version e.g. v26.0</param>
        /// <param name="objectType">object type</param>
        /// <param name="externalIdField">External id field</param>
        /// <param name="externalId">External id</param>
        /// <param name="fields">Fields</param>
        /// <returns>A RestRequest</returns>
        public static RestRequest GetRequestForUpsert(string apiVersion, string objectType, string externalIdField, string externalId, Dictionary<string, object> fields)
        {
            string fieldsData = (fields == null ? null : JsonConvert.SerializeObject(fields));
            return new RestRequest(RestMethod.PATCH, RestAction.UPSERT.Path(apiVersion, objectType, externalIdField, externalId), fieldsData, ContentType.JSON);
        }

        /// <summary>
        /// Request to delete a record. 
        /// See http://www.salesforce.com/us/developer/docs/api_rest/index_Left.htm#StartTopic=Content/resources_sobject_retrieve.htm
        /// </summary>
        /// <param name="apiVersion">API version e.g. v26.0</param>
        /// <param name="objectType">Ojbect type</param>
        /// <param name="objectId">object id</param>
        /// <returns>A RestRequest</returns>
        public static RestRequest GetRequestForDelete(string apiVersion, string objectType, string objectId)
        {
            return new RestRequest(RestMethod.DELETE, RestAction.DELETE.Path(apiVersion, objectType, objectId));
        }

        /// <summary>
        /// Request to execute the specified SOSL search. 
        /// See http://www.salesforce.com/us/developer/docs/api_rest/index_Left.htm#StartTopic=Content/resources_search.htm
        /// </summary>
        /// <param name="apiVersion">API version e.g. v26.0</param>
        /// <param name="q">Query string</param
        /// <returns>A RestRequest</returns>
        public static RestRequest GetRequestForSearch(string apiVersion, string q)
        {
            StringBuilder path = new StringBuilder(RestAction.SEARCH.Path(apiVersion));
            path.Append("?q=");
            path.Append(Uri.EscapeUriString(q));
            return new RestRequest(RestMethod.GET, path.ToString());
        }

        /// <summary>
        /// Request to execute the specified SOQL search. 
        /// See http://www.salesforce.com/us/developer/docs/api_rest/index_Left.htm#StartTopic=Content/resources_query.htm
        /// </summary>
        /// <param name="apiVersion">API version e.g. v26.0</param>
        /// <param name="q">Query string</param
        /// <returns>A RestRequest</returns>
        public static RestRequest GetRequestForQuery(string apiVersion, string q)
        {
            StringBuilder path = new StringBuilder(RestAction.QUERY.Path(apiVersion));
            path.Append("?q=");
            path.Append(Uri.EscapeUriString(q));
            return new RestRequest(RestMethod.GET, path.ToString());
        }
    }
}
