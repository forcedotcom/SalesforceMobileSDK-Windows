using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Salesforce.WinSDK.Net;
using System;
using System.Collections.Generic;
using System.Net;

namespace Salesforce.WinSDK.Auth
{
    [TestClass]
    public class OAuth2Test
    {
        [TestMethod]
        public void testGetAuthorizationUrl()
        {
            String loginServer = "https://login.salesforce.com";
            String clientId = "TEST_CLIENT_ID";
            String callbackUrl = "test://sfdc";
            String[] scopes = { "web", "api" };

            String expectedUrl = "https://login.salesforce.com/services/oauth2/authorize?display=mobile&response_type=token&client_id=TEST_CLIENT_ID&redirect_uri=test://sfdc&scope=web%20api%20refresh_token";
            String actualUrl = OAuth2.getAuthorizationUrl(loginServer, clientId, callbackUrl, scopes);
            Assert.AreEqual(expectedUrl, actualUrl, "Wrong authorization url");
        }

        [TestMethod]
        public void testRefreshAuthToken()
        {
            // Try describe without being authenticated, expect 401
            Assert.AreEqual(HttpStatusCode.Unauthorized, doDescribe(null));

            // Get auth token (through refresh)
            String loginServer = "https://test.salesforce.com";
            String clientId = "3MVG92.uWdyphVj4bnolD7yuIpCQsNgddWtqRND3faxrv9uKnbj47H4RkwheHA2lKY4cBusvDVp0M6gdGE8hp";
            String refreshToken = "5Aep861_OKMvio5gy9sGt9Z9mdt62xXK.9ugif6nZJYknXeANTICBf4ityN9j6YDgHjFvbzu6FTUQ==";
            RefreshResponse refreshResponse = OAuth2.refreshAuthToken(loginServer, clientId, refreshToken).Result;

            // Try describe again, expect 200
            Assert.AreEqual(HttpStatusCode.OK, doDescribe(refreshResponse.AccessToken));
        }


        private HttpStatusCode doDescribe(String authToken)
        {
            String instanceServer = "https://tapp0.salesforce.com";
            String describeAccountPath = "/services/data/v26.0/sobjects/Account/describe";
            Dictionary<String, String> headers = (authToken == null ? null : new Dictionary<string, string> { { "Authorization", "Bearer " + authToken }});
            return HttpCall.createGet(headers, instanceServer + describeAccountPath).execute().Result.StatusCode;
        }
    
    }
}
