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
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Salesforce.WinSDK.Net
{
    /**
	 * Enumeration for all HTTP methods.
	 */
    public enum Method
    {
        GET, POST, PUT, DELETE, HEAD, PATCH
    }

    /**
     * Enumeration for content types
     */
    public enum ContentType
    {
        FORM_URLENCODED,
        JSON,
        NONE
    }

    public static class Extensions
    {
        public static String MimeType(this ContentType contentType)
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


    public class HttpCall
    {
        private readonly Method _method;
        private readonly Dictionary<String, String> _headers;
        private readonly String _url;
        private readonly String _requestBody;
        private readonly ContentType _contentType;

        private ManualResetEvent _allDone;
        private HttpWebRequest _request;
        private HttpWebResponse _response;
        private String _responseBody;
        private WebException _webException;

        public Boolean Executed
        {
            get
            {
                return (_response != null || _webException != null);
            }
        }

        public Boolean Success
        {
            get
            {
                CheckExecuted();

                return (_response != null);
            }
        }

        public WebException Error
        {
            get
            {
                return _webException;
            }
        }

        public String ResponseBody
        {
            get
            {
                CheckExecuted();

                if (_response != null && _responseBody == null)
                {
                    using (var reader = new StreamReader(_response.GetResponseStream()))
                    {
                        _responseBody = reader.ReadToEnd();
                    }
                }
                return _responseBody;
            }
        }

        public HttpStatusCode StatusCode
        {
            get
            {
                CheckExecuted();

                if (_response != null)
                {
                    return _response.StatusCode;
                }
                else
                {
                    throw _webException;
                }
            }
        }

        private void CheckExecuted()
        {
            if (!Executed)
            {
                throw new System.InvalidOperationException("HttpCall must be executed first");
            }
        }

        public HttpCall(Method method, Dictionary<String, String> headers, String url, String requestBody, ContentType contentType)
        {
            _method = method;
            _headers = headers;
            _url = url;
            _requestBody = requestBody;
            _contentType = contentType;
        }

        public static HttpCall CreateGet(Dictionary<String, String> headers, String url) 
        {
            return new HttpCall(Method.GET, headers, url, null, ContentType.NONE);
        }

        public static HttpCall CreateGet(String url)
        {
            return CreateGet(null, url);
        }

        public static HttpCall CreatePost(Dictionary<String, String> headers, String url, String requestBody, ContentType contentType)
        {
            return new HttpCall(Method.POST, headers, url, requestBody, contentType);
        }

        public static HttpCall CreatePost(String url, String requestBody)
        {
            return CreatePost(null, url, requestBody, ContentType.FORM_URLENCODED);
        }

        public async Task<HttpCall> Execute()
        {
            return await Task.Factory.StartNew(() => ExecuteSync());
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
                foreach (KeyValuePair<String, String> item in _headers)
                {
                    _request.Headers[item.Key] = item.Value;
                }
            }

            if (_method == Method.GET)
            {
                // Start the asynchronous operation to get the response
                _request.BeginGetResponse(new AsyncCallback(GetResponseCallback), null);
            }
            else if (_method == Method.POST)
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
            // End the operation
            try
            {
                _response = (HttpWebResponse)_request.EndGetResponse(asynchronousResult);
            }
            catch (System.Net.WebException ex)
            {
                _webException = ex;
                _response = (HttpWebResponse) ex.Response;
            }

            // Signalling that we are done
            _allDone.Set();
        }
    }
}
