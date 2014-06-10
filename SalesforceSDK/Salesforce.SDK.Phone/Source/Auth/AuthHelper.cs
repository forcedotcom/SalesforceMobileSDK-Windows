using Salesforce.SDK.App;
using Salesforce.SDK.Source.Pages;
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
using System;
using System.Windows;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Salesforce.SDK.Auth
{
    /// <summary>
    /// Phone specific implementation if IAuthHelper
    /// </summary>
    public sealed class AuthHelper : IAuthHelper
    {
        /// <summary>
        /// Navigate to the /Pages/LoginPage.xaml and load login page in the webview
        /// </summary>
        /// <param name="loginOptions"></param>
        public async void StartLoginFlow()
        {
            Frame frame = Window.Current.Content as Frame;
            if (frame != null)
            {
                await frame.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    frame.Navigate(typeof(AccountPage));
                });

            }
        }

        /// <summary>
        /// Persist oauth credentials via the AccountManager and navigate back to previous screen
        /// </summary>
        /// <param name="loginOptions"></param>
        /// <param name="authResponse"></param>
        public async void EndLoginFlow(LoginOptions loginOptions, AuthResponse authResponse)
        {
            await AccountManager.CreateNewAccount(loginOptions, authResponse);
            Frame frame = Window.Current.Content as Frame;
            frame.Navigate(SalesforceApplication.RootApplicationPage);
            SalesforcePhoneApplication.ContinuationManager.MarkAsStale();
        }
    }
}
