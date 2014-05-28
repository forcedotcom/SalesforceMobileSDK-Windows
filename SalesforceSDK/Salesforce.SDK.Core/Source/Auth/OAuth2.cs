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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Salesforce.SDK.Auth
{
    /// <summary>
    /// object representing conncted application oauth configuration (login host, client id, callback url, oauth scopes)
    /// </summary>
    public class LoginOptions
    {
        public string LoginUrl { get; private set; }
        public string ClientId { get; private set; }
        public string CallbackUrl { get; private set; }
        public string[] Scopes { get; private set; }

        /// <summary>
        /// Constructor for LoginOptions
        /// </summary>
        /// <param name="loginUrl"></param>
        /// <param name="clientId"></param>
        /// <param name="callbackUrl"></param>
        /// <param name="scopes"></param>
        public LoginOptions(string loginUrl, string clientId, string callbackUrl, string[] scopes)
        {
            LoginUrl = loginUrl;
            ClientId = clientId;
            CallbackUrl = callbackUrl;
            Scopes = scopes;
        }
    }

    /// <summary>
    /// object representing the connected application mobile policy set by administrator
    /// </summary>
    public class MobilePolicy
    {
        /// <summary>
        /// Pin length required
        /// </summary>
        [JsonProperty(PropertyName = "pin_length")]
        public int PinLength { get; set; }

        /// <summary>
        /// Inactivite time after which the user should be prompted to enter her pin
        /// </summary>
        [JsonProperty(PropertyName = "screen_lock")]
        public int ScreenLockTimeout { get; set; }
    }

    /// <summary>
    /// object representing response from identity service
    /// </summary>
    public class IdentityResponse
    {
        /// <summary>
        /// URL for identity service
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string IdentityUrl { get; set; }

        /// <summary>
        /// Salesforce user id of authenticated user
        /// </summary>
        [JsonProperty(PropertyName = "user_id")]
        public string UserId { get; set; }

        /// <summary>
        /// Salesforce organization id of authenticated user
        /// </summary>
        [JsonProperty(PropertyName = "organization_id")]
        public string OrganizationId { get; set; }

        /// <summary>
        /// Salesforce username of authenticated user
        /// </summary>
        [JsonProperty(PropertyName = "username")]
        public string UserName { get; set; }

        /// <summary>
        /// Mobile policy for connected application set by administrator
        /// </summary>
        [JsonProperty(PropertyName = "mobile_policy")]
        public MobilePolicy MobilePolicy { get; set; }
    }

    /// <summary>
    /// object representing response from oauth service (during initial login flow or subsequent refresh flows)
    /// </summary>
    public class AuthResponse
    {
        /// <summary>
        /// URL for identity service
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string IdentityUrl { get; set; }

        /// <summary>
        /// Instance URL
        /// </summary>
        [JsonProperty(PropertyName = "instance_url")]
        public string InstanceUrl { get; set; }

        /// <summary>
        /// Date and time the oauth tokens were issued at
        /// </summary>
        [JsonProperty(PropertyName = "issued_at")]
        public string IssuedAt { get; set; }

        /// <summary>
        /// Access token
        /// </summary>
        [JsonProperty(PropertyName = "access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// Refresh token
        /// </summary>
        [JsonProperty(PropertyName = "refresh_token")]
        public string RefreshToken { get; set; }

        /// <summary>
        /// Auth scopes in a space delimited string
        /// </summary>
        [JsonProperty(PropertyName = "scope")]
        public string ScopesStr
        {
            set
            {
                Scopes = value.Split(' ');
            }
        }
        /// <summary>
        /// Auth scopes as a string array
        /// </summary>
        public string[] Scopes;
    }

    /// <summary>
    /// Utility class to interact with Salesforce oauth service
    /// </summary>
    public class OAuth2
    {
        // Refresh scope
        const string REFRESH_SCOPE = "refresh_token";

        // Authorization url
        const string OAUTH_AUTH_PATH = "/services/oauth2/authorize";
        const string OAUTH_AUTH_QUERY_STRING = "display=touch&response_type=token&client_id={0}&redirect_uri={1}&scope={2}";

        // Front door url
        const string FRONT_DOOR_PATH = "/secur/frontdoor.jsp";
        const string FRONT_DOOR_QUERY_STRING = "display=touch&sid={0}&retURL={1}";

        // Refresh url
        const string OAUTH_REFRESH_PATH = "/services/oauth2/token";
        const string OAUTH_REFRESH_QUERY_STRING = "grant_type=refresh_token&format=json&client_id={0}&refresh_token={1}";

        // Revoke url
        const string OAUTH_REVOKE_PATH = "/services/oauth2/revoke";
        const string OAUTH_REVOKE_QUERY_STRING = "token={0}";


        /// <summary>
        /// Build the URL to the authorization web page for this login server
        /// You need not provide refresh_token, as it is provided automatically
        /// </summary>
        /// <param name="loginOptions"></param>
        /// <returns>A URL to start the OAuth flow in a web browser/view.</returns>
        public static string ComputeAuthorizationUrl(LoginOptions loginOptions)
        {
            // Scope
            string scopeStr = string.Join(" ", loginOptions.Scopes.Concat(new string[] { REFRESH_SCOPE }).Distinct().ToArray());

            // Args
            string[] args = { loginOptions.ClientId, loginOptions.CallbackUrl, scopeStr };
            string[] urlEncodedArgs = args.Select(s => Uri.EscapeUriString(s)).ToArray();

            // Authorization url
            string authorizationUrl = string.Format(loginOptions.LoginUrl + OAUTH_AUTH_PATH + "?" + OAUTH_AUTH_QUERY_STRING, urlEncodedArgs);

            return authorizationUrl;
        }

        /// <summary>
        /// Build the front-doored URL for a given URL
        /// </summary>
        /// <param name="instanceUrl"></param>
        /// <param name="accessToken"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string ComputeFrontDoorUrl(string instanceUrl, string accessToken, string url)
        {
            // Args
            string[] args = { accessToken, url };
            string[] urlEncodedArgs = args.Select(s => Uri.EscapeUriString(s)).ToArray();

            // Authorization url
            string frontDoorUrl = string.Format(instanceUrl + FRONT_DOOR_PATH + "?" + FRONT_DOOR_QUERY_STRING, urlEncodedArgs);

            return frontDoorUrl;
        }

        /// <summary>
        /// Async method to get a new auth token by doing a refresh flow
        /// </summary>
        /// <param name="loginOptions"></param>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        public static async Task<AuthResponse> RefreshAuthToken(LoginOptions loginOptions, string refreshToken)
        {
            // Args
            string argsStr = string.Format(OAUTH_REFRESH_QUERY_STRING, new string[] { loginOptions.ClientId, refreshToken });

            // Refresh url
            string refreshUrl = loginOptions.LoginUrl + OAUTH_REFRESH_PATH;

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
        /// <returns>true if successful</returns>
        public static async Task<bool> RevokeAuthToken(LoginOptions loginOptions, string refreshToken)
        {
            // Args
            string argsStr = string.Format(OAUTH_REVOKE_QUERY_STRING, new string[] { refreshToken });

            // Revoke url
            string revokeUrl = loginOptions.LoginUrl + OAUTH_REVOKE_PATH;

            // Post
            HttpCall c = HttpCall.CreatePost(revokeUrl, argsStr);

            // Execute post
            return await c.Execute().ContinueWith(t => t.Result.StatusCode == HttpStatusCode.OK);
        }

        public async static void ClearCookies(LoginOptions loginOptions)
        {
            Frame frame = Window.Current.Content as Frame;
            if (frame != null)
            {
                await frame.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    Uri loginUri = new Uri(ComputeAuthorizationUrl(loginOptions));
                    WebView web = new WebView();
                    Windows.Web.Http.Filters.HttpBaseProtocolFilter myFilter = new Windows.Web.Http.Filters.HttpBaseProtocolFilter();
                    var cookieManager = myFilter.CookieManager;
                    Windows.Web.Http.HttpCookieCollection cookies = cookieManager.GetCookies(new Uri(loginUri.Scheme + "://" + loginUri.Host));
                    foreach (Windows.Web.Http.HttpCookie cookie in cookies)
                    {
                        cookieManager.DeleteCookie(cookie);
                    }
                });
            }
        }

        /// <summary>
        /// Async method to call the identity service (to get the mobile policy among other pieces of information)
        /// </summary>
        /// <param name="idUrl"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public static async Task<IdentityResponse> CallIdentityService(string idUrl, string accessToken)
        {
            // Auth header
            Dictionary<string, string> headers = new Dictionary<string, string>() { { "Authorization", "Bearer " + accessToken } };

            // Get
            HttpCall c = HttpCall.CreateGet(headers, idUrl);

            // Execute get
            return await c.ExecuteAndDeserialize<IdentityResponse>();
        }

        /// <summary>
        /// Extract the authentication data from the fragment portion of a URL
        /// </summary>
        /// <param name="fragmentstring"></param>
        /// <returns></returns>
        public static AuthResponse ParseFragment(string fragmentstring)
        {
            AuthResponse res = new AuthResponse();

            string[] parameters = fragmentstring.Split('&');
            foreach (string parameter in parameters)
            {
                string[] parts = parameter.Split('=');
                string name = Uri.UnescapeDataString(parts[0]);
                string value = parts.Length > 1 ? Uri.UnescapeDataString(parts[1]) : "";

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