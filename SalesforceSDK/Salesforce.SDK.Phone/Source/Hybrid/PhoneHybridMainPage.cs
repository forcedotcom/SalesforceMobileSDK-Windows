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
    /// </summary>
    public class PhoneHybridMainPage : PhoneApplicationPage
    {
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
            _bootConfig = BootConfig.GetBootConfig();
            _loginOptions = new LoginOptions("https://test.salesforce.com" /* FIXME once we have a server picker */,
                _bootConfig.ClientId, _bootConfig.CallbackURL, _bootConfig.Scopes);
            _clientManager = new ClientManager(_loginOptions);
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

        protected void LoadErrorPage()
        {
            LoadUri(new Uri("www/" + _bootConfig.ErrorPage, UriKind.Relative));
        }

        protected void LoadRemoteStartPage()
        {
            LoadUri(new Uri(OAuth2.ComputeFrontDoorUrl(_client.InstanceUrl, _client.AccessToken, _bootConfig.StartPage), UriKind.Absolute));
            _webAppLoaded = true;
        }

        protected void LoadLocalStartPage()
        {
            LoadUri(new Uri("www/" + _bootConfig.StartPage, UriKind.Relative));
            _webAppLoaded = true;
        }
    }

}