using Microsoft.Phone.Controls;
using Salesforce.SDK.Rest;
using Salesforce.SDK.Source.Auth;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Salesforce.Sample.RestExplorer.Phone
{
    public partial class MainPage : PhoneApplicationPage
    {
        private ClientManager _clientManager;
        private Button[] _buttons;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            _clientManager = new ClientManager(((App)Application.Current).LoginOptions);
            _buttons = new Button[] { btnVersions, btnResources, btnDescribeGlobal, btnDescribe, btnMetadata, btnCreate, btnRetrieve, btnUpdate, btnUpsert, btnDelete, btnQuery, btnSearch, btnManual, btnLogout };

            foreach (Button button in _buttons)
            {
                button.Click += OnAnyButtonClicked;
            }
        }

        private void OnAnyButtonClicked(object sender, RoutedEventArgs e)
        {
            RestAction restAction = RestAction.RESOURCES;
            
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
            NavigationService.Navigate(new Uri("/Pages/RestActionPage.xaml?rest_action=" + restAction, UriKind.Relative));
        }

        protected void OnLogout()
        {
            _clientManager.Logout();
            OnNavigatedTo(null);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _clientManager.GetRestClient();
        }
    }
}