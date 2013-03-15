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
using Microsoft.Phone.Controls;
using Salesforce.Sample.RestExplorer.Shared;
using Salesforce.SDK.Rest;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Salesforce.Sample.RestExplorer.Phone
{
    /// <summary>
    /// Starting page of the Rest Explorer Phone application
    /// </summary>
    public partial class MainPage : PhoneApplicationPage
    {
        ClientManager _clientManager;
        Button[] _buttons;

        /// <summary>
        /// Constructor
        /// </summary>
        public MainPage()
        {
            InitializeComponent();

            _clientManager = new ClientManager(Config.LoginOptions);
            _buttons = new Button[] { btnVersions, btnResources, btnDescribeGlobal, btnDescribe, btnMetadata, btnCreate, btnRetrieve, btnUpdate, btnUpsert, btnDelete, btnQuery, btnSearch, btnManual, btnLogout };

            foreach (Button button in _buttons)
            {
                button.Click += OnAnyButtonClicked;
            }
        }

        /// <summary>
        /// When one of the button is clicked, we go to the RestActionPage with the Rest action name as a parameter in the URI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            NavigationService.Navigate(new Uri("/Pages/RestActionPage.xaml?rest_action=" + restAction, UriKind.Relative));
        }

        /// <summary>
        /// Handler for logout
        /// </summary>
        protected async void OnLogout()
        {
            await _clientManager.Logout();
            OnNavigatedTo(null);
        }

        /// <summary>
        /// When navigated to, we try to get a RestClient
        /// If we are not already authenticated, this will kick off the login flow
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _clientManager.GetRestClient();
        }
    }
}