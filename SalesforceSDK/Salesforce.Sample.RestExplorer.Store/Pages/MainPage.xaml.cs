using Salesforce.Sample.RestExplorer.Shared;
using Salesforce.Sample.RestExplorer.ViewModels;
using Salesforce.SDK.Rest;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Salesforce.Sample.RestExplorer.Store
{
    public sealed partial class MainPage : Page
    {
        RestActionViewModel _viewModel;
        ClientManager _clientManager;
        Button[] _buttons;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            _viewModel = DataContext as RestActionViewModel;
            _viewModel.PropertyChanged += OnViewModelPropertyChanged;
            _viewModel.SyncContext = SynchronizationContext.Current;

            _clientManager = new ClientManager(Config.LoginOptions);
            _buttons = new Button[] { btnVersions, btnResources, btnDescribeGlobal, btnDescribe, btnMetadata, btnCreate, btnRetrieve, btnUpdate, btnUpsert, btnDelete, btnQuery, btnSearch, btnManual, btnLogout };

            foreach (Button button in _buttons)
            {
                button.Click += OnAnyButtonClicked;
            }

            SwitchToRestAction(RestAction.VERSIONS);
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == RestActionViewModel.RETURNED_REST_RESPONSE)
            {
                // Data binding would be more elegant
                ShowResponse(_viewModel.ReturnedRestResponse);
            }
        }


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
            SwitchToRestAction(restAction);
        }

        private void SwitchToRestAction(RestAction restAction)
        {
            ShowResponse(null);
            String restActionStr = restAction.ToString();
            _viewModel[RestActionViewModel.SELECTED_REST_ACTION] = restActionStr;

            HashSet<String> names = RestActionViewHelper.GetNamesOfControlsToShow(restActionStr);
            foreach (TextBox tb in new TextBox[] {tbApiVersion, tbObjectType, 
                tbObjectId, tbExternalIdField, tbExternalId, tbFieldList, tbFields, 
                tbSoql, tbSosl, tbRequestPath, tbRequestBody, tbRequestMethod})
            {
                tb.Visibility = names.Contains(tb.Name) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void OnLogout()
        {
            _clientManager.Logout();
            OnNavigatedTo(null);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _clientManager.GetRestClient();
        }

        private void ShowResponse(RestResponse response)
        {
            wbResult.NavigateToString(RestActionViewHelper.BuildHtml(response));
        }
    }
}
