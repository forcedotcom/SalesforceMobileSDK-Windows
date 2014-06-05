using Salesforce.SDK.App;
using Salesforce.SDK.Auth;
using Salesforce.SDK.Rest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Salesforce.One.App
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// When navigated to, we try to get a RestClient
        /// If we are not already authenticated, this will kick off the login flow
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            RestClient client = SalesforceApplication.GlobalClientManager.GetRestClient();
            if (client != null)
            {
                Account account = AccountManager.GetAccount();
                String startPage = OAuth2.ComputeFrontDoorUrl(account.InstanceUrl, account.AccessToken, account.LoginUrl + "/one/one.app");
                oneView.Navigate(new Uri(startPage));
            }
        }

        private void SwitchAccount(object sender, RoutedEventArgs e)
        {
            AccountManager.SwitchAccount();
        }
    }
}
