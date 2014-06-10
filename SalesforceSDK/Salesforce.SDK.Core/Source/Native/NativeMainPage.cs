using Salesforce.SDK.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Salesforce.SDK.Native
{
    public class NativeMainPage : Page, ISalesforcePage
    {
        /// <summary>
        /// Helper method for handling logout.
        /// </summary>
        protected async void OnLogout()
        {
            await SalesforceApplication.GlobalClientManager.Logout();
            OnNavigatedTo(null);
        }

        /// <summary>
        /// When navigated to, we try to get a RestClient
        /// If we are not already authenticated, this will kick off the login flow
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SalesforceApplication.GlobalClientManager.GetRestClient();
        }
    }
}
