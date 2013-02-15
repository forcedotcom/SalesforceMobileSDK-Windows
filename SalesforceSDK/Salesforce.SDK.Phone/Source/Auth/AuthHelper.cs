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
using Salesforce.SDK.Source.Auth;
using System;
using System.Windows;

namespace Salesforce.SDK.Auth
{
    public class AuthHelper : IAuthHelper
    {
        private const string ACCOUNT_SETTING = "account";

        public const string LOGIN_SERVER = "loginServer";
        public const string CLIENT_ID = "clientId";
        public const string CALLBACK_URL = "callbackUrl";
        public const string SCOPES = "scopes";

        public void StartLoginFlow(LoginOptions loginOptions)
        {
            string loginUrl = Uri.EscapeUriString(loginOptions.LoginUrl);
            string clientId = Uri.EscapeUriString(loginOptions.ClientId);
            string callbackUrl = Uri.EscapeUriString(loginOptions.CallbackUrl);
            string scopes = Uri.EscapeUriString(string.Join(" ", loginOptions.Scopes));
            string QueryString = string.Format("?{0}={1}&{2}={3}&{4}={5}&{6}={7}", LOGIN_SERVER, loginUrl, CLIENT_ID, clientId, CALLBACK_URL, callbackUrl, SCOPES, scopes);

            ((PhoneApplicationFrame) Application.Current.RootVisual).Navigate(new Uri("/Pages/LoginPage.xaml" + QueryString, UriKind.Relative));
        }

        public void EndLoginFlow(LoginOptions loginOptions, AuthResponse authResponse)
        {
            AccountManager.CreateNewAccount(loginOptions, authResponse);
            ((PhoneApplicationFrame) Application.Current.RootVisual).GoBack();
        }
    }
}
