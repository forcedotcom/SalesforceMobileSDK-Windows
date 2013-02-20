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

namespace Salesforce.SDK.Auth
{
    /// <summary>
    /// object representing an authenticated user credentials
    /// </summary>
    public class Account
    {
        public string LoginUrl     { get; private set; }
        public string ClientId     { get; private set; }
        public string CallbackUrl  { get; private set; }
        public string[] Scopes     { get; private set; }
        public string InstanceUrl  { get; private set; }
        public string AccessToken  { get; set; }
        public string RefreshToken { get; private set; }

        /// <summary>
        /// Constructor for Account
        /// NB: the Account is not stored anywhere until we call PersistCredentials on the IAuthStorageHelper
        /// </summary>
        /// <param name="loginUrl"></param>
        /// <param name="clientId"></param>
        /// <param name="callbackUrl"></param>
        /// <param name="scopes"></param>
        /// <param name="instanceUrl"></param>
        /// <param name="accessToken"></param>
        /// <param name="refreshToken"></param>
        public Account(string loginUrl, string clientId, string callbackUrl, string[] scopes, string instanceUrl, string accessToken, string refreshToken)
        {
            LoginUrl = loginUrl;
            ClientId = clientId;
            CallbackUrl = callbackUrl;
            Scopes = scopes;
            InstanceUrl = instanceUrl;
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }

        /// <summary>
        /// Serialize Account object as a JSON string
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static string ToJson(Account account)
        {
            return JsonConvert.SerializeObject(account);
        }

        /// <summary>
        /// Deserialize Account from a JSON string
        /// </summary>
        /// <param name="accountJson"></param>
        /// <returns></returns>
        public static Account fromJson(string accountJson)
        {
            return JsonConvert.DeserializeObject<Account>(accountJson);
        }
    }

    /// <summary>
    /// Class providing (static) methods for creating/deleting or retrieving an Account
    /// </summary>
    public class AccountManager
    {
        /// <summary>
        /// Delete Account for currently authenticated user
        /// </summary>
        public static void DeleteAccount()
        {
            PlatformAdapter.Resolve<IAuthStorageHelper>().DeletePersistedCredentials();
        }

        /// <summary>
        /// Return Account for currently authenticated user
        /// </summary>
        /// <returns></returns>
        public static Account GetAccount()
        {
            return PlatformAdapter.Resolve<IAuthStorageHelper>().RetrievePersistedCredentials();
        }

        /// <summary>
        /// Create and persist Account for newly authenticated user
        /// </summary>
        /// <param name="loginOptions"></param>
        /// <param name="authResponse"></param>
        public static void CreateNewAccount(LoginOptions loginOptions, AuthResponse authResponse)
        {
            Account account = new Account(loginOptions.LoginUrl, loginOptions.ClientId, loginOptions.CallbackUrl, loginOptions.Scopes,
                authResponse.InstanceUrl, authResponse.AccessToken, authResponse.RefreshToken);
            PlatformAdapter.Resolve<IAuthStorageHelper>().PersistCredentials(account);
        }
    }
}
