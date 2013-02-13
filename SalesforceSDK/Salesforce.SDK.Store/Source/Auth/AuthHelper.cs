using Salesforce.SDK.Adaptation;
using Salesforce.SDK.Source.Auth;
using System;
using Windows.Security.Authentication.Web;

namespace Salesforce.SDK.Auth
{
    public class AuthHelper : IAuthHelper
    {
        public void StartLoginFlow(LoginOptions loginOptions)
        {
            DoAuthFlow(loginOptions);
        }

        private async void DoAuthFlow(LoginOptions loginOptions)
        {
            Uri loginUri = new Uri(OAuth2.ComputeAuthorizationUrl(loginOptions));
            Uri callbackUri = new Uri(loginOptions.CallbackUrl);

            WebAuthenticationResult webAuthenticationResult = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, loginUri, callbackUri)   ;
            if (webAuthenticationResult.ResponseStatus == WebAuthenticationStatus.Success)
            {
                Uri responseUri = new Uri(webAuthenticationResult.ResponseData.ToString());
                AuthResponse authResponse = OAuth2.ParseFragment(responseUri.Fragment.Substring(1));
                PlatformAdapter.Resolve<IAuthHelper>().EndLoginFlow(loginOptions, authResponse);
            }
        }

        public void EndLoginFlow(LoginOptions loginOptions, AuthResponse authResponse)
        {
            AccountManager.CreateNewAccount(loginOptions, authResponse);
        }
    }
}
