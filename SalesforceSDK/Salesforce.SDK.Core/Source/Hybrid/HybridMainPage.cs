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
using Salesforce.SDK.Auth;
using Salesforce.SDK.Rest;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using Windows.Networking.Connectivity;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Salesforce.SDK.Source.Utilities;
using Salesforce.SDK.App;

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
    /// TODO: authenticate should also refresh VF cookies by hitting the "ping" page in a hidden WebBrowser
    /// TODO: change this to a UserControl?
    /// FIXME: resuming app causes a reload of web view (so web state is reset)
    /// </summary>
    public class HybridMainPage : Page, ISalesforcePage
    {
        private const string API_VERSION = "v30.0";
        private static HybridMainPage _instance;

        private SynchronizationContext _syncContext;
        private BootConfig _bootConfig;
        private LoginOptions _loginOptions;
        private RestClient _client;
        private bool _webAppLoaded;

        /// <summary>
        /// Concrete hybrid main page page class should override this method and return the cordova view
        /// </summary>
        /// <returns></returns>
        protected virtual WebView GetCordovaView() { return null; }

        /// <summary>
        /// Constructor
        /// </summary>
        public HybridMainPage()
        {
            _instance = this;
            _syncContext = SynchronizationContext.Current;
            _bootConfig = BootConfig.GetBootConfig();
        }

        /// <summary>
        /// Get active instance of PhoneHybridMainPage
        /// </summary>
        /// <returns></returns>
        public static HybridMainPage GetInstance()
        {
            return _instance;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _client = SalesforceApplication.GlobalClientManager.PeekRestClient();

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
                    Log("Starting authentication flow");
                    _client = SalesforceApplication.GlobalClientManager.GetRestClient();
                    // After login, we will end up in OnResumeLoggedInNotLoaded
                }
                // Offline
                else
                {
                    Log("Error:Can't start application that requires authentication while offline");
                    LoadErrorPage();
                }
            }

            // Does not need to be authenticated
            else
            {
                // Local
                if (_bootConfig.IsLocal)
                {
                    Log("Success:Loading local application - no authentication required");
                    LoadLocalStartPage();
                }
                // Remote
                else
                {
                    Log("Success:Loading remote application - no authentication required");
                    LoadRemoteStartPage();
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
                Log("Success:Loading local application");
                LoadLocalStartPage();
            }
            // Remote
            else
            {
                // Online
                if (NetworkInterface.GetIsNetworkAvailable())
                {
                    Log("Success:Loading remote application");
                    LoadRemoteStartPage();
                }
                // Offline
                else
                {
                    // Has cached version
                    if (false /* FIXME */)
                    {
                        Log("Success:Loading cached version of remote application because offline");
                    }
                    // No cached version
                    else
                    {
                        Log("Error:Can't load remote application offline without cached version");
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
            Uri uri = new Uri("www/" + _bootConfig.ErrorPage, UriKind.Relative);
            LoadUri(uri);
        }

        /// <summary>
        /// Show remote page in cordova view
        /// </summary>
        protected void LoadRemoteStartPage()
        {
            Uri uri = new Uri(OAuth2.ComputeFrontDoorUrl(_client.InstanceUrl, _client.AccessToken, _bootConfig.StartPage), UriKind.Absolute);
            LoadUri(uri);
        }

        /// <summary>
        /// Show local page in cordova view
        /// </summary>
        protected void LoadLocalStartPage()
        {
            Uri uri = new Uri("www/" + _bootConfig.StartPage, UriKind.Relative);
            LoadUri(uri);
        }

        private void LoadUri(Uri uri)
        {
            WebView browser = GetCordovaView();
            browser.NavigationStarting += onBrowserNavigationStarting;
            if (GetCordovaView().Source.Equals(uri))
            {
                // Already there - maybe back in after a log out
                // Manually reloading page
                browser.Navigate(uri);
            }
            else
            {
                // That only works before the view is loaded
                GetCordovaView().Source = uri;
            }
            _webAppLoaded = true;
        }

        private void onBrowserNavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            string startURL = IsLoginRedirect(args.Uri);
            if (startURL != null)
            {
                sender.Stop();
                // Cheap REST call to refresh session
                _client.SendAsync(RestRequest.GetRequestForResources(API_VERSION), (response) =>
                    {
                        Uri frontDoorStartURL = new Uri(OAuth2.ComputeFrontDoorUrl(_client.InstanceUrl, _client.AccessToken, startURL));
                        _syncContext.Post((state) => { sender.Navigate(state as Uri); }, frontDoorStartURL);
                    }
                );
            }
        }

        /// <summary>
        ///  Login redirect are of the form https://host/?ec=30x&startURL=xyz
        /// </summary>
        /// <param name="uri"></param>
        /// <returns>null if this is not a login redirect and return the the value for startURL if this is a login redirect</returns>
        private string IsLoginRedirect(Uri uri)
        {
            if (uri != null
                && uri.IsAbsoluteUri
                && uri.AbsolutePath != null && uri.AbsolutePath == "/"
                && uri.Query != null) 
            {
                Dictionary<string, string> qs = uri.ParseQueryString();
                if ((qs["ec"] == "301" || qs["ec"] == "302") && qs["startURL"] != null)
                {
                    return qs["startURL"];
                }
            }
            
            return null;
        }

        /// <summary>
        /// Launch login flow if not authenticated or refresh auth token if already authenticated
        /// </summary>
        /// <param name="plugin"></param>
        public void Authenticate(SalesforceOAuthPlugin plugin)
        {
            _client = SalesforceApplication.GlobalClientManager.GetRestClient();
            if (_client == null)
            {
                // login flow will get started, when page is eventually reloaded, we will be called again and _client will not be null
            }
            else
            {
                RefreshSession(plugin);
            }
        }

        private void RefreshSession(SalesforceOAuthPlugin plugin)
        {
            // Cheap REST call to refresh session
            _client.SendAsync(RestRequest.GetRequestForResources(API_VERSION), (response) =>
            {
                if (plugin != null)
                {
                    if (!response.Success)
                    {
                        plugin.OnAuthenticateError(response.Error.Message);
                    }
                    else
                    {
                        plugin.OnAuthenticateSuccess(GetJSONCredentials());
                    }
                }
            });
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
        public async void LogoutCurrentUser()
        {
            _webAppLoaded = false;
            await SalesforceApplication.GlobalClientManager.Logout();
            _syncContext.Post((state) => { Authenticate(null); }, null); // XXX Authenticate might call Navigate so it must be done on the UI thread
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

        private void Log(string p)
        {
            Debug.WriteLine("PhoneHybridMainPage:" + p);
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
            ClientId = account.ClientId;
            RefreshToken = account.RefreshToken;
            UserAgent = "SalesforceMobileSDK/2.0 windows phone"; // FIXME
            // TODO wire through the other fields
        }
    }

}