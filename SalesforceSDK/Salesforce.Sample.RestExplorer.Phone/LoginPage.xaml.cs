using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Salesforce.SDK.Auth;

namespace Salesforce.Sample.RestExplorer.Phone
{
    public partial class LoginPage : PhoneApplicationPage
    {
        private const String LOGIN_URL = "https://test.salesforce.com";
        private const String CALLBACK_URL = "sfdc:///axm/detect/oauth/done";
        private const String CLIENT_ID = "3MVG92.uWdyphVj4bnolD7yuIpCQsNgddWtqRND3faxrv9uKnbj47H4RkwheHA2lKY4cBusvDVp0M6gdGE8hp";

        public LoginPage()
        {
            InitializeComponent();
            wvLogin.Source = new Uri(OAuth2.ComputeAuthorizationUrl(LOGIN_URL, CLIENT_ID, CALLBACK_URL, new String[] { "api" }));
            wvLogin.Navigating += OnNavigating;
        }

        private void OnNavigating(object sender, NavigatingEventArgs e)
        {
            if (e.Uri.ToString().StartsWith(CALLBACK_URL) && e.Uri.Fragment.Length > 0)
            {
                e.Cancel = true;
                AuthResponse ar = OAuth2.ParseFragment(e.Uri.Fragment.Substring(1));
            }
        }
    }

}