using Microsoft.Phone.Controls;
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
        private RestAction _restAction;

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
            IDictionary<String, String> qs = NavigationContext.QueryString;
            _restAction = (RestAction)Enum.Parse(typeof(RestAction), qs["rest_action"]);
            _viewModel.SelectedRestAction = _restAction;

            SetupButtons();
        }

        private void SetupButtons()
        {
            switch (_restAction)
            {
                case RestAction.VERSIONS:
                    break;
                case RestAction.RESOURCES:
                    Show(tbApiVersion);
                    break;
                case RestAction.DESCRIBE_GLOBAL:
                    Show(tbApiVersion);
                    break;
                case RestAction.METADATA:
                    Show(tbApiVersion);
                    Show(tbObjectType);
                    break;
                case RestAction.DESCRIBE:
                    Show(tbApiVersion);
                    Show(tbObjectType);
                    break;
                case RestAction.CREATE:
                    Show(tbApiVersion);
                    Show(tbObjectType);
                    Show(tbFields);
                    break;
                case RestAction.RETRIEVE:
                    Show(tbApiVersion);
                    Show(tbObjectType);
                    Show(tbObjectId);
                    Show(tbFieldList);
                    break;
                case RestAction.UPSERT:
                    Show(tbApiVersion);
                    Show(tbObjectType);
                    Show(tbExternalIdField);
                    Show(tbExternalId);
                    Show(tbFields);
                    break;
                case RestAction.UPDATE:
                    Show(tbApiVersion);
                    Show(tbObjectType);
                    Show(tbObjectId);
                    Show(tbFields);
                    break;
                case RestAction.DELETE:
                    Show(tbApiVersion);
                    Show(tbObjectType);
                    Show(tbObjectId);
                    break;
                case RestAction.QUERY:
                    Show(tbApiVersion);
                    Show(tbSoql);
                    break;
                case RestAction.SEARCH:
                    Show(tbApiVersion);
                    Show(tbSosl);
                    break;
                case RestAction.MANUAL:
                    Show(tbRequestPath);
                    Show(tbRequestBody);
                    Show(svMethods);
                    break;
            }
        }

        private void Show(params Control[] controls)
        {
            foreach (Control control in controls)
            {
                control.Visibility = Visibility.Visible;
            }
        }


        private void ShowResponse(RestResponse response)
        {
            wbResult.NavigateToString(BuildHtml(response == null 
                ? null 
                : new String[] { "<b>Status Code:</b>" + response.StatusCode, "<b>Body:</b>\n" + response.PrettyBody}));
        }

        private string BuildHtml(String[] blocks)
        {
            String htmlHead = @"
            <head>
                <meta name=""viewport"" content=""width=device-width, initial-scale=1.0, maximum-scale=1.0; user-scalable=no"" />
                <style>
                    body { background-color: black; color: white; }
                    pre {border: 1px solid white; padding: 5px; word-wrap: break-word;}
                </style>
            </head>
            ";

            StringBuilder sb = new StringBuilder(htmlHead);

            sb.Append("<body>");

            if (blocks != null)
            {
                foreach (String block in blocks)
                {
                    sb.Append("<pre>").Append(block).Append("</pre>");
                }
            }
            sb.Append("</body>");

            return sb.ToString();
        }
    }
}