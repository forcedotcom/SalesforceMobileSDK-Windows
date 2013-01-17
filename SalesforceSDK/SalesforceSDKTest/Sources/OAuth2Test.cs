using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;

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
            String loginServer = "https://test.salesforce.com";
            String clientId = "3MVG92.uWdyphVj4bnolD7yuIpCQsNgddWtqRND3faxrv9uKnbj47H4RkwheHA2lKY4cBusvDVp0M6gdGE8hp";
            String refreshToken = "3MVG92.uWdyphVj4bnolD7yuIpCQsNgddWtqRND3faxrv9uKnbj47H4RkwheHA2lKY4cBusvDVp0M6gdGE8hp";
            String authToken = OAuth2.refreshAuthToken(loginServer, clientId, refreshToken).Result;

        }
    }
}
