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
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Salesforce.SDK.Adaptation;
using Salesforce.SDK.App;
using Salesforce.SDK.Rest;

namespace Salesforce.SDK.Auth
{
    /// <summary>
    ///     Class providing (static) methods for creating/deleting or retrieving an Account
    /// </summary>
    public class AccountManager
    {
        /// <summary>
        ///     Delete Account for currently authenticated user
        /// </summary>
        public static void DeleteAccount()
        {
            Account account = GetAccount();
            AuthStorageHelper.GetAuthStorageHelper().DeletePersistedCredentials(account.UserName, account.UserId);
        }

        public static Dictionary<string, Account> GetAccounts()
        {
            return AuthStorageHelper.GetAuthStorageHelper().RetrievePersistedCredentials();
        }

        /// <summary>
        ///     Return Account for currently authenticated user
        /// </summary>
        /// <returns></returns>
        public static Account GetAccount()
        {
            return AuthStorageHelper.GetAuthStorageHelper().RetrieveCurrentAccount();
        }

        public static async Task<bool> SwitchToAccount(Account account)
        {
            if (account != null && account.UserId != null)
            {
                PincodeManager.SavePinTimer();
                AuthStorageHelper.GetAuthStorageHelper().PersistCredentials(account);
                RestClient client = SalesforceApplication.GlobalClientManager.PeekRestClient();
                if (client != null)
                {
                    OAuth2.ClearCookies(account.GetLoginOptions());
                    IdentityResponse identity = await OAuth2.CallIdentityService(account.IdentityUrl, client);
                    if (identity != null)
                    {
                        account.UserId = identity.UserId;
                        account.UserName = identity.UserName;
                        account.Policy = identity.MobilePolicy;
                        AuthStorageHelper.GetAuthStorageHelper().PersistCredentials(account);
                    }
                    OAuth2.RefreshCookies();
                    return true;
                }
            }
            return false;
        }

        public static void WipeAccounts()
        {
            AuthStorageHelper.GetAuthStorageHelper().DeletePersistedCredentials();
            PincodeManager.WipePincode();
            SwitchAccount();
        }

        public static void SwitchAccount()
        {
            PlatformAdapter.Resolve<IAuthHelper>().StartLoginFlow();
        }

        /// <summary>
        ///     Create and persist Account for newly authenticated user
        /// </summary>
        /// <param name="loginOptions"></param>
        /// <param name="authResponse"></param>
        public static async Task<Account> CreateNewAccount(LoginOptions loginOptions, AuthResponse authResponse)
        {
            var account = new Account(loginOptions.LoginUrl, loginOptions.ClientId, loginOptions.CallbackUrl,
                loginOptions.Scopes,
                authResponse.InstanceUrl, authResponse.IdentityUrl, authResponse.AccessToken, authResponse.RefreshToken);
            account.CommunityId = authResponse.CommunityId;
            account.CommunityUrl = authResponse.CommunityUrl;
            var cm = new ClientManager();
            cm.PeekRestClient();
            IdentityResponse identity = null;
            try
            {
                identity = await OAuth2.CallIdentityService(authResponse.IdentityUrl, authResponse.AccessToken);
            }
            catch (JsonException)
            {
                Debug.WriteLine("Error retrieving account identity");
            }
            if (identity != null)
            {
                account.UserId = identity.UserId;
                account.UserName = identity.UserName;
                account.Policy = identity.MobilePolicy;
                AuthStorageHelper.GetAuthStorageHelper().PersistCredentials(account);
            }
            return account;
        }
    }
}