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
using System;
using System.Collections.Generic;
using System.IO;
using Windows.Web.Http;
using System.Threading;
using System.Threading.Tasks;
using Windows.Web.Http.Headers;
using Salesforce.SDK.Source.Utilities;

namespace Salesforce.SDK.Net
{
    /// <summary>
    /// Enumeration used to represent the content type of a HTTP request
    /// </summary>
    public enum ContentType
    {
        FORM_URLENCODED,
        JSON,
        NONE
    }
    public class HttpCallHeaders
    {
        public HttpCredentialsHeaderValue Authorization { get; private set; }
        public Dictionary<string, string> Headers { get; private set; }

        public HttpCallHeaders(string authorization, Dictionary<string, string> headers)
        {
            if (!String.IsNullOrWhiteSpace(authorization))
            {
                Authorization = new HttpCredentialsHeaderValue("Bearer", authorization);
            }
            Headers = headers;
        }
    }

    /// <summary>
    /// Extension for ContentType enum (to get the mime type of a given content type)
    /// </summary>
    public static class Extensions
    {
        public static string MimeType(this ContentType contentType)
        {
            switch (contentType)
            {
                case ContentType.JSON: return "application/json";
                case ContentType.FORM_URLENCODED: return "application/x-www-form-urlencoded";
                case ContentType.NONE: 
                default:
                    return null;
            }
        }
    }

    /// <summary>
    /// A portable class to send HTTP requests
    /// HttpCall objects can only be used once
    /// </summary>
    public class HttpCall
    {
        private readonly HttpMethod _method;
        private readonly HttpCallHeaders _headers;
        private readonly string _url;
        private readonly string _requestBody;
        private readonly ContentType _contentType;
        private string _responseBody;
        private HttpClient _webClient;
        private Exception _webException;
        private HttpStatusCode _statusCode;

        /// <summary>
        /// True if HTTP request has been executed
        /// </summary>
        public bool Executed
        {
            get
            {
                return (_responseBody != null || _webException != null);
            }
        }

        /// <summary>
        /// True if HTTP request was successfully executed
        /// </summary>
        public bool Success
        {
            get
            {
                CheckExecuted();
                return _webException == null;
            }
        }

        /// <summary>
        /// Error that was raised if HTTP request did not execute successfully
        /// </summary>
        public Exception Error
        {
            get
            {
                CheckExecuted();
                return _webException;
            }
        }

        /// <summary>
        /// True if the HTTP response returned by the server had a body
        /// </summary>
        public bool HasResponse
        {
            get
            {
                return _responseBody != null;
            }
        }

        /// <summary>
        /// Body of the HTTP response returned by the server
        /// </summary>
        public string ResponseBody
        {
            get
            {
                CheckExecuted();
                return _responseBody;
            }
        }

        /// <summary>
        /// HTTP status code fo the response returned by the server
        /// </summary>
        public HttpStatusCode StatusCode
        {
            get
            {
                CheckExecuted();
                return _statusCode;
            }
        }

        private void CheckExecuted()
        {
            if (!Executed)
            {
                throw new System.InvalidOperationException("HttpCall must be executed first");
            }
        }

        /// <summary>
        /// Constructor for HttpCall
        /// </summary>
        /// <param name="method"></param>
        /// <param name="headers"></param>
        /// <param name="url"></param>
        /// <param name="requestBody"></param>
        /// <param name="contentType"></param>
        public HttpCall(HttpMethod method, HttpCallHeaders headers, string url, string requestBody, ContentType contentType)
        {
            _webClient = new HttpClient();
            _method = method;
            _headers = headers;
            _url = url;
            _requestBody = requestBody;
            _contentType = contentType;
        }

        /// <summary>
        /// Factory method to build a HttpCall objet for a GET request with additional HTTP request headers
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static HttpCall CreateGet(HttpCallHeaders headers, string url) 
        {
            return new HttpCall(HttpMethod.Get, headers, url, null, ContentType.NONE);
        }

        /// <summary>
        /// Factory method to build a HttpCall object for a GET request
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static HttpCall CreateGet(string url)
        {
            return CreateGet(null, url);
        }

        /// <summary>
        /// Factory method to build a HttpCall object for a POST request with a specific content type
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="url"></param>
        /// <param name="requestBody"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static HttpCall CreatePost(HttpCallHeaders headers, string url, string requestBody, ContentType contentType)
        {
            return new HttpCall(HttpMethod.Post, headers, url, requestBody, contentType);
        }

        /// <summary>
        /// Factory method to build a HttpCall object for a POST request with form url encoded arguments
        /// </summary>
        /// <param name="url"></param>
        /// <param name="requestBody"></param>
        /// <returns></returns>
        public static HttpCall CreatePost(string url, string requestBody)
        {
            return CreatePost(null, url, requestBody, ContentType.FORM_URLENCODED);
        }

        /// <summary>
        /// Async method to execute the HTTP request which expects the HTTP response body to be a Json object that can be deserizalized as an instance of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<T> ExecuteAndDeserialize<T>()
        {
            HttpCall call = await Execute().ConfigureAwait(false);
            if (call.Success)
            {
                return JsonConvert.DeserializeObject<T>(call.ResponseBody);
            }
            else
            {
                throw call.Error;
            }
        }

        public async Task<HttpCall> Execute() 
        {
            if (Executed)
            {
                throw new System.InvalidOperationException("A HttpCall can only be executed once");
            }
            HttpRequestMessage req = new HttpRequestMessage(_method, new Uri(_url));
            // Setting header
            if (_headers != null)
            {
                var headers = req.Headers;
                foreach (KeyValuePair<string, string> item in _headers.Headers)
                {
                    headers[item.Key] = item.Value;
                }
                if (_headers.Authorization != null)
                {
                    headers.Authorization = _headers.Authorization;
                }
            }

            if (!String.IsNullOrWhiteSpace(_requestBody) &&
                (_method == HttpMethod.Post || _method == HttpMethod.Put || _method == HttpMethod.Patch))
            {
                switch (_contentType)
                {
                    case ContentType.FORM_URLENCODED:
                        req.Content = new HttpFormUrlEncodedContent(_requestBody.ParseQueryString());
                        break;
                    default:
                            req.Content = new HttpStringContent(_requestBody);
                            req.Content.Headers.ContentType = new HttpMediaTypeHeaderValue(_contentType.MimeType());
                        break;
                }
            }
            HttpResponseMessage message = await _webClient.SendRequestAsync(req);
            HandleMessageResponse(message);
            return this;
        }

        private async void HandleMessageResponse(HttpResponseMessage response)
        {
            // End the operation
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _webException = ex;
               // response = (HttpWebResponse) ex.Response;
            }

            if (response != null)
            {
                _responseBody = await response.Content.ReadAsStringAsync();
                _statusCode = response.StatusCode;
                response.Dispose();
            }
        }
    }
}
