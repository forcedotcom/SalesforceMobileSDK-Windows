using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Foundation;
using System;
using Windows.Networking.Sockets;
using Salesforce.SDK.Auth;
using Salesforce.SDK.Source.Security;
using Salesforce.SDK.Source.Settings;

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

        public static void InitEncryption()
        {
            Encryptor.init(new EncryptionSettings(new HmacSHA256KeyGenerator()));
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

        public static IAsyncOperation<Account> CreateNewAccount(LoginOptions loginOptions, string response)
        {
            SDK.Auth.AuthResponse authResponse = SDK.Auth.OAuth2.ParseFragment(response);
            return Task.Run(async () =>
            {
                var account = await SDK.Auth.AccountManager.CreateNewAccount(loginOptions.ConvertToSDKLoginOptions(), authResponse);
                return Account.FromJson(SDK.Auth.Account.ToJson(account));
            }).AsAsyncOperation<Account>();
        }

       
    }
}
