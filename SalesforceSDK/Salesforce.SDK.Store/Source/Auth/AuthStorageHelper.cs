using Salesforce.SDK.Source.Auth;
using System;
using Windows.Storage;

namespace Salesforce.SDK.Auth
{
    public class AuthStorageHelper : IAuthStorageHelper
    {
        private const String ACCOUNT_SETTING = "account";

        public void PersistCredentials(Account account)
        {
            ApplicationDataContainer settings = ApplicationData.Current.RoamingSettings;
            String accountJson = Account.ToJson(account);
            settings.Values[ACCOUNT_SETTING] = accountJson;
        }

        public Account RetrievePersistedCredentials()
        {
            ApplicationDataContainer settings = ApplicationData.Current.RoamingSettings;
            String accountJson = settings.Values[ACCOUNT_SETTING] as String;
            return accountJson == null ? null : Account.fromJson(accountJson);
        }

        public void DeletePersistedCredentials()
        {
            ApplicationDataContainer settings = ApplicationData.Current.RoamingSettings;
            settings.Values.Remove(ACCOUNT_SETTING);
        }
    }
}
