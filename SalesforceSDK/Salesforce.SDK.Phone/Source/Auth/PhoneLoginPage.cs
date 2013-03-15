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
using Salesforce.SDK.Adaptation;
using System;
using System.Collections.Generic;
using System.Windows.Navigation;

namespace Salesforce.SDK.Auth
{
    /// <summary>
    /// Super class for Windows Phone login page
    /// Note: some methods have empty implementations so it can't be used directly
    /// TODO: change this to a UserControl?
    /// FIXME: logout following login goes to approve/deny even though token is revoked
    /// </summary>
    public class PhoneLoginPage : PhoneApplicationPage
    {
        /// <summary>
        /// Concrete login page class should override this method to return actual webview where login flow will take place
        /// </summary>
        /// <returns></returns>
        public virtual WebBrowser WebViewControl() { return null; }

        private LoginOptions _loginOptions;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            IDictionary<string, string> qs = NavigationContext.QueryString;
            _loginOptions = new LoginOptions(qs[AuthHelper.LOGIN_SERVER], qs[AuthHelper.CLIENT_ID], qs[AuthHelper.CALLBACK_URL], qs[AuthHelper.SCOPES].Split(' '));
            Uri loginUri = new Uri(OAuth2.ComputeAuthorizationUrl(_loginOptions));
            WebBrowser wv = WebViewControl();
            wv.Navigating += OnNavigating;
            wv.Source = loginUri;
        }

        private void OnNavigating(object sender, NavigatingEventArgs e)
        {
            if (e.Uri.ToString().StartsWith(_loginOptions.CallbackUrl) && e.Uri.Fragment.Length > 0)
            {
                e.Cancel = true;
                AuthResponse authResponse = OAuth2.ParseFragment(e.Uri.Fragment.Substring(1));
                PlatformAdapter.Resolve<IAuthHelper>().EndLoginFlow(_loginOptions, authResponse);
            }
        }
    }

}