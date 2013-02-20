using Microsoft.Phone.Controls;
using Salesforce.SDK.Auth;
using Salesforce.SDK.Rest;
using Salesforce.SDK.Source.Config;
/*
 * Copyright (c) 2013, salesforce.com, inc.
 * All rights reserved.
 * Redistribution and use of this software in source and binary forms, with or
 * without modification, are permitted provided that the following conditions
 * are met:
 * - Redistributions of source code must retain the above copyright notice, this
 * list of conditions and the following disclaimer.
 * - Redistributions in binary form must reproduce the above copyright notice,
 * this list of conditions and the following disclaimer in the documentation
 * and/or other materials provided with the distribution.
 * - Neither the name of salesforce.com, inc. nor the names of its contributors
 * may be used to endorse or promote products derived from this software without
 * specific prior written permission of salesforce.com, inc.
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 */
using System;
using System.Linq;
using System.Windows.Navigation;

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