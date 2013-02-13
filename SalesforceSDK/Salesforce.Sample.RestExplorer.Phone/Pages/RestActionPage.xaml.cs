using Microsoft.Phone.Controls;
using Salesforce.Sample.RestExplorer.Shared;
using Salesforce.Sample.RestExplorer.ViewModels;
using Salesforce.SDK.Rest;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Salesforce.Sample.RestExplorer.Phone
{
    public partial class RestActionPage : PhoneApplicationPage
    {
        private RestActionViewModel _viewModel;

        public RestActionPage()
        {
            InitializeComponent();
            ShowResponse(null);
            _viewModel = DataContext as RestActionViewModel;
            _viewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == RestActionViewModel.RETURNED_REST_RESPONSE)
            {
                // Data binding would be more elegant
                Dispatcher.BeginInvoke(() => { ShowResponse(_viewModel.ReturnedRestResponse); });
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            String restActionStr = NavigationContext.QueryString["rest_action"];
            _viewModel[RestActionViewModel.SELECTED_REST_ACTION] = restActionStr;

            HashSet<String> names = RestActionViewHelper.GetNamesOfControlsToShow(restActionStr);
            foreach (TextBox tb in new TextBox[] {tbApiVersion, tbObjectType, 
                tbObjectId, tbExternalIdField, tbExternalId, tbFieldList, tbFields, 
                tbSoql, tbSosl, tbRequestPath, tbRequestBody, tbRequestMethod})
            {
                tb.Visibility = names.Contains(tb.Name) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void ShowResponse(RestResponse response)
        {
            wbResult.NavigateToString(RestActionViewHelper.BuildHtml(response));
        }
    }
}