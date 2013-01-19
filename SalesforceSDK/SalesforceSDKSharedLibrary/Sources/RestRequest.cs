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

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Newtonsoft.Json;
using Salesforce.WinSDK.Net;

namespace Salesforce.WinSDK.Rest
{
    /**
     * Enumeration for all REST API actions.
     */
    enum RestAction
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
        SEARCH
    }

    /**
     * RestRequest: Class to represent any REST request.
     * 
     * The class offers factory methods to build RestRequest objects for all REST API actions:
     * <ul>
     * <li> versions</li>
     * <li> resources</li>
     * <li> describeGlobal</li>
     * <li> metadata</li>
     * <li> describe</li>
     * <li> create</li>
     * <li> retrieve</li>
     * <li> update</li>
     * <li> upsert</li>
     * <li> delete</li>
     * </ul>
     * 
     * It also has constructors to build any arbitrary request.
     * 
     */
    public class RestRequest
    {

        private readonly Method _method;
        public Method Method
        {
            get { return _method; }
        }

        private readonly String _path;
        public String Path
        {
            get { return _path; }
        }

        private readonly String _requestBody;
        public String Body
        {
            get { return _requestBody; }
        }

        private readonly ContentType _contentType;
        public ContentType ContentType
        {
            get { return _contentType; }
        }

        private readonly Dictionary<String, String> _additionalHeaders;
        public Dictionary<String, String> AdditionalHeaders
        {
            get { return _additionalHeaders; }
        }

        /**
         * Generic constructor for arbitrary requests.
         * 
         * @param method				the HTTP method for the request (GET/POST/DELETE etc)
         * @param path					the URI path, this will automatically be resolved against the users current instance host.
         * @param requestBody			the request body if there is one, can be null.
         * @param contentType           the content type.
         * @param additionalHeaders     additional HTTP headers, can be null.
         */
        public RestRequest(Method method, String path) : this(method, path, null, ContentType.NONE, null) { }
        public RestRequest(Method method, String path, String requestBody) : this(method, path, requestBody, ContentType.FORM_URLENCODED, null) { }
        public RestRequest(Method method, String path, String requestBody, ContentType contentType) : this(method, path, requestBody, contentType, null) { }

        public RestRequest(Method method, String path, String requestBody, ContentType contentType, Dictionary<String, String> additionalHeaders)
        {
            _method = method;
            _path = path;
            _requestBody = requestBody;
            _contentType = contentType;
            _additionalHeaders = additionalHeaders;
        }


        public override String ToString()
        {
            return _method + " " + _path;
        }

        private static String getPath(RestAction action, params String[] args)
        {
            String format = "";
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

            return String.Format(format, args);
        }

        /**
         * Request to get summary information about each Salesforce.com version currently available.
         * See http://www.salesforce.com/us/developer/docs/api_rest/index_Left.htm#StartTopic=Content/resources_versions.htm
         * 
         * @return a RestRequest
         */
        public static RestRequest getRequestForVersions()
        {
            return new RestRequest(Method.GET, getPath(RestAction.VERSIONS));
        }

        /**
         * Request to list available resources for the specified API version, including resource name and URI.
         * See http://www.salesforce.com/us/developer/docs/api_rest/index_Left.htm#StartTopic=Content/resources_discoveryresource.htm
         *
         * @param apiVersion
         * @return a RestRequest
         */
        public static RestRequest getRequestForResources(String apiVersion)
        {
            return new RestRequest(Method.GET, getPath(RestAction.RESOURCES, apiVersion));
        }

        /**
         * Request to list the available objects and their metadata for your organization's data.
         * See http://www.salesforce.com/us/developer/docs/api_rest/index_Left.htm#StartTopic=Content/resources_describeGlobal.htm
         *
         * @param apiVersion
         * @return a RestRequest
         */
        public static RestRequest getRequestForDescribeGlobal(String apiVersion)
        {
            return new RestRequest(Method.GET, getPath(RestAction.DESCRIBE_GLOBAL, apiVersion));
        }

        /**
         * Request to describe the individual metadata for the specified object.
         * See http://www.salesforce.com/us/developer/docs/api_rest/index_Left.htm#StartTopic=Content/resources_sobject_basic_info.htm
         * 
         * @param apiVersion
         * @param objectType
         * @return a RestRequest
         */
        public static RestRequest getRequestForMetadata(String apiVersion, String objectType)
        {
            return new RestRequest(Method.GET, getPath(RestAction.METADATA, apiVersion, objectType));
        }

        /**
         * Request to completely describe the individual metadata at all levels for the specified object. 
         * See http://www.salesforce.com/us/developer/docs/api_rest/index_Left.htm#StartTopic=Content/resources_sobject_describe.htm
         * 
         * @param apiVersion
         * @param objectType
         * @return a RestRequest
         */
        public static RestRequest getRequestForDescribe(String apiVersion, String objectType)
        {
            return new RestRequest(Method.GET, getPath(RestAction.DESCRIBE, apiVersion, objectType));
        }

        /**
         * Request to create a record. 
         * See http://www.salesforce.com/us/developer/docs/api_rest/index_Left.htm#StartTopic=Content/resources_sobject_retrieve.htm
         * 
         * @param apiVersion
         * @param objectType
         * @param fields
         * @return a RestRequest
         */
        public static RestRequest getRequestForCreate(String apiVersion, String objectType, Dictionary<String, Object> fields)
        {
            String fieldsData = (fields == null ? null : JsonConvert.SerializeObject(fields));
            return new RestRequest(Method.POST, getPath(RestAction.CREATE, apiVersion, objectType), fieldsData, ContentType.JSON);
        }

        /**
         * Request to retrieve a record by object id. 
         * See http://www.salesforce.com/us/developer/docs/api_rest/index_Left.htm#StartTopic=Content/resources_sobject_retrieve.htm
         * 
         * @param apiVersion
         * @param objectType
         * @param objectId
         * @param fieldList
         * @return a RestRequest
         */
        public static RestRequest getRequestForRetrieve(String apiVersion, String objectType, String objectId, String[] fieldList)
        {
            StringBuilder path = new StringBuilder(getPath(RestAction.RETRIEVE, apiVersion, objectType, objectId));
            if (fieldList != null && fieldList.Length > 0)
            {
                path.Append("?fields=");
                path.Append(Uri.EscapeUriString(String.Join(",", fieldList)));
            }

            return new RestRequest(Method.GET, path.ToString());
        }

        /**
         * Request to update a record. 
         * See http://www.salesforce.com/us/developer/docs/api_rest/index_Left.htm#StartTopic=Content/resources_sobject_retrieve.htm
         *
         * @param apiVersion 
         * @param objectType
         * @param objectId
         * @param fields
         * @return a RestRequest
         */
        public static RestRequest getRequestForUpdate(String apiVersion, String objectType, String objectId, Dictionary<String, Object> fields)
        {
            String fieldsData = (fields == null ? null : JsonConvert.SerializeObject(fields));
            return new RestRequest(Method.PATCH, getPath(RestAction.UPDATE, apiVersion, objectType, objectId), fieldsData, ContentType.JSON);
        }


        /**
         * Request to upsert (update or insert) a record. 
         * See http://www.salesforce.com/us/developer/docs/api_rest/index_Left.htm#StartTopic=Content/resources_sobject_retrieve.htm
         *
         * @param apiVersion
         * @param objectType
         * @param externalIdField
         * @param externalId
         * @param fields
         * @return a RestRequest
         */
        public static RestRequest getRequestForUpsert(String apiVersion, String objectType, String externalIdField, String externalId, Dictionary<String, Object> fields)
        {
            String fieldsData = (fields == null ? null : JsonConvert.SerializeObject(fields));
            return new RestRequest(Method.PATCH, getPath(RestAction.UPSERT, apiVersion, objectType, externalIdField, externalId), fieldsData, ContentType.JSON);
        }

        /**
         * Request to delete a record. 
         * See http://www.salesforce.com/us/developer/docs/api_rest/index_Left.htm#StartTopic=Content/resources_sobject_retrieve.htm
         * 
         * @param apiVersion
         * @param objectType
         * @param objectId
         * @return a RestRequest
         */
        public static RestRequest getRequestForDelete(String apiVersion, String objectType, String objectId)
        {
            return new RestRequest(Method.DELETE, getPath(RestAction.DELETE, apiVersion, objectType, objectId));
        }

        /**
         * Request to execute the specified SOSL search. 
         * See http://www.salesforce.com/us/developer/docs/api_rest/index_Left.htm#StartTopic=Content/resources_search.htm
         * 
         * @param apiVersion
         * @param q
         * @return a RestRequest
         */
        public static RestRequest getRequestForSearch(String apiVersion, String q)
        {
            StringBuilder path = new StringBuilder(getPath(RestAction.SEARCH, apiVersion));
            path.Append("?q=");
            path.Append(Uri.EscapeUriString(q));
            return new RestRequest(Method.GET, path.ToString());
        }

        /**
         * Request to execute the specified SOQL search. 
         * See http://www.salesforce.com/us/developer/docs/api_rest/index_Left.htm#StartTopic=Content/resources_query.htm
         * 
         * @param apiVersion
         * @param q
         * @return a RestRequest
         */
        public static RestRequest getRequestForQuery(String apiVersion, String q)
        {
            StringBuilder path = new StringBuilder(getPath(RestAction.QUERY, apiVersion));
            path.Append("?q=");
            path.Append(Uri.EscapeUriString(q));
            return new RestRequest(Method.GET, path.ToString());
        }
    }
}
