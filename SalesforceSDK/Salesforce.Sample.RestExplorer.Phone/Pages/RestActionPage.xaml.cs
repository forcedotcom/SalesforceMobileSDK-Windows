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
using Salesforce.Sample.RestExplorer.ViewModels;
using Salesforce.SDK.Rest;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Salesforce.SDK.Source.Utilities;
using Salesforce.Sample.RestExplorer.Shared;
using Windows.UI.Xaml;
using Windows.Phone.UI.Input;

namespace Salesforce.Sample.RestExplorer.Phone
{
    /// <summary>
    /// Page used to make REST calls and display their results
    /// </summary>
    public partial class RestActionPage : Page
    {
        private RestActionViewModel _viewModel;

        /// <summary>
        /// Constructor
        /// </summary>
        public RestActionPage()
        {
            InitializeComponent();
            ShowResponse(null);
            _viewModel = DataContext as RestActionViewModel;
            _viewModel.SyncContext = SynchronizationContext.Current;
            _viewModel.PropertyChanged += OnViewModelPropertyChanged;
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
        }

        void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame != null && rootFrame.CanGoBack)
            {
                rootFrame.GoBack();
                e.Handled = true;
            }

        }

        /// <summary>
        /// Watching the RETURNED_REST_RESPONSE of the ViewModel and showing the response (formatted) in the result webbrowser control
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
        /// When page is navigated to, we first parse the rest action name out of the URI
        /// Then we hide/show the input controls that should be visible for that action
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string restActionStr = e.Parameter.ToString();
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
        /// Helper method to show a RestResponse (formatted) in the result webbrowser control
        /// </summary>
        /// <param name="response"></param>
        private void ShowResponse(RestResponse response)
        {
            wbResult.NavigateToString(RestActionViewHelper.BuildHtml(response));
        }
    }
}