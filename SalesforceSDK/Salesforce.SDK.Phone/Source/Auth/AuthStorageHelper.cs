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
using System.IO.IsolatedStorage;

namespace Salesforce.SDK.Auth
{
    /// <summary>
    /// Phone specific implementation if IAuthStorageHelper
    /// </summary>
    public class AuthStorageHelper : IAuthStorageHelper
    {
        private const string ACCOUNT_SETTING = "account";

        /// <summary>
        /// Persist account
        /// </summary>
        /// <param name="account"></param>
        public void PersistCredentials(Account account)
        {
            // TODO store encrypted credentials
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            string accountJson = Account.ToJson(account);
            if (settings.Contains(ACCOUNT_SETTING))
            {
                settings[ACCOUNT_SETTING] = accountJson;
            }
            else
            {
                settings.Add(ACCOUNT_SETTING, accountJson);
            }
            settings.Save();
        }

        /// <summary>
        /// Retrieve persisted account
        /// </summary>
        /// <returns></returns>
        public Account RetrievePersistedCredentials()
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            string accountJson;
            if (settings.TryGetValue<string>(ACCOUNT_SETTING, out accountJson))
            {
                return Account.fromJson(accountJson);
            }
            return null;
        }

        /// <summary>
        /// Delete persisted account
        /// </summary>
        public void DeletePersistedCredentials()
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            if (settings.Contains(ACCOUNT_SETTING))
            {
                settings.Remove(ACCOUNT_SETTING);
            }
        }

    }
}
