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
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Newtonsoft.Json.Linq;
using Salesforce.WinSDK.Auth;
using Salesforce.WinSDK.Net;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Salesforce.WinSDK.Rest
{
    [TestClass]
    public class RestClientTest
    {
        private const String BAD_TOKEN = "bad-token";

        private RestClient _restClient;
        private String _accessToken;

        [TestInitialize]
        public void SetUp()
        {
            _accessToken = OAuth2.RefreshAuthToken(TestCredentials.LOGIN_SERVER, TestCredentials.CLIENT_ID, TestCredentials.REFRESH_TOKEN).Result.AccessToken;
            _restClient = new RestClient(TestCredentials.INSTANCE_SERVER, _accessToken, null);
        }

        [TestCleanup]
        public void TearDown()
        {
        }

        [TestMethod]
        public void TestGetAuthToken()
        {
            Assert.AreEqual(_accessToken, _restClient.AccessToken, "Wrong access token");
        }


        [TestMethod]
        public void TestCallWithBadAuthToken()
        {
            RestClient unauthenticatedRestClient = new RestClient(TestCredentials.INSTANCE_SERVER, BAD_TOKEN, null);
            RestResponse response = unauthenticatedRestClient.SendSync(RestRequest.GetRequestForResources(TestCredentials.API_VERSION));
            Assert.IsFalse(response.Success, "Success not expected");
            Assert.IsNotNull(response.Error, "Expected error");
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Expected 401");
        }

        [TestMethod]
        public void TestCallWithBadAuthTokenAndTokenProvider()
        {
            RestClient unauthenticatedRestClient = new RestClient(TestCredentials.INSTANCE_SERVER, BAD_TOKEN, () => Task.Factory.StartNew(() => _accessToken));
            Assert.AreEqual(BAD_TOKEN, unauthenticatedRestClient.AccessToken, "RestClient should be using the bad token initially");
            RestResponse response = unauthenticatedRestClient.SendSync(RestRequest.GetRequestForResources(TestCredentials.API_VERSION));
            Assert.IsTrue(response.Success, "Success expected");
            Assert.IsNull(response.Error, "Expected error");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Expected 200");
            Assert.AreEqual(_accessToken, unauthenticatedRestClient.AccessToken, "RestClient should now using the good token");
        }

        [TestMethod]
        public void TestGetVersions()
        {
            // We don't need to be authenticated
            RestClient unauthenticatedRestClient = new RestClient(TestCredentials.INSTANCE_SERVER, BAD_TOKEN, null);
            RestResponse response = unauthenticatedRestClient.SendSync(RestRequest.GetRequestForVersions());
            CheckResponse(response, HttpStatusCode.OK, true);
        }

        [TestMethod]
        public void TestGetResources() 
        {
            RestResponse response = _restClient.SendSync(RestRequest.GetRequestForResources(TestCredentials.API_VERSION));
            CheckResponse(response, HttpStatusCode.OK, false);
            CheckKeys(response.AsJObject, "sobjects", "search", "recent");
        }

        [TestMethod]
        public void TestDescribeGlobal()
        {
            RestResponse response = _restClient.SendSync(RestRequest.GetRequestForDescribeGlobal(TestCredentials.API_VERSION));
            CheckResponse(response, HttpStatusCode.OK, false);
            JObject jsonResponse = response.AsJObject;
            CheckKeys(jsonResponse, "encoding", "maxBatchSize", "sobjects");
            CheckKeys((JObject) ((JArray) jsonResponse["sobjects"])[0], "name", "label", "custom", "keyPrefix");
        }

        [TestMethod]
        public void TestMetadata()
        {
            RestResponse response = _restClient.SendSync(RestRequest.GetRequestForMetadata(TestCredentials.API_VERSION, "account"));
            CheckResponse(response, HttpStatusCode.OK, false);
            JObject jsonResponse = response.AsJObject;
            CheckKeys(jsonResponse, "objectDescribe", "recentItems");
            CheckKeys((JObject) jsonResponse["objectDescribe"], "name", "label", "keyPrefix");
            Assert.AreEqual("Account", jsonResponse["objectDescribe"]["name"], "Wrong object name");
        }

        [TestMethod]
        public void TestDescribe()
        {
            RestResponse response = _restClient.SendSync(RestRequest.GetRequestForDescribe(TestCredentials.API_VERSION, "account"));
            CheckResponse(response, HttpStatusCode.OK, false);
            JObject jsonResponse = response.AsJObject;
            CheckKeys(jsonResponse, "name", "fields", "urls", "label");
            Assert.AreEqual("Account", jsonResponse["name"], "Wrong object name");
        }

        private void CheckResponse(RestResponse response, HttpStatusCode expectedStatusCode, Boolean isJArray)
        {
            Assert.AreEqual(expectedStatusCode, response.StatusCode);
            try
            {
                if (isJArray)
                {
                    Assert.IsNotNull(response.AsJArray);
                }
                else
                {
                    Assert.IsNotNull(response.AsJObject);
                }
            }
            catch
            {
                Assert.Fail("Failed to parse response body");
            }

        }

        private void CheckKeys(JObject jObject, params String[] expectedKeys)
        {
            foreach (String expectedKey in expectedKeys)
            {
                Assert.IsNotNull(jObject[expectedKey]);
            }
        }
    }
}
