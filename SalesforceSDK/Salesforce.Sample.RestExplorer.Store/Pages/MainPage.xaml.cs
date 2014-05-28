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
using Salesforce.Sample.RestExplorer.Shared;
using Salesforce.Sample.RestExplorer.ViewModels;
using Salesforce.SDK.App;
using Salesforce.SDK.Rest;
using Salesforce.SDK.Native;
using Salesforce.SDK.Source.Settings;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Salesforce.Sample.RestExplorer.Store
{
    /// <summary>
    /// Only page of the Rest Explorer Store application
    /// </summary>
    public sealed partial class MainPage : NativeMainPage
    {
        RestActionViewModel _viewModel;
        Button[] _buttons;

        /// <summary>
        /// Constructor
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
            _viewModel = DataContext as RestActionViewModel;
            _viewModel.PropertyChanged += OnViewModelPropertyChanged;
            _viewModel.SyncContext = SynchronizationContext.Current;
            _buttons = new Button[] { btnVersions, btnResources, btnDescribeGlobal, btnDescribe, btnMetadata, btnCreate, btnRetrieve, btnUpdate, btnUpsert, btnDelete, btnQuery, btnSearch, btnManual, btnLogout };

            foreach (Button button in _buttons)
            {
                button.Click += OnAnyButtonClicked;
            }

            SwitchToRestAction(RestAction.VERSIONS);
        }

        /// <summary>
        /// Watching the RETURNED_REST_RESPONSE of the ViewModel and showing the response (formatted) in the result webview control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == RestActionViewModel.RETURNED_REST_RESPONSE)
            {
                // Data binding would be more elegant
                ShowResponse(_viewModel.ReturnedRestResponse);
            }
        }


        /// <summary>
        /// When one of the button is clicked, we go hide/show the adequate input controls in the right pane
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
            SwitchToRestAction(restAction);
        }

        /// <summary>
        /// Helper method called when user select a Rest action
        /// </summary>
        /// <param name="restAction"></param>
        private void SwitchToRestAction(RestAction restAction)
        {
            ShowResponse(null);
            string restActionStr = restAction.ToString();
            _viewModel[RestActionViewModel.SELECTED_REST_ACTION] = restActionStr;

            HashSet<string> names = RestActionViewHelper.GetNamesOfControlsToShow(restActionStr);
            foreach (TextBox tb in new TextBox[] {tbApiVersion, tbObjectType, 
                tbObjectId, tbExternalIdField, tbExternalId, tbFieldList, tbFields, 
                tbSoql, tbSosl, tbRequestPath, tbRequestBody, tbRequestMethod})
            {
                tb.Visibility = names.Contains(tb.Name) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Helper method to show a RestResponse (formatted) in the result webview control
        /// </summary>
        /// <param name="response"></param>
        private void ShowResponse(RestResponse response)
        {
            wbResult.NavigateToString(RestActionViewHelper.BuildHtml(response));
        }
    }
}
