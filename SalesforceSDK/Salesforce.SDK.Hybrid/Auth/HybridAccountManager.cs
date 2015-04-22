using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Foundation;
using System;

namespace Salesforce.SDK.Hybrid.Auth
{
    public sealed class HybridAccountManager
    {
        private static HybridAccountManager _instance = new HybridAccountManager();

        public static HybridAccountManager GetInstance()
        {
            return _instance;
        }

        public static void DeleteAccount()
        {
            SDK.Auth.AccountManager.DeleteAccount();
        }

        public static IDictionary<string, Account> GetAccounts()
        {
            Dictionary<string, Account> accounts = new Dictionary<string, Account>();
            var acMgrAccounts = SDK.Auth.AccountManager.GetAccounts();
            foreach (var key in acMgrAccounts.Keys)
            {
                accounts[key] = Account.FromJson(SDK.Auth.Account.ToJson(acMgrAccounts[key]));
            }
            return accounts;
        }

        public static Account GetAccount()
        {
            var account = SDK.Auth.AccountManager.GetAccount();
            return Account.FromJson(SDK.Auth.Account.ToJson(account));
        }

        public static void WipeAccounts()
        {
            SDK.Auth.AccountManager.WipeAccounts();
        }

        public static IAsyncOperation<Account> CreateNewAccount(LoginOptions loginOptions, AuthResponse authResponse)
        {
            return Task<Account>.Run(async () =>
            {
                var account = await SDK.Auth.AccountManager.CreateNewAccount(loginOptions.ConvertToSDKLoginOptions(), authResponse.ConvertToSDKResponse());
                return Account.FromJson(SDK.Auth.Account.ToJson(account));
            }).AsAsyncOperation<Account>();
        }
    }
}
