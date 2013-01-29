using Microsoft.Phone.Controls;
using Salesforce.SDK.Auth;
using Salesforce.SDK.Rest;
using Salesforce.SDK.Source.Auth;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.Storage;

namespace Salesforce.SDK.Auth
{
    public class AuthHelper : IAuthHelper
    {
        private const String ACCOUNT_SETTING = "account";

        public const String LOGIN_SERVER = "loginServer";
        public const String CLIENT_ID = "clientId";
        public const String CALLBACK_URL = "callbackUrl";
        public const String SCOPES = "scopes";

        public void StartLoginFlow(LoginOptions loginOptions)
        {
            String loginUrl = Uri.EscapeUriString(loginOptions.LoginUrl);
            String clientId = Uri.EscapeUriString(loginOptions.ClientId);
            String callbackUrl = Uri.EscapeUriString(loginOptions.CallbackUrl);
            String scopes = Uri.EscapeUriString(String.Join(" ", loginOptions.Scopes));
            String queryString = String.Format("?{0}={1}&{2}={3}&{4}={5}&{6}={7}", LOGIN_SERVER, loginUrl, CLIENT_ID, clientId, CALLBACK_URL, callbackUrl, SCOPES, scopes);

            ((PhoneApplicationFrame) Application.Current.RootVisual).Navigate(new Uri("/LoginPage.xaml" + queryString, UriKind.Relative));
        }

        public void EndLoginFlow(LoginOptions loginOptions, AuthResponse authResponse)
        {
            AccountManager.CreateNewAccount(loginOptions, authResponse);
            ((PhoneApplicationFrame) Application.Current.RootVisual).GoBack();
        }

        public void PersistCredentials(Account account)
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            String accountJson = Account.ToJson(account);
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

        public Account RetrievePersistedCredentials()
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            String accountJson;
            if (settings.TryGetValue<String>(ACCOUNT_SETTING, out accountJson))
            {
                return Account.fromJson(accountJson);
            }
            return null;
        }

    }
}
