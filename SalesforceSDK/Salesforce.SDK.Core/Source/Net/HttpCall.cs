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
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Salesforce.SDK.Net
{
    /// <summary>
    /// Enumeration used to represent the method of a HTTP request
    /// </summary>
    public enum RestMethod
    {
        GET, POST, PUT, DELETE, HEAD, PATCH
    }

    /// <summary>
    /// Enumeration used to represent the content type of a HTTP request
    /// </summary>
    public enum ContentType
    {
        FORM_URLENCODED,
        JSON,
        NONE
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
        private readonly RestMethod _method;
        private readonly Dictionary<string, string> _headers;
        private readonly string _url;
        private readonly string _requestBody;
        private readonly ContentType _contentType;

        private ManualResetEvent _allDone;
        private HttpWebRequest _request;
        private string _responseBody;
        private HttpStatusCode _statusCode;
        private WebException _webException;

        /// <summary>
        /// True if HTTP request has been executed
        /// </summary>
        public Boolean Executed
        {
            get
            {
                return (_responseBody != null || _webException != null);
            }
        }

        /// <summary>
        /// True if HTTP request was successfully executed
        /// </summary>
        public Boolean Success
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
        public WebException Error
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
        public Boolean HasResponse
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
        public HttpCall(RestMethod method, Dictionary<string, string> headers, string url, string requestBody, ContentType contentType)
        {
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
        public static HttpCall CreateGet(Dictionary<string, string> headers, string url) 
        {
            return new HttpCall(RestMethod.GET, headers, url, null, ContentType.NONE);
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
        public static HttpCall CreatePost(Dictionary<string, string> headers, string url, string requestBody, ContentType contentType)
        {
            return new HttpCall(RestMethod.POST, headers, url, requestBody, contentType);
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
        /// Async method to execute the HTTP request
        /// </summary>
        /// <returns></returns>
        public async Task<HttpCall> Execute()
        {
            return await Task.Factory.StartNew(() => ExecuteSync());
        }

        /// <summary>
        /// Async method to execute the HTTP request which expects the HTTP response body to be a Json object that can be deserizalized as an instance of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<T> ExecuteAndDeserialize<T>()
        {
            return await Execute().ContinueWith(t =>
                {
                    HttpCall call = t.Result;
                    if (call.Success)
                    {
                        return JsonConvert.DeserializeObject<T>(call.ResponseBody);
                    }
                    else
                    {
                        throw call.Error;
                    }
                });
        }

        private HttpCall ExecuteSync() 
        {
            if (Executed)
            {
                throw new System.InvalidOperationException("A HttpCall can only be executed once");
            }

            _allDone = new ManualResetEvent(false);
            _request = (HttpWebRequest)HttpWebRequest.Create(_url);

            // Setting method
            _request.Method = _method.ToString();

            // Setting header
            if (_headers != null)
            {
                foreach (KeyValuePair<string, string> item in _headers)
                {
                    _request.Headers[item.Key] = item.Value;
                }
            }

            if (_method == RestMethod.GET || _method == RestMethod.HEAD || _method == RestMethod.DELETE)
            {
                // Start the asynchronous operation to get the response
                _request.BeginGetResponse(new AsyncCallback(GetResponseCallback), null);
            }
            else if (_method == RestMethod.POST || _method == RestMethod.PUT || _method == RestMethod.PATCH)
            {
                // Setting content type
                _request.ContentType = _contentType.MimeType();

                // Start the asynchronous operation to send the request body
                _request.BeginGetRequestStream(new AsyncCallback(GetRequestStreamCallback), null);
            }

            // Keep the thread from continuing while the asynchronous 
            _allDone.WaitOne();

            return this;
        }

        private void GetRequestStreamCallback(IAsyncResult asynchronousResult)
        {
            // End the operation
            Stream postStream = _request.EndGetRequestStream(asynchronousResult);

            // Sending post body
            using(var writer = new StreamWriter(postStream))
            {
                writer.Write(_requestBody);
            }

            // Start the asynchronous operation to get the response
            _request.BeginGetResponse(new AsyncCallback(GetResponseCallback), null);
        }

        private void GetResponseCallback(IAsyncResult asynchronousResult)
        {
            HttpWebResponse response;

            // End the operation
            try
            {
                response = (HttpWebResponse)_request.EndGetResponse(asynchronousResult);
            }
            catch (WebException ex)
            {
                _webException = ex;
                response = (HttpWebResponse) ex.Response;
            }

            if (response != null)
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    _responseBody = reader.ReadToEnd();
                }
                _statusCode = response.StatusCode;

                response.Dispose();
            }


            // Signalling that we are done
            _allDone.Set();
        }
    }
}
