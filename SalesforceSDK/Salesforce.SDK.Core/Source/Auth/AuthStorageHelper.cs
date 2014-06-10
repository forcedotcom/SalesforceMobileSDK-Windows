using Newtonsoft.Json;
using Salesforce.SDK.App;
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
using Salesforce.SDK.Auth;
using Salesforce.SDK.Source.Security;
using Salesforce.SDK.Source.Settings;
using System;
using System.Collections.Generic;
using Windows.Storage;

namespace Salesforce.SDK.Auth
{
    /// <summary>
    /// Store specific implementation if IAuthStorageHelper
    /// </summary>    /// </summary>
    internal class AuthStorageHelper
    {
        private const string ACCOUNT_SETTING = "accounts";
        private const string CURRENT_ACCOUNT = "currentAccount";

        /// <summary>
        /// Persist account, and sets account as the current account.
        /// </summary>
        /// <param name="account"></param>
        internal void PersistCredentials(Account account)
        {
            // TODO use PasswordVault
            ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;
            Dictionary<string, Account> accounts = RetrievePersistedCredentials();
            if (accounts.ContainsKey(account.UserId))
            {
                accounts[account.UserId] = account;
            } else
            {
                accounts.Add(account.UserId, account);
            }
            String accountJson = JsonConvert.SerializeObject(accounts);
            settings.Values[ACCOUNT_SETTING] = Encryptor.Encrypt(accountJson);
            settings.Values[CURRENT_ACCOUNT] = Encryptor.Encrypt(account.UserId);
            LoginOptions options = new LoginOptions(account.LoginUrl, account.ClientId, account.CallbackUrl, account.Scopes);
            SalesforceConfig.LoginOptions = options;
        }

        internal Account RetrieveCurrentAccount()
        {
            ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;
            String key = settings.Values[CURRENT_ACCOUNT] as string;
            if (String.IsNullOrWhiteSpace(key))
                return null;
            return RetrievePersistedCredential(Encryptor.Decrypt(key));
        }

        /// <summary>
        /// Retrieve an account based on the id of the user.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal Account RetrievePersistedCredential(String id)
        {
            Dictionary<string, Account> accounts = RetrievePersistedCredentials();
            if (accounts.ContainsKey(id))
                return accounts[id];
            return null;
        }

        /// <summary>
        /// Retrieve persisted account
        /// </summary>
        /// <returns></returns>
        internal Dictionary<string, Account> RetrievePersistedCredentials()
        {
            ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;
            string accountJson = settings.Values[ACCOUNT_SETTING] as string;
            if (String.IsNullOrWhiteSpace(accountJson))
                return new Dictionary<string, Account>();
            return JsonConvert.DeserializeObject<Dictionary<string, Account>>(Encryptor.Decrypt(accountJson));
        }

        /// <summary>
        /// Delete a persisted account credential based on the user id.
        /// </summary>
        /// <param name="id"></param>
        internal void DeletePersistedCredentials(String id)
        {
            ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;
            Dictionary<string, Account> accounts = RetrievePersistedCredentials();
            accounts.Remove(id);
            String accountJson = JsonConvert.SerializeObject(accounts);
            settings.Values[ACCOUNT_SETTING] = Encryptor.Encrypt(accountJson);
        }
        /// <summary>
        /// Delete all persisted accounts
        /// </summary>
        internal void DeletePersistedCredentials()
        {
            ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;
            settings.Values.Remove(ACCOUNT_SETTING);
        }
    }
}
