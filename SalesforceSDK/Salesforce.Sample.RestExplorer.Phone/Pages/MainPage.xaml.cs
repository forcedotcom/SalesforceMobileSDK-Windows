using Salesforce.Sample.RestExplorer.Shared;
using Salesforce.SDK.App;
using Salesforce.SDK.Hybrid;
using Salesforce.SDK.Rest;
using Salesforce.SDK.Source.Settings;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace Salesforce.Sample.RestExplorer.Phone
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Button[] _buttons;

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
            _buttons = new Button[] { btnVersions, btnResources, btnDescribeGlobal, btnDescribe, btnMetadata, btnCreate, btnRetrieve, btnUpdate, btnUpsert, btnDelete, btnQuery, btnSearch, btnManual, btnLogout };

            foreach (Button button in _buttons)
            {
                button.Click += OnAnyButtonClicked;
            }
        }

        /// <summary>
        /// When one of the button is clicked, we go to the RestActionPage with the Rest action name as a parameter in the URI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAnyButtonClicked(object sender, RoutedEventArgs e)
        {
            RestAction restAction = RestAction.VERSIONS;

            switch (((Button)sender).Name)
            {
                case "btnLogout": OnLogout(); return;
                case "btnManual": restAction = RestAction.MANUAL; break;
                case "btnCreate": restAction = RestAction.CREATE; break;
                case "btnDelete": restAction = RestAction.DELETE; break;
                case "btnDescribe": restAction = RestAction.DESCRIBE; break;
                case "btnDescribeGlobal": restAction = RestAction.DESCRIBE_GLOBAL; break;
                case "btnMetadata": restAction = RestAction.METADATA; break;
                case "btnQuery": restAction = RestAction.QUERY; break;
                case "btnResources": restAction = RestAction.RESOURCES; break;
                case "btnRetrieve": restAction = RestAction.RETRIEVE; break;
                case "btnSearch": restAction = RestAction.SEARCH; break;
                case "btnUpdate": restAction = RestAction.UPDATE; break;
                case "btnUpsert": restAction = RestAction.UPSERT; break;
                case "btnVersions": restAction = RestAction.VERSIONS; break;
            }
            Frame.Navigate(typeof(RestActionPage), restAction);
        }

        /// <summary>
        /// Handler for logout
        /// </summary>
        protected async void OnLogout()
        {
            await SalesforceApplication.GlobalClientManager.Logout();
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
