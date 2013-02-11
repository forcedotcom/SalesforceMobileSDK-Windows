using Microsoft.Phone.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Salesforce.SDK.Net;
using Salesforce.SDK.Rest;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace Salesforce.Sample.RestExplorer.Phone
{
    public partial class RestActionPage : PhoneApplicationPage
    {
        private RestAction _restAction;

        public RestActionPage()
        {
            InitializeComponent();
            ShowResults(null);
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

        private void OnActionClicked(Object sender, RoutedEventArgs e)
        {
            ClientManager cm = new ClientManager(Config.LoginOptions);
            RestClient rc = cm.GetRestClient();
            if (rc != null)
            {
                RestRequest request = BuildRestRequest();
                rc.SendAsync(request, (response) => Dispatcher.BeginInvoke(() => { ShowResponse(response); }));
            }
        }

        private void ShowResponse(RestResponse response)
        {
            ShowResults(new String[] { "<b>StatusCode:</b>" + response.StatusCode, "<b>Body:</b>\n" + response.PrettyBody });
        }

        private void ShowResults(String[] blocks)
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
            wbResult.NavigateToString(sb.ToString());
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
                    return RestRequest.GetRequestForCreate(tbApiVersion.Text, tbObjectType.Text, ParseFieldsValue());
                case RestAction.RETRIEVE:
                    return RestRequest.GetRequestForRetrieve(tbApiVersion.Text, tbObjectType.Text, tbObjectId.Text, ParseFieldListValue());
                case RestAction.UPSERT:
                    return RestRequest.GetRequestForUpsert(tbApiVersion.Text, tbObjectType.Text, tbExternalIdField.Text, tbExternalId.Text, ParseFieldsValue());
                case RestAction.UPDATE:
                    return RestRequest.GetRequestForUpdate(tbApiVersion.Text, tbObjectType.Text, tbObjectId.Text, ParseFieldsValue());
                case RestAction.DELETE:
                    return RestRequest.GetRequestForDelete(tbApiVersion.Text, tbObjectType.Text, tbObjectId.Text);
                case RestAction.QUERY:
                    return RestRequest.GetRequestForQuery(tbApiVersion.Text, tbSoql.Text);
                case RestAction.SEARCH:
                    return RestRequest.GetRequestForSearch(tbApiVersion.Text, tbSosl.Text);
                case RestAction.MANUAL:
                    return BuildManualRestReuqest();
                default:
                    throw new InvalidOperationException();
            }
        }

        private RestRequest BuildManualRestReuqest()
        {
            RestMethod method = RestMethod.GET;
            foreach (var child in spMethods.Children)
            {
                if (child.GetType() == typeof(RadioButton) && ((RadioButton)child).IsChecked == true)
                {
                    method = (RestMethod)Enum.Parse(typeof(RestMethod), ((RadioButton)child).Tag.ToString(), true);
                    break;
                }
            }
            return new RestRequest(method, tbRequestPath.Text, tbRequestBody.Text, ContentType.JSON);
        }

        private string[] ParseFieldListValue()
        {
            return tbFieldList.Text.Split(',');
        }

        private Dictionary<String, Object> ParseFieldsValue()
        {
            Dictionary<String, Object> result = new Dictionary<String, Object>();
            JObject fieldMap = JObject.Parse(tbFields.Text);
            foreach (var item in fieldMap)
            {
                result.Add(item.Key, item.Value);
            }
            return result;
        }

    }
}