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
