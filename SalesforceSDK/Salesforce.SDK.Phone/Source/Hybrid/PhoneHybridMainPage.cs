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
using Microsoft.Phone.Controls;
using Microsoft.Phone.Net.NetworkInformation;
using Newtonsoft.Json;
using Salesforce.SDK.Auth;
using Salesforce.SDK.Rest;
using System;
using System.Windows.Navigation;

namespace Salesforce.SDK.Hybrid
{
    /// <summary>
    /// Super class for Windows Phone hybrid application main page
    /// Note: some methods have empty implementations so it can't be used directly
    /// 
    /// TODO: some of the code below should move to the portable library (Salesforce.SDK.Core)
    /// TODO: set user agent
    /// TODO: configure HTML5 cache support
    /// TODO: capture app home url
    /// TODO: intercept redirect to login and do a refresh token when that happens
    /// TODO: change this to a UserControl?
    /// </summary>
    public class PhoneHybridMainPage : PhoneApplicationPage
    {
        private const string API_VERSION = "v26.0";
        private static PhoneHybridMainPage _instance;

        private BootConfig _bootConfig;
        private LoginOptions _loginOptions;
        private ClientManager _clientManager;
        private RestClient _client;
        private bool _webAppLoaded;

        /// <summary>
        /// Concrete hybrid main page page class should override this method to load uri in web view
        /// </summary>
        /// <param name="uri"></param>
        protected virtual void LoadUri(Uri uri) { }

        /// <summary>
        /// Constructor
        /// </summary>
        public PhoneHybridMainPage()
        {
            _instance = this;
            _bootConfig = BootConfig.GetBootConfig();
            _loginOptions = new LoginOptions("https://test.salesforce.com" /* FIXME once we have a server picker */,
                _bootConfig.ClientId, _bootConfig.CallbackURL, _bootConfig.Scopes);
            _clientManager = new ClientManager(_loginOptions);
        }

        /// <summary>
        /// Get active instance of PhoneHybridMainPage
        /// </summary>
        /// <returns></returns>
        public static PhoneHybridMainPage GetInstance()
        {
            return _instance;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _client = _clientManager.PeekRestClient();

            // Not logged in
            if (_client == null)
            {
                OnResumeNotLoggedIn();
            }
            // Logged in
            else
            {
                // Web app never loaded
                if (!_webAppLoaded)
                {
                    OnResumeLoggedInNotLoaded();
                }
                // Web app already loaded
                else
                {
                    // Nothing to do
                }
            }
        }

        /// <summary>
        /// Called when bringing up page and user is not authenticated
        /// </summary>
        protected void OnResumeNotLoggedIn()
        {
            // Need to be authenticated
            if (_bootConfig.ShouldAuthenticate)
            {
                // Online
                if (NetworkInterface.GetIsNetworkAvailable())
                {
                    _client = _clientManager.GetRestClient();
                    // After login, we will end up in OnResumeLoggedInNotLoaded
                }
                // Offline
                else
                {
                    LoadErrorPage();
                }
            }

            // Does not need to be authenticated
            else
            {
                // Local
                if (_bootConfig.IsLocal)
                {
                    LoadLocalStartPage();
                }
                // Remote
                else
                {
                    LoadErrorPage();
                }
            }
        }

        /// <summary>
        /// Called when bringing up page and user is authenticated but web view has not been loaded yet
        /// </summary>
        protected void OnResumeLoggedInNotLoaded()
        {
            // Local
            if (_bootConfig.IsLocal)
            {
                LoadLocalStartPage();
            }
            // Remote
            else
            {
                // Online
                if (NetworkInterface.GetIsNetworkAvailable())
                {
                    LoadRemoteStartPage();
                }
                // Offline
                else
                {
                    // Has cached version
                    if (false /* FIXME */)
                    {
                    }
                    // No cached version
                    else
                    {
                        LoadErrorPage();
                    }
                }
            }
        }

        /// <summary>
        /// Show error page in cordova view
        /// </summary>
        protected void LoadErrorPage()
        {
            LoadUri(new Uri("www/" + _bootConfig.ErrorPage, UriKind.Relative));
        }

        /// <summary>
        /// Show remote page in cordova view
        /// </summary>
        protected void LoadRemoteStartPage()
        {
            LoadUri(new Uri(OAuth2.ComputeFrontDoorUrl(_client.InstanceUrl, _client.AccessToken, _bootConfig.StartPage), UriKind.Absolute));
            _webAppLoaded = true;
        }

        /// <summary>
        /// Show local page in cordova view
        /// </summary>
        protected void LoadLocalStartPage()
        {
            LoadUri(new Uri("www/" + _bootConfig.StartPage, UriKind.Relative));
            _webAppLoaded = true;
        }

        /// <summary>
        /// Launch login flow if not authenticated or refresh auth token if already authenticated
        /// </summary>
        /// <param name="plugin"></param>
        public void Authenticate(SalesforceOAuthPlugin plugin)
        {
            _client = _clientManager.GetRestClient();
            if (_client == null)
            {
                // login flow will get started, when page is eventually reloaded, we will be called again and _client will not be null
            }
            else
            {
                /*
                 * Do a cheap REST call to refresh the access token if needed.
                 * If the login took place a while back (e.g. the already logged
                 * in application was restarted), then the returned session ID
                 * (access token) might be stale. This is not an issue if one
                 * uses exclusively RestClient for calling the server because
                 * it takes care of refreshing the access token when needed,
                 * but a stale session ID will cause the WebView to redirect
                 * to the web login.
                 */
                _client.SendAsync(RestRequest.GetRequestForResources(API_VERSION), (response) =>
                    {
                        if (!response.Success)
                        {
                            plugin.OnAuthenticateError(response.Error.Message);
                        }
                        else 
                        {
                            // TODO set sid cookies
                            plugin.OnAuthenticateSuccess(GetJSONCredentials());
                        }
                    });
            }
        }

        /// <summary>
        /// Return credentials as object with the fields expected by the javascript side
        /// </summary>
        /// <returns></returns>
        public JSONCredentials GetJSONCredentials()
        {
            return _client == null ? null : new JSONCredentials(AccountManager.GetAccount(), _client); // TODO have account be provided by RestClient?
        }

        /// <summary>
        /// Logout current user
        /// </summary>
        public void LogoutCurrentUser()
        {
            _clientManager.Logout();
        }

        /// <summary>
        ///  Gets the app's homepage as an absolute URL.  Used for attempting to load any cached
        ///  content that the developer may have built into the app (via HTML5 caching).
        ///  
        /// This method will either return the URL as a string, or an empty string if the URL has not been
        /// initialized.
        /// </summary>
        /// <returns></returns>
        public string GetAppHomeUrl()
        {
            return null; // TODO provide actual implementation
        }
    }

    /// <summary>
    /// Credentials for javascript consumption
    /// </summary>
    public class JSONCredentials
    {
        [JsonProperty(PropertyName = "userAgent")]
        public string UserAgent { get; set; }

        [JsonProperty(PropertyName = "instanceUrl")]
        public string InstanceUrl { get; set; }

        [JsonProperty(PropertyName = "loginUrl")]
        public string LoginUrl { get; set; }

        [JsonProperty(PropertyName = "identityUrl")]
        public string IdentityUrl { get; set; }

        [JsonProperty(PropertyName = "clientId")]
        public string ClientId { get; set; }

        [JsonProperty(PropertyName = "orgId")]
        public string OrgId { get; set; }

        [JsonProperty(PropertyName = "userId")]
        public string UserId { get; set; }

        [JsonProperty(PropertyName = "refreshToken")]
        public string RefreshToken { get; set; }

        [JsonProperty(PropertyName = "accessToken")]
        public string AccessToken { get; set; }

        public JSONCredentials(Account account, RestClient client)
        {
            AccessToken = client.AccessToken;
            LoginUrl = account.LoginUrl;
            InstanceUrl = account.InstanceUrl;
            LoginUrl = account.LoginUrl;
            ClientId = account.ClientId;
            RefreshToken = account.RefreshToken;
            UserAgent = "SalesforceMobileSDK/2.0 windows phone"; // FIXME
            // TODO wire through the other fields
        }
    }

}