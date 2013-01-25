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
using Salesforce.SDK.Auth;
using Salesforce.SDK.Net;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Salesforce.SDK.Rest
{
    class IdName 
    {
        private readonly String _id;
        public String Id
        {
            get { return _id; }
        }

        private readonly String _name;
        public String Name
        {
            get { return _name; }
        }

        public IdName(String id, String name) {
            _id = id;
            _name = name;
        }
    }


    [TestClass]
    public class RestClientTest
    {
        private const String ENTITY_NAME_PREFIX = "RestClientTest";
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
            Cleanup();
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

        [TestMethod]
        public void TestCreate()
        {
            Dictionary<String, Object> fields = new Dictionary<String, Object>() {{"name", generateAccountName()}};
            RestResponse response = _restClient.SendSync(RestRequest.GetRequestForCreate(TestCredentials.API_VERSION, "account", fields));
            JObject jsonResponse = response.AsJObject;
            CheckKeys(jsonResponse, "id", "errors", "success");
            Assert.IsTrue((Boolean) jsonResponse["success"], "Create failed");
        }
    
        [TestMethod]
        public void TestRetrieve() 
        {
            String[] fields = new String[] { "name", "ownerId" };
            IdName newAccountIdName = CreateAccount();
            RestResponse response = _restClient.SendSync(RestRequest.GetRequestForRetrieve(TestCredentials.API_VERSION, "account", newAccountIdName.Id, fields));
            CheckResponse(response, HttpStatusCode.OK, false);
            JObject jsonResponse = response.AsJObject;
            CheckKeys(jsonResponse, "attributes", "Name", "OwnerId", "Id");
            Assert.AreEqual(newAccountIdName.Name, jsonResponse["Name"], "Wrong row returned");
        }
    
        [TestMethod]
        public void TestUpdate()
        {
            // Create
            IdName newAccountIdName = CreateAccount();
    
            // Update
            String updatedAccountName = generateAccountName();
            Dictionary<String, Object> fields = new Dictionary<String, Object>() {{"name", updatedAccountName}};

            RestResponse updateResponse = _restClient.SendSync(RestRequest.GetRequestForUpdate(TestCredentials.API_VERSION, "account", newAccountIdName.Id, fields));
            Assert.IsTrue(updateResponse.Success, "Update failed");
    
            // Retrieve - expect updated name
            RestResponse response = _restClient.SendSync(RestRequest.GetRequestForRetrieve(TestCredentials.API_VERSION, "account", newAccountIdName.Id, new String[] {"name"}));
            Assert.AreEqual(updatedAccountName, response.AsJObject["Name"], "Wrong row returned");
        }
    
    
        [TestMethod]
        public void TestDelete() 
        {
            // Create
            IdName newAccountIdName = CreateAccount();
    
            // Delete
            RestResponse deleteResponse = _restClient.SendSync(RestRequest.GetRequestForDelete(TestCredentials.API_VERSION, "account", newAccountIdName.Id));
            Assert.IsTrue(deleteResponse.Success, "Delete failed");
    
            // Retrieve - expect 404
            RestResponse response = _restClient.SendSync(RestRequest.GetRequestForRetrieve(TestCredentials.API_VERSION, "account", newAccountIdName.Id, new String[] {"name"}));
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "404 was expected");
        }
    
    
        [TestMethod]
        public void TestQuery() 
        {
            IdName newAccountIdName = CreateAccount();
            RestResponse response = _restClient.SendSync(RestRequest.GetRequestForQuery(TestCredentials.API_VERSION, "select name from account where id = '" + newAccountIdName.Id + "'"));
            CheckResponse(response, HttpStatusCode.OK, false);
            JObject jsonResponse = response.AsJObject;
            CheckKeys(jsonResponse, "done", "totalSize", "records");
            Assert.AreEqual(1, jsonResponse["totalSize"], "Expected one row");
            Assert.AreEqual(newAccountIdName.Name, jsonResponse["records"][0]["Name"], "Wrong row returned");
        }
    
        [TestMethod]
        public void TestSearch() 
        {
            IdName newAccountIdName = CreateAccount();
            RestResponse response = _restClient.SendSync(RestRequest.GetRequestForSearch(TestCredentials.API_VERSION, "find {" + ENTITY_NAME_PREFIX + "}"));
            CheckResponse(response, HttpStatusCode.OK, true);
            JArray matchingRows = response.AsJArray;
            Assert.AreEqual(1, matchingRows.Count, "Expected one row");
            JObject matchingRow = (JObject) matchingRows[0];
            CheckKeys(matchingRow, "attributes", "Id");
            Assert.AreEqual(newAccountIdName.Id, matchingRow["Id"], "Wrong row returned");
        }

        private String generateAccountName()
        {
            return ENTITY_NAME_PREFIX + "-" + DateTime.Now.ToFileTime() + (new Random()).Next(10000, 99999);
        }

        private IdName CreateAccount() 
        {
            String newAccountName = generateAccountName();
            Dictionary<String, Object> fields = new Dictionary<String, Object>() {{"name", newAccountName}};            
            RestResponse response = _restClient.SendSync(RestRequest.GetRequestForCreate(TestCredentials.API_VERSION, "account", fields));
            String newAccountId = (String) response.AsJObject["id"];
            return new IdName(newAccountId, newAccountName);
        }
    
        private void Cleanup() {
            try {
                RestResponse searchResponse = _restClient.SendSync(RestRequest.GetRequestForSearch(TestCredentials.API_VERSION, "find {" + ENTITY_NAME_PREFIX + "}"));
                JArray matchingRows = searchResponse.AsJArray;
                for (int i=0; i<matchingRows.Count; i++) {
                    JObject matchingRow = (JObject) matchingRows[i];
                    String matchingRowType = (String) matchingRow["attributes"]["type"];
                    String matchingRowId = (String) matchingRow["Id"];
                    Debug.WriteLine("Trying to delete {0}", matchingRowId);
                    _restClient.SendSync(RestRequest.GetRequestForDelete(TestCredentials.API_VERSION, matchingRowType, matchingRowId));
                    Debug.WriteLine("Successfully deleted {0}", matchingRowId);
                }
            }
            catch {
                // We tried our best :-(
            }
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
