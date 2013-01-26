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
using Newtonsoft.Json;
using Salesforce.SDK.Net;
using System;
using System.Collections.Generic;
using System.Net;

namespace Salesforce.SDK.Auth
{
    [TestClass]
    public class OAuth2Test
    {
        [TestMethod]
        public void TestGetAuthorizationUrl()
        {
            String loginServer = "https://login.salesforce.com";
            String clientId = "TEST_CLIENT_ID";
            String callbackUrl = "test://sfdc";
            String[] scopes = { "web", "api" };

            String expectedUrl = "https://login.salesforce.com/services/oauth2/authorize?display=mobile&response_type=token&client_id=TEST_CLIENT_ID&redirect_uri=test://sfdc&scope=web%20api%20refresh_token";
            String actualUrl = OAuth2.ComputeAuthorizationUrl(loginServer, clientId, callbackUrl, scopes);
            Assert.AreEqual(expectedUrl, actualUrl, "Wrong authorization url");
        }

        [TestMethod]
        public void TestRefreshAuthToken()
        {
            // Try describe without being authenticated, expect 401
            Assert.AreEqual(HttpStatusCode.Unauthorized, DoDescribe(null));

            // Get auth token (through refresh)
            AuthResponse refreshResponse = OAuth2.RefreshAuthToken(TestCredentials.LOGIN_SERVER, TestCredentials.CLIENT_ID, TestCredentials.REFRESH_TOKEN).Result;

            // Try describe again, expect 200
            Assert.AreEqual(HttpStatusCode.OK, DoDescribe(refreshResponse.AccessToken));
        }

        [TestMethod]
        public void testCallIdentityService()
        {
            // Get auth token and identity url (through refresh)
            AuthResponse refreshResponse = OAuth2.RefreshAuthToken(TestCredentials.LOGIN_SERVER, TestCredentials.CLIENT_ID, TestCredentials.REFRESH_TOKEN).Result;

            // Call the identity service
            IdentityResponse identityResponse = OAuth2.CallIdentityService(refreshResponse.IdentityUrl, refreshResponse.AccessToken).Result;

            // Check username
            Assert.AreEqual("sdktest@cs0.com", identityResponse.UserName);
        }

        [TestMethod]
        public void testDeserializeIdentityResponseWithoutMobilePolicy()
        {
            String idUrl = "https://login.salesforce.com/id/00D50000000IZ3ZEAW/00550000001fg5OAAQ";
            String userId = "00550000001fg5OAAQ";
            String organizationId = "00D50000000IZ3ZEAW";
            String userName = "user@example.com";
            String partialResponseWithoutMobilePolicy = "{\"id\":\"" + idUrl + "\",\"user_id\":\"" + userId + "\",\"organization_id\":\"" + organizationId + "\",\"username\":\"" + userName + "\"}";
            IdentityResponse idResponse = JsonConvert.DeserializeObject<IdentityResponse>(partialResponseWithoutMobilePolicy);

            Assert.AreEqual(idUrl, idResponse.IdentityUrl);
            Assert.AreEqual(userId, idResponse.UserId);
            Assert.AreEqual(organizationId, idResponse.OrganizationId);
            Assert.AreEqual(userName, idResponse.UserName);
            Assert.IsNull(idResponse.MobilePolicy);
        }

        [TestMethod]
        public void testDeserializeIdentityResponseWithMobilePolicy()
        {
            String idUrl = "https://login.salesforce.com/id/00D50000000IZ3ZEAW/00550000001fg5OAAQ";
            String userId = "00550000001fg5OAAQ";
            String organizationId = "00D50000000IZ3ZEAW";
            String userName = "user@example.com";
            String partialResponseWithMobilePolicy = "{\"id\":\"" + idUrl + "\",\"user_id\":\"" + userId + "\",\"organization_id\":\"" + organizationId + "\",\"username\":\"" + userName + "\",\"mobile_policy\":{\"pin_length\":6,\"screen_lock\":1}}";
            IdentityResponse idResponse = JsonConvert.DeserializeObject<IdentityResponse>(partialResponseWithMobilePolicy);

            Assert.AreEqual(idUrl, idResponse.IdentityUrl);
            Assert.AreEqual(userId, idResponse.UserId);
            Assert.AreEqual(organizationId, idResponse.OrganizationId);
            Assert.AreEqual(userName, idResponse.UserName);
            Assert.IsNotNull(idResponse.MobilePolicy);
            Assert.AreEqual(6, idResponse.MobilePolicy.PinLength);
            Assert.AreEqual(1, idResponse.MobilePolicy.ScreenLockTimeout);
        }

        private HttpStatusCode DoDescribe(String authToken)
        {
            String describeAccountPath = "/services/data/v26.0/sobjects/Account/describe";
            Dictionary<String, String> headers = (authToken == null ? null : new Dictionary<string, string> { { "Authorization", "Bearer " + authToken }});
            return HttpCall.CreateGet(headers, TestCredentials.INSTANCE_SERVER + describeAccountPath).Execute().Result.StatusCode;
        }
    
    }
}
