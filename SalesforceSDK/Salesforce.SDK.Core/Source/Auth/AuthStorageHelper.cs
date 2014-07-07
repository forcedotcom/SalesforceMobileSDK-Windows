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
using Windows.Security.Credentials;
using Windows.Storage;
using System.Linq;
using System.Diagnostics;

namespace Salesforce.SDK.Auth
{
    /// <summary>
    /// Store specific implementation if IAuthStorageHelper
    /// </summary>    /// </summary>
    public sealed class AuthStorageHelper
    {
        private const string PersistedDataStorage = "persistedDataStorage";
        private const string PasswordVaultAccounts = "Salesforce Accounts";
        private const string PasswordVaultCurrentAccount = "Salesforce Account";
        private const string PasswordVaultSecuredData = "Salesforce Secure";
        private const string PasswordVaultPincode = "Salesforce Pincode";
        private const string InstallationStatusKey = "InstallationStatus";

        private PasswordVault Vault;
        private ApplicationDataContainer PersistedData;
        private static readonly Lazy<AuthStorageHelper> Auth = new Lazy<AuthStorageHelper>(() => new AuthStorageHelper());

        public static AuthStorageHelper GetAuthStorageHelper()
        {
            return Auth.Value;
        }

        private AuthStorageHelper()
        {
            Vault = new PasswordVault();
            PersistedData = ApplicationData.Current.LocalSettings;
            InstallationStatusCheck();
        }

        private void InstallationStatusCheck()
        {
            if (!PersistedData.Values.ContainsKey(InstallationStatusKey))
            {
                var accounts = Vault.RetrieveAll();
                foreach (var next in accounts)
                {
                    Vault.Remove(next);
                }
                PersistedData.Values.Add(InstallationStatusKey, "");
            }
        }

        private IReadOnlyList<PasswordCredential> SafeRetrieveResource(string resource)
        {
            try
            {
                return Vault.FindAllByResource(resource);
            } catch (Exception)
            {
                Debug.WriteLine("Failed to retrieve vault data for resource " + resource);
            }
            return new List<PasswordCredential>();
        }

        private PasswordCredential SafeRetrieveUser(string resource, string userName)
        {
            try
            {
                return Vault.Retrieve(resource, userName);
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to retrieve vault data for resource " + resource);
            }
            return null;
        }

        private IReadOnlyList<PasswordCredential> SafeRetrieveUser(string userName)
        {
            try
            {
                return Vault.FindAllByUserName(userName);
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to retrieve vault data for user");
            }
            return new List<PasswordCredential>();
        }

        /// <summary>
        /// Persist account, and sets account as the current account.
        /// </summary>
        /// <param name="account"></param>
        internal void PersistCredentials(Account account)
        {
            var creds = SafeRetrieveUser(PasswordVaultAccounts, account.UserName);
            if (creds != null)
            {
                Vault.Remove(creds);
                var current = Vault.FindAllByResource(PasswordVaultCurrentAccount);
                if (current != null)
                {
                    foreach (var user in current)
                    {
                        Vault.Remove(user);
                    }
                }
            }
            string serialized = Encryptor.Encrypt(JsonConvert.SerializeObject(account));
            Vault.Add(new PasswordCredential(PasswordVaultAccounts, account.UserName, serialized));
            Vault.Add(new PasswordCredential(PasswordVaultCurrentAccount, account.UserName, serialized));
            Dictionary<string, Account> accounts = RetrievePersistedCredentials();
            if (accounts.ContainsKey(account.UserId))
            {
                accounts[account.UserId] = account;
            }
            else
            {
                accounts.Add(account.UserId, account);
            }
            LoginOptions options = new LoginOptions(account.LoginUrl, account.ClientId, account.CallbackUrl, account.Scopes);
            SalesforceConfig.LoginOptions = options;
        }

        internal Account RetrieveCurrentAccount()
        {
            var creds = SafeRetrieveResource(PasswordVaultCurrentAccount).FirstOrDefault();
            if (creds != null)
            {
                var account = Vault.Retrieve(creds.Resource, creds.UserName);
                if (String.IsNullOrWhiteSpace(account.Password))
                    Vault.Remove(creds);
                else
                    return JsonConvert.DeserializeObject<Account>(Encryptor.Decrypt(account.Password));
            }
            return null;
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
            var creds = SafeRetrieveResource(PasswordVaultAccounts);
            Dictionary<string, Account> accounts = new Dictionary<string, Account>();
            if (creds != null)
            {
                foreach (var next in creds)
                {
                    var account = Vault.Retrieve(next.Resource, next.UserName);
                    if (String.IsNullOrWhiteSpace(account.Password))
                        Vault.Remove(next);
                    else
                        accounts.Add(next.UserName, JsonConvert.DeserializeObject<Account>(Encryptor.Decrypt(account.Password)));
                }
            }
            return accounts;
        }

        /// <summary>
        /// Delete a persisted account credential based on the user id.
        /// </summary>
        /// <param name="id"></param>
        internal void DeletePersistedCredentials(string userName, string id)
        {
            var creds = SafeRetrieveUser(userName);
            if (creds != null)
            {
                foreach (var next in creds)
                {
                    var vaultAccount = Vault.Retrieve(next.Resource, next.UserName);
                    Account account = JsonConvert.DeserializeObject<Account>(Encryptor.Decrypt(vaultAccount.Password));
                    if (id.Equals(account.UserId))
                    {
                        Vault.Remove(next);
                    }
                }
            }
        }
        /// <summary>
        /// Delete all persisted accounts
        /// </summary>
        internal void DeletePersistedCredentials()
        {
            var accounts = SafeRetrieveResource(PasswordVaultAccounts);
            var current = SafeRetrieveResource(PasswordVaultCurrentAccount);
            if (accounts != null)
            {
                foreach (var next in accounts)
                {
                    Vault.Remove(next);
                }
            }
            if (current != null)
            {
                foreach (var next in current)
                {
                    Vault.Remove(next);
                }
            }
        }

        internal void PersistPincode(MobilePolicy policy)
        {
            DeletePincode();
            PasswordCredential newPin = new PasswordCredential(PasswordVaultSecuredData, PasswordVaultPincode, JsonConvert.SerializeObject(policy));
            Vault.Add(newPin);
        }

        internal string RetrievePincode()
        {
            var pin = SafeRetrieveUser(PasswordVaultSecuredData, PasswordVaultPincode);
            if (pin != null)
                return pin.Password;
            return null;
        }

        internal void DeletePincode()
        {
            var pin = SafeRetrieveUser(PasswordVaultSecuredData, PasswordVaultPincode);
            if (pin != null)
            {
                Vault.Remove(pin);
            }
        }

        internal void PersistData(bool replace, string key, string data, string nonce = null)
        {
            if (PersistedData.Values.ContainsKey(key))
            {
                if (replace)
                {
                    PersistedData.Values[key] = Encryptor.Encrypt(data, nonce);
                }
            }
            else
            {
                PersistedData.Values.Add(key, Encryptor.Encrypt(data));
            }
        }

        internal string RetrieveData(string key, string nonce = null)
        {
            string data = null;
            if (PersistedData.Values.ContainsKey(key))
            {
                data = Encryptor.Decrypt(PersistedData.Values[key] as string, nonce);
            }
            return data;
        }

        internal void DeleteData(string key)
        {
            if (PersistedData.Values.ContainsKey(key))
            {
                PersistedData.Values.Remove(key);
            }
        }
    }
}
