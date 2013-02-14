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
using Salesforce.SDK.Net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Salesforce.SDK.Auth
{
    /// <summary>
    /// Object representing conncted application oauth configuration (login host, client id, callback url, oauth scopes)
    /// </summary>
    public class LoginOptions
    {
        public String LoginUrl    { get; private set; }
        public String ClientId    { get; private set; }
        public String CallbackUrl { get; private set; }
        public String[] Scopes    { get; private set; }

        /// <summary>
        /// Constructor for LoginOptions
        /// </summary>
        /// <param name="loginUrl"></param>
        /// <param name="clientId"></param>
        /// <param name="callbackUrl"></param>
        /// <param name="scopes"></param>
        public LoginOptions(String loginUrl, String clientId, String callbackUrl, String[] scopes)
        {
            LoginUrl = loginUrl;
            ClientId = clientId;
            CallbackUrl = callbackUrl;
            Scopes = scopes;
        }
    }

    /// <summary>
    /// Object representing the connected application mobile policy set by administrator
    /// </summary>
    public class MobilePolicy
    {
        /// <summary>
        /// Pin length required
        /// </summary>
        [JsonProperty(PropertyName="pin_length")]
        public int PinLength { get; set; }

        /// <summary>
        /// Inactivite time after which the user should be prompted to enter her pin
        /// </summary>
        [JsonProperty(PropertyName="screen_lock")]
        public int ScreenLockTimeout { get; set; }
    }

    /// <summary>
    /// Object representing response from identity service
    /// </summary>
    public class IdentityResponse
    {
        /// <summary>
        /// URL for identity service
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public String IdentityUrl { get; set; }

        /// <summary>
        /// Salesforce user id of authenticated user
        /// </summary>
        [JsonProperty(PropertyName = "user_id")]
        public String UserId { get; set; }

        /// <summary>
        /// Salesforce organization id of authenticated user
        /// </summary>
        [JsonProperty(PropertyName = "organization_id")]
        public String OrganizationId { get; set; }

        /// <summary>
        /// Salesforce username of authenticated user
        /// </summary>
        [JsonProperty(PropertyName="username")] 
        public String UserName { get; set; }

        /// <summary>
        /// Mobile policy for connected application set by administrator
        /// </summary>
        [JsonProperty(PropertyName="mobile_policy")]
        public MobilePolicy MobilePolicy {get; set; }
    }

    /// <summary>
    /// Object representing response from oauth service (during initial login flow or subsequent refresh flows)
    /// </summary>
    public class AuthResponse
    {
        /// <summary>
        /// URL for identity service
        /// </summary>
        [JsonProperty(PropertyName="id")] 
        public String IdentityUrl { get; set; }
        
        /// <summary>
        /// Instance URL
        /// </summary>
        [JsonProperty(PropertyName="instance_url")] 
        public String InstanceUrl { get; set; }
        
        /// <summary>
        /// Date and time the oauth tokens were issued at
        /// </summary>
        [JsonProperty(PropertyName = "issued_at")]
        public String IssuedAt { get; set; }
        
        /// <summary>
        /// Access token
        /// </summary>
        [JsonProperty(PropertyName = "access_token")]
        public String AccessToken { get; set; }

        /// <summary>
        /// Refresh token
        /// </summary>
        [JsonProperty(PropertyName = "refresh_token")]
        public String RefreshToken { get; set; }

        /// <summary>
        /// Auth scopes in a space delimited string
        /// </summary>
        [JsonProperty(PropertyName = "scope")]
        public String ScopesStr {
            set
            {
                Scopes = value.Split(' ');
            }
        }
        /// <summary>
        /// Auth scopes as a String array
        /// </summary>
        public String[] Scopes;
    }

    /// <summary>
    /// Utility class to interact with Salesforce oauth service
    /// </summary>
    public class OAuth2
    {
        // Refresh scope
        const String REFRESH_SCOPE = "refresh_token";

        // Authorization url
        const String OAUTH_AUTH_PATH = "/services/oauth2/authorize";
        const String OAUTH_AUTH_QUERY_STRING = "display=touch&response_type=token&client_id={0}&redirect_uri={1}&scope={2}";

        // Refresh url
        const String OAUTH_REFRESH_PATH = "/services/oauth2/token";
        const String OAUTH_REFRESH_QUERY_STRING = "grant_type=refresh_token&format=json&client_id={0}&refresh_token={1}";

        // Revoke url
        const String OAUTH_REVOKE_PATH = "/services/oauth2/refresh";
        const String OAUTH_REVOKE_QUERY_STRING = "token={0}";


        /// <summary>
        /// Build the URL to the authorization web page for this login server
        /// You need not provide refresh_token, as it is provided automatically
        /// </summary>
        /// <param name="loginOptions"></param>
        /// <returns>A URL to start the OAuth flow in a web browser/view.</returns>
        public static String ComputeAuthorizationUrl(LoginOptions loginOptions)
        {
            // Scope
            String scopeStr = String.Join(" ", loginOptions.Scopes.Concat(new String[] {REFRESH_SCOPE}).Distinct().ToArray());

            // Args
            String[] args = {loginOptions.ClientId, loginOptions.CallbackUrl, scopeStr };
            String[] urlEncodedArgs = args.Select(s => Uri.EscapeUriString(s)).ToArray();

            // Authorization url
            String authorizationUrl = String.Format(loginOptions.LoginUrl + OAUTH_AUTH_PATH + "?" + OAUTH_AUTH_QUERY_STRING, urlEncodedArgs);

            return authorizationUrl;
        }

        /// <summary>
        /// Async method to get a new auth token by doing a refresh flow
        /// </summary>
        /// <param name="loginOptions"></param>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        public static async Task<AuthResponse> RefreshAuthToken(LoginOptions loginOptions, String refreshToken)
        {
            // Args
            String argsStr = String.Format(OAUTH_REFRESH_QUERY_STRING, new String[] { loginOptions.ClientId, refreshToken });
            
            // Refresh url
            String refreshUrl = loginOptions.LoginUrl + OAUTH_REFRESH_PATH;

            // Post
            HttpCall c = HttpCall.CreatePost(refreshUrl, argsStr);

            // Execute post
            return await c.ExecuteAndDeserialize<AuthResponse>();
        }

        /// <summary>
        /// Async method to revoke the user's refresh token (i.e. do a server-side logout for the authenticated user)
        /// </summary>
        /// <param name="loginOptions"></param>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        public static async Task<HttpStatusCode> RevokeAuthToken(LoginOptions loginOptions, String refreshToken)
        {
            // Args
            String argsStr = String.Format(OAUTH_REVOKE_QUERY_STRING, new String[] { refreshToken });

            // Refresh url
            String revokeUrl = loginOptions.LoginUrl + OAUTH_REVOKE_PATH;

            // Post
            HttpCall c = HttpCall.CreatePost(revokeUrl, argsStr);

            // Execute post
            return await c.Execute().ContinueWith(t => t.Result.StatusCode);
        }

        /// <summary>
        /// Async method to call the identity service (to get the mobile policy among other pieces of information)
        /// </summary>
        /// <param name="idUrl"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public static async Task<IdentityResponse> CallIdentityService(String idUrl, String accessToken)
        {
            // Auth header
            Dictionary<String, String> headers = new Dictionary<String, String>() {{"Authorization", "Bearer " + accessToken }};

            // Get
            HttpCall c = HttpCall.CreateGet(headers, idUrl);

            // Execute get
            return await c.ExecuteAndDeserialize<IdentityResponse>();
        }

        /// <summary>
        /// Extract the authentication data from the fragment portion of a URL
        /// </summary>
        /// <param name="fragmentString"></param>
        /// <returns></returns>
        public static AuthResponse ParseFragment(String fragmentString)
        {
            AuthResponse res = new AuthResponse();

            String[] parameters = fragmentString.Split('&');
            foreach (String parameter in parameters)
            {
                String[] parts = parameter.Split('=');
                String name = Uri.UnescapeDataString(parts[0]);
                String value = parts.Length > 1 ? Uri.UnescapeDataString(parts[1]) : "";

                switch (name)
                {
                    case "id": res.IdentityUrl = value; break;
                    case "instance_url": res.InstanceUrl = value; break;
                    case "access_token": res.AccessToken = value; break;
                    case "refresh_token": res.RefreshToken = value; break;
                    case "issued_at": res.IssuedAt = value; break;
                    case "scope": res.Scopes = value.Split('+'); break;
                    default: Debug.WriteLine("Parameter not recognized {0}", name); break;
                }
            }
            return res;
        }
    }
}