using Microsoft.Phone.Controls;
using Salesforce.SDK.Rest;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Salesforce.Sample.RestExplorer.Phone
{
    public partial class RestActionPage : PhoneApplicationPage
    {
        private RestAction _restAction;

        public RestActionPage()
        {
            InitializeComponent();
            btnAction.Click += OnActionClicked;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            IDictionary<String, String> qs = NavigationContext.QueryString;
            _restAction = (RestAction) Enum.Parse(typeof(RestAction), qs["rest_action"]);

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
                    Show(tbFields);
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
            }
        }

        private void Show(params TextBox[] tbs)
        {
            foreach (TextBox tb in tbs)
            {
                tb.Visibility = Visibility.Visible;
            }
        }

        private void OnActionClicked(object sender, RoutedEventArgs e)
        {
            ClientManager cm = new ClientManager(((App)Application.Current).LoginOptions);
            RestClient rc = cm.GetRestClient();
            if (rc != null)
            {
                RestRequest request = BuildRestRequest();
                AsyncRequestCallback callback = (response) => { tbResult.Text = response.AsString; };
                rc.SendAsync(request, (response) => Dispatcher.BeginInvoke(() => { callback(response); }));
            }
        }

        private RestRequest BuildRestRequest()
        {
            switch (_restAction)
            {
                case RestAction.VERSIONS:
                    return RestRequest.GetRequestForVersions();
                case RestAction.RESOURCES:
                    return RestRequest.GetRequestForResources(tbApiVersion.Text);
                case RestAction.DESCRIBE_GLOBAL:
                    return RestRequest.GetRequestForDescribeGlobal(tbApiVersion.Text);
                case RestAction.METADATA:
                    return RestRequest.GetRequestForMetadata(tbApiVersion.Text, tbObjectType.Text);
                case RestAction.DESCRIBE:
                    return RestRequest.GetRequestForDescribe(tbApiVersion.Text, tbObjectType.Text);
                case RestAction.CREATE:
                    return RestRequest.GetRequestForCreate(tbApiVersion.Text, tbObjectType.Text, ParseFieldMap(tbFields));
                case RestAction.RETRIEVE:
                    return RestRequest.GetRequestForRetrieve(tbApiVersion.Text, tbObjectType.Text, tbObjectId.Text, tbFields.Text.Split(','));
                case RestAction.UPSERT:
                    return RestRequest.GetRequestForUpsert(tbApiVersion.Text, tbObjectType.Text, tbExternalIdField.Text, tbExternalId.Text, ParseFieldMap(tbFields));
                case RestAction.UPDATE:
                    return RestRequest.GetRequestForUpdate(tbApiVersion.Text, tbObjectType.Text, tbObjectId.Text, ParseFieldMap(tbFields));
                case RestAction.DELETE:
                    return RestRequest.GetRequestForDelete(tbApiVersion.Text, tbObjectType.Text, tbObjectId.Text);
                case RestAction.QUERY:
                    return RestRequest.GetRequestForQuery(tbApiVersion.Text, tbSoql.Text);
                case RestAction.SEARCH:
                    return RestRequest.GetRequestForQuery(tbApiVersion.Text, tbSosl.Text);
            }

            return null;
        }

        private Dictionary<string, object> ParseFieldMap(TextBox tb)
        {
            return null;
        }

    }
}