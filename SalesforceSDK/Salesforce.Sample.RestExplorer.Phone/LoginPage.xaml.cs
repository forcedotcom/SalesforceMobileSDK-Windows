using Microsoft.Phone.Controls;
using Salesforce.SDK.Adaptation;
using Salesforce.SDK.Auth;
using Salesforce.SDK.Rest;
using System;
using System.Collections.Generic;
using System.Windows.Navigation;

namespace Salesforce.SDK.Auth
{
    public partial class LoginPage : PhoneApplicationPage
    {
        private LoginOptions _loginOptions;

        public LoginPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            IDictionary<String,String> qs = NavigationContext.QueryString;
            _loginOptions = new LoginOptions(qs[AuthHelper.LOGIN_SERVER], qs[AuthHelper.CLIENT_ID], qs[AuthHelper.CALLBACK_URL], qs[AuthHelper.SCOPES].Split(' '));
            wvLogin.Source = new Uri(OAuth2.ComputeAuthorizationUrl(_loginOptions));
            wvLogin.Navigating += OnNavigating;
        }

        private void OnNavigating(object sender, NavigatingEventArgs e)
        {
            if (e.Uri.ToString().StartsWith(_loginOptions.CallbackUrl) && e.Uri.Fragment.Length > 0)
            {
                e.Cancel = true;
                AuthResponse authResponse = OAuth2.ParseFragment(e.Uri.Fragment.Substring(1));
                PlatformAdapter.Resolve<IAuthHelper>().EndLoginFlow(_loginOptions, authResponse);
            }
        }
    }

}