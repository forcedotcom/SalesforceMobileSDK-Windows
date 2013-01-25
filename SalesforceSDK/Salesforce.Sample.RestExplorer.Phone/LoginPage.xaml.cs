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
            wvLogin.Navigated += OnNavigated;
            wvLogin.NavigationFailed += OnError;
        }

        private void OnError(object sender, NavigationFailedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Navigation failed to: " + e.Uri + " because of " + e.Exception);
        }

        private void OnNavigating(object sender, NavigatingEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Navigating to: " + e.Uri);
        }

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Navigated to: " + e.Uri);
            if (e!= null && e.Content != null)
            {
                System.Diagnostics.Debug.WriteLine("Body: " + e.Content);
            }
        }
    }

}