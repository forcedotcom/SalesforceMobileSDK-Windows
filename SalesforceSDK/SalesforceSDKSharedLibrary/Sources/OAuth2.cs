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
using Salesforce.WinSDK.Net;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Salesforce.WinSDK.Auth
{
    public class RefreshResponse
    {
        [JsonProperty(PropertyName="id")] 
        public String Id { get; set; }
        
        [JsonProperty(PropertyName="instance_url")] 
        public String InstanceUrl { get; set; }
        
        [JsonProperty(PropertyName = "issued_at")]
        public String IssuedAt { get; set; }
        
        [JsonProperty(PropertyName = "signature")]
        public String Signature { get; set; }
        
        [JsonProperty(PropertyName = "access_token")]
        public String AccessToken { get; set; }
    }

    public class OAuth2
    {
        // Refresh scope
        const String REFRESH_SCOPE = "refresh_token";

        // Access token

        // Authorization url
        const String OAUTH_AUTH_PATH = "/services/oauth2/authorize";
        const String OAUTH_AUTH_QUERY_STRING = "display=mobile&response_type=token&client_id={0}&redirect_uri={1}&scope={2}";

        // Refresh url
        const String OAUTH_REFRESH_PATH = "/services/oauth2/token";
        const String OAUTH_REFRESH_QUERY_STRING = "grant_type=refresh_token&format=json&client_id={0}&refresh_token={1}";


        /**
         * Build the URL to the authorization web page for this login server.
         * You need not provide refresh_token, as it is provided automatically.
         *
         * @param loginServer
         *            the base protocol and server to use (e.g.
         *            https://login.salesforce.com)
         * @param clientId
         *            OAuth client ID
         * @param callbackUrl
         *            OAuth callback url
         * @param scopes A list of OAuth scopes to request (eg {"visualforce","api"}).
         * @return A URL to start the OAuth flow in a web browser/view.
         *
         * @see <a href="https://help.salesforce.com/apex/HTViewHelpDoc?language=en&id=remoteaccess_oauth_scopes.htm">RemoteAccess OAuth Scopes</a>
         *
         */
        public static String getAuthorizationUrl(String loginServer, String clientId, String callbackUrl, String[] scopes)
        {
            // Scope
            String scopeStr = String.Join(" ", scopes.Concat(new String[] {REFRESH_SCOPE}).Distinct().ToArray());

            // Args
            String[] args = {clientId, callbackUrl, scopeStr};
            String[] urlEncodedArgs = args.Select(s => Uri.EscapeUriString(s)).ToArray();

            // Authorization url
            String authorizationUrl = String.Format(loginServer + OAUTH_AUTH_PATH + "?" + OAUTH_AUTH_QUERY_STRING, urlEncodedArgs);

            return authorizationUrl;
        }


        public static async Task<RefreshResponse> refreshAuthToken(String loginServer, String clientId, String refreshToken)
        {
            // Args
            String argsStr = String.Format(OAUTH_REFRESH_QUERY_STRING, new String[] {clientId, refreshToken});
            
            // Refresh url
            String refreshUrl = loginServer + OAUTH_REFRESH_PATH;

            // Post
            HttpCall c = HttpCall.createPost(refreshUrl, argsStr);

            // Execute post
            return await c.execute().ContinueWith(t => JsonConvert.DeserializeObject<RefreshResponse>(t.Result.ResponseBody) );
        }
    
    }
}
