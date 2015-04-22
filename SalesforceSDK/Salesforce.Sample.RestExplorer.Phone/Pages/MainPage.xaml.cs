using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
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
            var restAction = RestAction.Versions;

            switch (((Button) sender).Name)
            {
                case "btnLogout":
                    OnLogout();
                    return;
                case "btnSwitch":
                    OnSwitch();
                    return;
                case "btnManual":
                    restAction = RestAction.Manual;
                    break;
                case "btnCreate":
                    restAction = RestAction.Create;
                    break;
                case "btnDelete":
                    restAction = RestAction.Delete;
                    break;
                case "btnDescribe":
                    restAction = RestAction.Describe;
                    break;
                case "btnDescribeGlobal":
                    restAction = RestAction.DescribeGlobal;
                    break;
                case "btnMetadata":
                    restAction = RestAction.Metadata;
                    break;
                case "btnQuery":
                    restAction = RestAction.Query;
                    break;
                case "btnResources":
                    restAction = RestAction.Resources;
                    break;
                case "btnRetrieve":
                    restAction = RestAction.Retrieve;
                    break;
                case "btnSearch":
                    restAction = RestAction.Search;
                    break;
                case "btnUpdate":
                    restAction = RestAction.Update;
                    break;
                case "btnUpsert":
                    restAction = RestAction.Upsert;
                    break;
                case "btnVersions":
                    restAction = RestAction.Versions;
                    break;
            }
            Frame.Navigate(typeof (RestActionPage), restAction);
        }

        /// <summary>
        ///     Handler for logout
        /// </summary>
        private async void OnLogout()
        {
            await SDKManager.GlobalClientManager.Logout();
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
            SDKManager.GlobalClientManager.GetRestClient();
        }
    }
}