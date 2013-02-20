using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Salesforce.Sample.VFConnector.Phone.Resources;
using Salesforce.SDK.Source.Config;
using Salesforce.SDK.Auth;
using Salesforce.SDK.Rest;

namespace Salesforce.Sample.VFConnector.Phone
{
    public partial class MainPage : PhoneApplicationPage
    {
        private BootConfig _bootConfig;
        private LoginOptions _loginOptions;
        private ClientManager _clientManager;
        private RestClient _client;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            _bootConfig = BootConfig.GetBootConfig();
            _loginOptions = new LoginOptions("https://test.salesforce.com", _bootConfig.ClientId, _bootConfig.CallbackURL, _bootConfig.Scopes);
            _clientManager = new ClientManager(_loginOptions);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _client = _clientManager.GetRestClient();
            if (_client != null)
            {
                PGView.StartPageUri = new Uri(GetFrontDoorUrl(_bootConfig.StartPage), UriKind.Absolute);
            }
        }

        // TODO move to SDK
        private const string FRONT_DOOR_PATH = "/secur/frontdoor.jsp";
        private const string FRONT_DOOR_QUERY_STRING = "sid={0}&retURL={1}&display=touch";
        public string GetFrontDoorUrl(String url) 
        {
            // Args
            string[] args = {_client.AccessToken, url};
            string[] urlEncodedArgs = args.Select(s => Uri.EscapeUriString(s)).ToArray();

            // Authorization url
            string frontDoorUrl = string.Format(_client.InstanceUrl + FRONT_DOOR_PATH + "?" + FRONT_DOOR_QUERY_STRING, urlEncodedArgs);

            return frontDoorUrl;
        }
    }
}