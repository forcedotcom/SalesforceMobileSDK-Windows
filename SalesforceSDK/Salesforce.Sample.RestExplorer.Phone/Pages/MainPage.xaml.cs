using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Salesforce.SDK.App;
using Salesforce.SDK.Auth;
using Salesforce.SDK.Rest;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace Salesforce.Sample.RestExplorer.Phone
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();

            NavigationCacheMode = NavigationCacheMode.Required;
            Button[] buttons =
            {
                btnSwitch, btnVersions, btnResources, btnDescribeGlobal, btnDescribe, btnMetadata,
                btnCreate, btnRetrieve, btnUpdate, btnUpsert, btnDelete, btnQuery, btnSearch, btnManual, btnLogout
            };

            foreach (Button button in buttons)
            {
                button.Click += OnAnyButtonClicked;
            }
        }

        /// <summary>
        ///     When one of the button is clicked, we go to the RestActionPage with the Rest action name as a parameter in the URI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAnyButtonClicked(object sender, RoutedEventArgs e)
        {
            var restAction = RestAction.VERSIONS;

            switch (((Button) sender).Name)
            {
                case "btnLogout":
                    OnLogout();
                    return;
                case "btnSwitch":
                    OnSwitch();
                    return;
                case "btnManual":
                    restAction = RestAction.MANUAL;
                    break;
                case "btnCreate":
                    restAction = RestAction.CREATE;
                    break;
                case "btnDelete":
                    restAction = RestAction.DELETE;
                    break;
                case "btnDescribe":
                    restAction = RestAction.DESCRIBE;
                    break;
                case "btnDescribeGlobal":
                    restAction = RestAction.DESCRIBE_GLOBAL;
                    break;
                case "btnMetadata":
                    restAction = RestAction.METADATA;
                    break;
                case "btnQuery":
                    restAction = RestAction.QUERY;
                    break;
                case "btnResources":
                    restAction = RestAction.RESOURCES;
                    break;
                case "btnRetrieve":
                    restAction = RestAction.RETRIEVE;
                    break;
                case "btnSearch":
                    restAction = RestAction.SEARCH;
                    break;
                case "btnUpdate":
                    restAction = RestAction.UPDATE;
                    break;
                case "btnUpsert":
                    restAction = RestAction.UPSERT;
                    break;
                case "btnVersions":
                    restAction = RestAction.VERSIONS;
                    break;
            }
            Frame.Navigate(typeof (RestActionPage), restAction);
        }

        /// <summary>
        ///     Handler for logout
        /// </summary>
        private async void OnLogout()
        {
            await SalesforceApplication.GlobalClientManager.Logout();
        }

        private void OnSwitch()
        {
            AccountManager.SwitchAccount();
        }

        /// <summary>
        ///     When navigated to, we try to get a RestClient
        ///     If we are not already authenticated, this will kick off the login flow
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SalesforceApplication.GlobalClientManager.GetRestClient();
        }
    }
}