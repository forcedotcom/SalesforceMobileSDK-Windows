using Salesforce.SDK.Adaptation;
using Salesforce.SDK.Auth;
using Salesforce.SDK.Source.Auth;
using System;

namespace Salesforce.SDK.Rest
{
    public class ClientManager
    {
        private LoginOptions _loginOptions;

        public ClientManager(LoginOptions loginOptions)
        {
            _loginOptions = loginOptions;
        }

        public async void Logout()
        {
            Account account = AccountManager.GetAccount();
            if (account != null)
            {
                AccountManager.DeleteAccount();
                await OAuth2.RevokeAuthToken(_loginOptions, account.RefreshToken);
            }
        }

        public RestClient GetRestClient()
        {
            Account account = AccountManager.GetAccount();
            if (account != null)
            {
                return new RestClient(account.InstanceUrl, account.AccessToken,
                                        () => OAuth2.RefreshAuthToken(_loginOptions, account.RefreshToken).ContinueWith(
                                            authResponse =>
                                            {
                                                account.AccessToken = authResponse.Result.AccessToken;
                                                return account.AccessToken;
                                            })
                                       );
            }
            else
            {
                PlatformAdapter.Resolve<IAuthHelper>().StartLoginFlow(_loginOptions);
                return null;
            }
        }
    }
}
