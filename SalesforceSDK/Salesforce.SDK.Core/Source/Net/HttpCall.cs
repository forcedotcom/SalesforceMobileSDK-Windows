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
using System.Threading.Tasks;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using Windows.Web.Http.Headers;
using Newtonsoft.Json;
using Salesforce.SDK.Utilities;

namespace Salesforce.SDK.Net
{
    /// <summary>
    ///     Enumeration used to represent the content type of a HTTP request
    /// </summary>
    public enum ContentTypeValues
    {
        FormUrlEncoded,
        Json,
        None
    }

    public class HttpCallHeaders
    {
        public HttpCallHeaders(string authorization, Dictionary<string, string> headers)
        {
            if (!String.IsNullOrWhiteSpace(authorization))
            {
                Authorization = new HttpCredentialsHeaderValue("Bearer", authorization);
            }
            Headers = headers;
        }

        public HttpCredentialsHeaderValue Authorization { get; private set; }
        public Dictionary<string, string> Headers { get; private set; }
    }

    /// <summary>
    ///     Extension for ContentType enum (to get the mime type of a given content type)
    /// </summary>
    public static class Extensions
    {
        public static string MimeType(this ContentTypeValues contentType)
        {
            switch (contentType)
            {
                case ContentTypeValues.Json:
                    return "application/json";
                case ContentTypeValues.FormUrlEncoded:
                    return "application/x-www-form-urlencoded";
                case ContentTypeValues.None:
                default:
                    return null;
            }
        }
    }

    /// <summary>
    ///     A portable class to send HTTP requests
    ///     HttpCall objects can only be used once
    /// </summary>
    public class HttpCall
    {
        private readonly ContentTypeValues ContentType;
        private readonly HttpCallHeaders Headers;
        private readonly HttpMethod Method;
        private readonly string RequestBody;
        private readonly string Url;
        private readonly HttpClient WebClient;
        private Exception HttpCallErrorException;
        private string ResponseBodyText;
        private HttpStatusCode StatusCodeValue;

        /// <summary>
        ///     Constructor for HttpCall
        /// </summary>
        /// <param name="method"></param>
        /// <param name="headers"></param>
        /// <param name="url"></param>
        /// <param name="requestBody"></param>
        /// <param name="contentType"></param>
        public HttpCall(HttpMethod method, HttpCallHeaders headers, string url, string requestBody,
            ContentTypeValues contentType)
        {
            var httpBaseFilter = new HttpBaseProtocolFilter
            {
                AllowUI = false
            };
            WebClient = new HttpClient(httpBaseFilter);
            Method = method;
            Headers = headers;
            Url = url;
            RequestBody = requestBody;
            ContentType = contentType;
        }

        /// <summary>
        ///     True if HTTP request has been executed
        /// </summary>
        public bool Executed
        {
            get { return (ResponseBodyText != null || HttpCallErrorException != null); }
        }

        /// <summary>
        ///     True if HTTP request was successfully executed
        /// </summary>
        public bool Success
        {
            get
            {
                CheckExecuted();
                return HttpCallErrorException == null;
            }
        }

        /// <summary>
        ///     Error that was raised if HTTP request did not execute successfully
        /// </summary>
        public Exception Error
        {
            get
            {
                CheckExecuted();
                return HttpCallErrorException;
            }
        }

        /// <summary>
        ///     True if the HTTP response returned by the server had a body
        /// </summary>
        public bool HasResponse
        {
            get { return ResponseBodyText != null; }
        }

        /// <summary>
        ///     Body of the HTTP response returned by the server
        /// </summary>
        public string ResponseBody
        {
            get
            {
                CheckExecuted();
                return ResponseBodyText;
            }
        }

        /// <summary>
        ///     HTTP status code fo the response returned by the server
        /// </summary>
        public HttpStatusCode StatusCode
        {
            get
            {
                CheckExecuted();
                return StatusCodeValue;
            }
        }

        private void CheckExecuted()
        {
            if (!Executed)
            {
                throw new InvalidOperationException("HttpCall must be executed first");
            }
        }

        /// <summary>
        ///     Factory method to build a HttpCall objet for a GET request with additional HTTP request headers
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static HttpCall CreateGet(HttpCallHeaders headers, string url)
        {
            return new HttpCall(HttpMethod.Get, headers, url, null, ContentTypeValues.None);
        }

        /// <summary>
        ///     Factory method to build a HttpCall object for a GET request
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static HttpCall CreateGet(string url)
        {
            return CreateGet(null, url);
        }

        /// <summary>
        ///     Factory method to build a HttpCall object for a POST request with a specific content type
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="url"></param>
        /// <param name="requestBody"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static HttpCall CreatePost(HttpCallHeaders headers, string url, string requestBody,
            ContentTypeValues contentType)
        {
            return new HttpCall(HttpMethod.Post, headers, url, requestBody, contentType);
        }

        /// <summary>
        ///     Factory method to build a HttpCall object for a POST request with form url encoded arguments
        /// </summary>
        /// <param name="url"></param>
        /// <param name="requestBody"></param>
        /// <returns></returns>
        public static HttpCall CreatePost(string url, string requestBody)
        {
            return CreatePost(null, url, requestBody, ContentTypeValues.FormUrlEncoded);
        }

        /// <summary>
        ///     Async method to execute the HTTP request which expects the HTTP response body to be a Json object that can be
        ///     deserizalized as an instance of type T
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
            throw call.Error;
        }

        /// <summary>
        ///     Executes the HttpCall. This will generate the headers, create the request and populate the HttpCall properties with
        ///     relevant data.
        ///     The HttpCall may only be called once; further attempts to execute the same call will throw an
        ///     InvalidOperationException.
        /// </summary>
        /// <returns>HttpCall with populated data</returns>
        public async Task<HttpCall> Execute()
        {
            if (Executed)
            {
                throw new InvalidOperationException("A HttpCall can only be executed once");
            }
            var req = new HttpRequestMessage(Method, new Uri(Url));
            // Setting header
            if (Headers != null)
            {
                HttpRequestHeaderCollection headers = req.Headers;
                if (Headers.Authorization != null)
                {
                    headers.Authorization = Headers.Authorization;
                }
                foreach (var item in Headers.Headers)
                {
                    headers[item.Key] = item.Value;
                }
            }

            if (!String.IsNullOrWhiteSpace(RequestBody))
            {
                switch (ContentType)
                {
                    case ContentTypeValues.FormUrlEncoded:
                        req.Content = new HttpFormUrlEncodedContent(RequestBody.ParseQueryString());
                        break;
                    default:
                        req.Content = new HttpStringContent(RequestBody);
                        req.Content.Headers.ContentType = new HttpMediaTypeHeaderValue(ContentType.MimeType());
                        break;
                }
            }
            HttpResponseMessage message;
            try
            {
               message = await WebClient.SendRequestAsync(req);
               HandleMessageResponse(message);
            }
            catch (Exception ex)
            {
                HttpCallErrorException = ex;
                message = null;
            } 
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
                HttpCallErrorException = ex;
            }

            if (response != null)
            {
                ResponseBodyText = await response.Content.ReadAsStringAsync();
                StatusCodeValue = response.StatusCode;
                response.Dispose();
            }
        }
    }
}