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
    public class AuthStorageHelper : IAuthStorageHelper
    {
        private const String ACCOUNT_SETTING = "account";

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
