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
using Salesforce.SDK.Adaptation;
using Salesforce.SDK.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salesforce.SDK.Source.Auth
{

    public class Account
    {
        public String LoginUrl     { get; private set; }
        public String ClientId     { get; private set; }
        public String CallbackUrl  { get; private set; }
        public String[] Scopes     { get; private set; }
        public String InstanceUrl  { get; private set; }
        public String AccessToken  { get; set; }
        public String RefreshToken { get; private set; }

        public Account(String loginUrl, String clientId, String callbackUrl, String[] scopes, String instanceUrl, String accessToken, String refreshToken)
        {
            LoginUrl = loginUrl;
            ClientId = clientId;
            CallbackUrl = callbackUrl;
            Scopes = scopes;
            InstanceUrl = instanceUrl;
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }

        public static String ToJson(Account account)
        {
            return JsonConvert.SerializeObject(account);
        }

        public static Account fromJson(String accountJson)
        {
            return JsonConvert.DeserializeObject<Account>(accountJson);
        }
    }

    public class AccountManager
    {
        public static void DeleteAccount()
        {
            PlatformAdapter.Resolve<IAuthStorageHelper>().DeletePersistedCredentials();
        }

        public static Account GetAccount()
        {
            return PlatformAdapter.Resolve<IAuthStorageHelper>().RetrievePersistedCredentials();
        }

        public static void CreateNewAccount(LoginOptions loginOptions, AuthResponse authResponse)
        {
            Account account = new Account(loginOptions.LoginUrl, loginOptions.ClientId, loginOptions.CallbackUrl, loginOptions.Scopes,
                authResponse.InstanceUrl, authResponse.AccessToken, authResponse.RefreshToken);
            PlatformAdapter.Resolve<IAuthStorageHelper>().PersistCredentials(account);
        }
    }
}
