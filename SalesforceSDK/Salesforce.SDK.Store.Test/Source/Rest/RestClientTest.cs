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
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Newtonsoft.Json.Linq;
using Salesforce.SDK.Auth;

namespace Salesforce.SDK.Rest
{
    internal class IdName
    {
        private readonly string _id;

        private readonly string _name;

        public IdName(string id, string name)
        {
            _id = id;
            _name = name;
        }

        public string Id
        {
            get { return _id; }
        }

        public string Name
        {
            get { return _name; }
        }
    }


    [TestClass]
    public class RestClientTest
    {
        private const string ENTITY_NAME_PREFIX = "RestClientTest";
        private const string BAD_TOKEN = "bad-token";

        private string _accessToken;
        private RestClient _restClient;

        [TestInitialize]
        public void SetUp()
        {
            var loginOptions = new LoginOptions(TestCredentials.LOGIN_URL, TestCredentials.CLIENT_ID, null, null);
            _accessToken =
                OAuth2.RefreshAuthTokenRequest(loginOptions, TestCredentials.REFRESH_TOKEN).Result.AccessToken;
            _restClient = new RestClient(TestCredentials.INSTANCE_SERVER, _accessToken, null);
        }

        [TestCleanup]
        public void TearDown()
        {
            Cleanup();
        }

        [TestMethod]
        public void TestGetAuthToken()
        {
            Assert.AreEqual(_accessToken, _restClient.AccessToken, "Wrong access token");
        }


        [TestMethod]
        public async void TestCallWithBadAuthToken()
        {
            var unauthenticatedRestClient = new RestClient(TestCredentials.INSTANCE_SERVER, BAD_TOKEN, null);
            RestResponse response =
                await
                    unauthenticatedRestClient.SendAsync(RestRequest.GetRequestForResources(TestCredentials.API_VERSION));
            Assert.IsFalse(response.Success, "Success not expected");
            Assert.IsNotNull(response.Error, "Expected error");
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Expected 401");
        }

        [TestMethod]
        public async void TestCallWithBadAuthTokenAndTokenProvider()
        {
            var unauthenticatedRestClient = new RestClient(TestCredentials.INSTANCE_SERVER, BAD_TOKEN,
                () => Task.Factory.StartNew(() => _accessToken));
            Assert.AreEqual(BAD_TOKEN, unauthenticatedRestClient.AccessToken,
                "RestClient should be using the bad token initially");
            RestResponse response =
                await
                    unauthenticatedRestClient.SendAsync(RestRequest.GetRequestForResources(TestCredentials.API_VERSION));
            Assert.IsTrue(response.Success, "Success expected");
            Assert.IsNull(response.Error, "Expected error");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Expected 200");
            Assert.AreEqual(_accessToken, unauthenticatedRestClient.AccessToken,
                "RestClient should now using the good token");
        }

        [TestMethod]
        public async void TestGetVersions()
        {
            // We don't need to be authenticated
            var unauthenticatedRestClient = new RestClient(TestCredentials.INSTANCE_SERVER, BAD_TOKEN, null);
            RestResponse response = await unauthenticatedRestClient.SendAsync(RestRequest.GetRequestForVersions());
            CheckResponse(response, HttpStatusCode.OK, true);
        }

        [TestMethod]
        public async void TestGetResources()
        {
            RestResponse response =
                await _restClient.SendAsync(RestRequest.GetRequestForResources(TestCredentials.API_VERSION));
            CheckResponse(response, HttpStatusCode.OK, false);
            CheckKeys(response.AsJObject, "sobjects", "search", "recent");
        }

        [TestMethod]
        public async void TestDescribeGlobal()
        {
            RestResponse response =
                await _restClient.SendAsync(RestRequest.GetRequestForDescribeGlobal(TestCredentials.API_VERSION));
            CheckResponse(response, HttpStatusCode.OK, false);
            JObject jsonResponse = response.AsJObject;
            CheckKeys(jsonResponse, "encoding", "maxBatchSize", "sobjects");
            CheckKeys((JObject) ((JArray) jsonResponse["sobjects"])[0], "name", "label", "custom", "keyPrefix");
        }

        [TestMethod]
        public async void TestMetadata()
        {
            RestResponse response =
                await _restClient.SendAsync(RestRequest.GetRequestForMetadata(TestCredentials.API_VERSION, "account"));
            CheckResponse(response, HttpStatusCode.OK, false);
            JObject jsonResponse = response.AsJObject;
            CheckKeys(jsonResponse, "objectDescribe", "recentItems");
            CheckKeys((JObject) jsonResponse["objectDescribe"], "name", "label", "keyPrefix");
            Assert.AreEqual("Account", jsonResponse["objectDescribe"]["name"], "Wrong object name");
        }

        [TestMethod]
        public async void TestDescribe()
        {
            RestResponse response =
                await _restClient.SendAsync(RestRequest.GetRequestForDescribe(TestCredentials.API_VERSION, "account"));
            CheckResponse(response, HttpStatusCode.OK, false);
            JObject jsonResponse = response.AsJObject;
            CheckKeys(jsonResponse, "name", "fields", "urls", "label");
            Assert.AreEqual("Account", jsonResponse["name"], "Wrong object name");
        }

        [TestMethod]
        public async void TestCreate()
        {
            var fields = new Dictionary<string, object> {{"name", generateAccountName()}};
            RestResponse response =
                await
                    _restClient.SendAsync(RestRequest.GetRequestForCreate(TestCredentials.API_VERSION, "account", fields));
            JObject jsonResponse = response.AsJObject;
            CheckKeys(jsonResponse, "id", "errors", "success");
            Assert.IsTrue((bool) jsonResponse["success"], "Create failed");
        }

        [TestMethod]
        public async void TestRetrieve()
        {
            string[] fields = {"name", "ownerId"};
            IdName newAccountIdName = await CreateAccount();
            RestResponse response =
                await
                    _restClient.SendAsync(RestRequest.GetRequestForRetrieve(TestCredentials.API_VERSION, "account",
                        newAccountIdName.Id, fields));
            CheckResponse(response, HttpStatusCode.OK, false);
            JObject jsonResponse = response.AsJObject;
            CheckKeys(jsonResponse, "attributes", "Name", "OwnerId", "Id");
            Assert.AreEqual(newAccountIdName.Name, jsonResponse["Name"], "Wrong row returned");
        }

        [TestMethod]
        public async void TestUpdate()
        {
            // Create
            IdName newAccountIdName = await CreateAccount();

            // Update
            string updatedAccountName = generateAccountName();
            var fields = new Dictionary<string, object> {{"name", updatedAccountName}};

            RestResponse updateResponse =
                await
                    _restClient.SendAsync(RestRequest.GetRequestForUpdate(TestCredentials.API_VERSION, "account",
                        newAccountIdName.Id, fields));
            Assert.IsTrue(updateResponse.Success, "Update failed");

            // Retrieve - expect updated name
            RestResponse response =
                await
                    _restClient.SendAsync(RestRequest.GetRequestForRetrieve(TestCredentials.API_VERSION, "account",
                        newAccountIdName.Id, new[] {"name"}));
            Assert.AreEqual(updatedAccountName, response.AsJObject["Name"], "Wrong row returned");
        }


        [TestMethod]
        public async void TestDelete()
        {
            // Create
            IdName newAccountIdName = await CreateAccount();

            // Delete
            RestResponse deleteResponse =
                await
                    _restClient.SendAsync(RestRequest.GetRequestForDelete(TestCredentials.API_VERSION, "account",
                        newAccountIdName.Id));
            Assert.IsTrue(deleteResponse.Success, "Delete failed");

            // Retrieve - expect 404
            RestResponse response =
                await
                    _restClient.SendAsync(RestRequest.GetRequestForRetrieve(TestCredentials.API_VERSION, "account",
                        newAccountIdName.Id, new[] {"name"}));
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "404 was expected");
        }


        [TestMethod]
        public async void TestQuery()
        {
            IdName newAccountIdName = await CreateAccount();
            RestResponse response =
                await
                    _restClient.SendAsync(RestRequest.GetRequestForQuery(TestCredentials.API_VERSION,
                        "select name from account where id = '" + newAccountIdName.Id + "'"));
            CheckResponse(response, HttpStatusCode.OK, false);
            JObject jsonResponse = response.AsJObject;
            CheckKeys(jsonResponse, "done", "totalSize", "records");
            Assert.AreEqual(1, jsonResponse["totalSize"], "Expected one row");
            Assert.AreEqual(newAccountIdName.Name, jsonResponse["records"][0]["Name"], "Wrong row returned");
        }

        [TestMethod]
        public async void TestSearch()
        {
            IdName newAccountIdName = await CreateAccount();
            RestResponse response =
                await
                    _restClient.SendAsync(RestRequest.GetRequestForSearch(TestCredentials.API_VERSION,
                        "find {" + newAccountIdName.Name + "}"));
            CheckResponse(response, HttpStatusCode.OK, true);
            JArray matchingRows = response.AsJArray;
            Assert.AreEqual(1, matchingRows.Count, "Expected one row");
            var matchingRow = (JObject) matchingRows[0];
            CheckKeys(matchingRow, "attributes", "Id");
            Assert.AreEqual(newAccountIdName.Id, matchingRow["Id"], "Wrong row returned");
        }

        private string generateAccountName()
        {
            return ENTITY_NAME_PREFIX + (new Random()).Next(10000, 99999);
        }

        private async Task<IdName> CreateAccount()
        {
            string newAccountName = generateAccountName();
            var fields = new Dictionary<string, object> {{"name", newAccountName}};
            RestResponse response =
                await
                    _restClient.SendAsync(RestRequest.GetRequestForCreate(TestCredentials.API_VERSION, "account", fields));
            var newAccountId = (string) response.AsJObject["id"];
            return new IdName(newAccountId, newAccountName);
        }

        private async void Cleanup()
        {
            try
            {
                RestResponse searchResponse =
                    await
                        _restClient.SendAsync(RestRequest.GetRequestForSearch(TestCredentials.API_VERSION,
                            "find {" + ENTITY_NAME_PREFIX + "}"));
                JArray matchingRows = searchResponse.AsJArray;
                for (int i = 0; i < matchingRows.Count; i++)
                {
                    var matchingRow = (JObject) matchingRows[i];
                    var matchingRowType = (string) matchingRow["attributes"]["type"];
                    var matchingRowId = (string) matchingRow["Id"];
                    Debug.WriteLine("Trying to delete {0}", matchingRowId);
                    await
                        _restClient.SendAsync(RestRequest.GetRequestForDelete(TestCredentials.API_VERSION,
                            matchingRowType, matchingRowId));
                    Debug.WriteLine("Successfully deleted {0}", matchingRowId);
                }
            }
            catch
            {
                // We tried our best :-(
            }
        }

        private void CheckResponse(RestResponse response, HttpStatusCode expectedStatusCode, bool isJArray)
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

        private void CheckKeys(JObject jObject, params string[] expectedKeys)
        {
            foreach (string expectedKey in expectedKeys)
            {
                Assert.IsNotNull(jObject[expectedKey]);
            }
        }
    }
}