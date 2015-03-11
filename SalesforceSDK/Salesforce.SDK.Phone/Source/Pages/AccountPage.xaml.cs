/*
 * Copyright (c) 2014, salesforce.com, inc.
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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Resources;
using Windows.Security.Authentication.Web;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Salesforce.SDK.Adaptation;
using Salesforce.SDK.App;
using Salesforce.SDK.Auth;
using Salesforce.SDK.Source.Settings;
using Salesforce.SDK.Strings;
using Windows.Foundation.Diagnostics;

namespace Salesforce.SDK.Source.Pages
{
    /// <summary>
    ///     Phone based page for displaying accounts.
    /// </summary>
    public partial class AccountPage : Page, IWebAuthenticationContinuable
    {
        private const string SingleUserViewState = "SingleUser";
        private const string MultipleUserViewState = "MultipleUser";
        private const string LoggingUserInViewState = "LoggingUserIn";
        private bool _addServerFlyoutShowing;
        private string _currentState;

        public AccountPage()
        {
            InitializeComponent();
        }

        public Account[] Accounts
        {
            get { return AccountManager.GetAccounts().Values.ToArray(); }
        }

        public Account CurrentAccount
        {
            get
            {
                Account account = AccountManager.GetAccount();
                return account;
            }
        }

        public ObservableCollection<ServerSetting> Servers
        {
            get { return SalesforceApplication.ServerConfiguration.ServerList; }
        }

        public void ContinueWebAuthentication(WebAuthenticationBrokerContinuationEventArgs args)
        {
            WebAuthenticationResult webResult = args.WebAuthenticationResult;

            var logMsg = String.Format("AccountPage.ContinueWebAuthentication - WebAuthenticationResult: Status={0}", webResult.ResponseStatus);
            if (webResult.ResponseStatus == WebAuthenticationStatus.ErrorHttp)
                logMsg += string.Format(", ErrorDetail={0}", webResult.ResponseErrorDetail);

            PlatformAdapter.SendToCustomLogger(logMsg, LoggingLevel.Verbose);

            if (webResult.ResponseStatus == WebAuthenticationStatus.Success)
            {
                var responseUri = new Uri(webResult.ResponseData);
                if (!responseUri.Query.Contains("error="))
                {
                    AuthResponse authResponse = OAuth2.ParseFragment(responseUri.Fragment.Substring(1));
                    PlatformAdapter.Resolve<IAuthHelper>().EndLoginFlow(SalesforceConfig.LoginOptions, authResponse);
                }
                else
                {
                    DisplayErrorDialog(LocalizedStrings.GetString("generic_error"));
                    SetupAccountPage();
                }
            }
            else
            {
                DisplayErrorDialog(LocalizedStrings.GetString("generic_authentication_error"));
                SetupAccountPage();
            }
        }

        private void DisplayErrorDialog(string message)
        {
            MessageContent.Text = message;
            MessageFlyout.ShowAt(ApplicationLogo);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            SetupAccountPage();
        }

        private void SetupAccountPage()
        {
            ResourceLoader loader = ResourceLoader.GetForCurrentView("Salesforce.SDK.Core/Resources");
            SalesforceConfig config = SalesforceApplication.ServerConfiguration;
            bool titleMissing = true;
            if (!String.IsNullOrWhiteSpace(config.ApplicationTitle) && config.IsApplicationTitleVisible)
            {
                ApplicationTitle.Visibility = Visibility.Visible;
                ApplicationTitle.Text = config.ApplicationTitle;
                titleMissing = false;
            }
            else
            {
                ApplicationTitle.Visibility = Visibility.Collapsed;
            }

            if (config.LoginBackgroundLogo != null)
            {
                if (ApplicationLogo.Items != null)
                {
                    ApplicationLogo.Items.Clear();
                    ApplicationLogo.Items.Add(config.LoginBackgroundLogo);
                }
                if (titleMissing)
                {
                    var padding = new Thickness(10, 24, 10, 24);
                    ApplicationLogo.Margin = padding;
                }
            }

            // set background from config
            if (config.LoginBackgroundColor != null)
            {
                var background = new SolidColorBrush((Color)config.LoginBackgroundColor);
                Background = background;
                ServerFlyoutPanel.Background = background;
                AddServerFlyoutPanel.Background = background;
            }

            // set foreground from config
            if (config.LoginForegroundColor != null)
            {
                var foreground = new SolidColorBrush((Color) config.LoginForegroundColor);
                Foreground = foreground;
                ApplicationTitle.Foreground = foreground;
                LoginToSalesforce.Foreground = foreground;
                LoginToSalesforce.BorderBrush = foreground;
                ChooseConnection.Foreground = foreground;
                ChooseConnection.BorderBrush = foreground;
                AddServerFlyoutLabel.Foreground = foreground;
                AddCustomHostBtn.Foreground = foreground;
                AddCustomHostBtn.BorderBrush = foreground;
                CancelCustomHostBtn.Foreground = foreground;
                CancelCustomHostBtn.BorderBrush = foreground;
                ServerFlyoutLabel.Foreground = foreground;
                AddConnection.Foreground = foreground;
                AddConnection.BorderBrush = foreground;
            }

            if (Accounts == null || Accounts.Length == 0)
            {
                _currentState = SingleUserViewState;
                SetLoginBarVisibility(Visibility.Collapsed);
                PincodeManager.WipePincode();
                VisualStateManager.GoToState(this, SingleUserViewState, true);
            }
            else
            {
                _currentState = MultipleUserViewState;
                SetLoginBarVisibility(Visibility.Visible);
                ListTitle.Text = loader.GetString("select_account");
                VisualStateManager.GoToState(this, MultipleUserViewState, true);
            }
            ListboxServers.ItemsSource = Servers;
            AccountsList.ItemsSource = Accounts;
            ServerFlyout.Opening += ServerFlyout_Opening;
            ServerFlyout.Closed += ServerFlyout_Closed;
            AddServerFlyout.Opened += AddServerFlyout_Opened;
            AddServerFlyout.Closed += AddServerFlyout_Closed;
            AccountsList.SelectionChanged += accountsList_SelectionChanged;
            AddConnection.Visibility = (SalesforceApplication.ServerConfiguration.AllowNewConnections
                ? Visibility.Visible
                : Visibility.Collapsed);
        }

        private async void accountsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await AccountManager.SwitchToAccount(AccountsList.SelectedItem as Account);
            SalesforceApplication.ResetClientManager();
            if (SalesforceApplication.GlobalClientManager.PeekRestClient() != null)
            {
                Frame.Navigate(SalesforceApplication.RootApplicationPage);
                Account account = AccountManager.GetAccount();
                PincodeManager.LaunchPincodeScreen();
            }
        }

        private void AddServerFlyout_Opened(object sender, object e)
        {
            if (AddCustomHostBtn.ActualWidth.CompareTo(CancelCustomHostBtn.ActualWidth) < 0)
            {
                AddCustomHostBtn.Width = CancelCustomHostBtn.ActualWidth;
            }
            else if (AddCustomHostBtn.ActualWidth.CompareTo(CancelCustomHostBtn.ActualWidth) > 0)
            {
                CancelCustomHostBtn.Width = AddCustomHostBtn.ActualWidth;
            }
        }

        private void AddServerFlyout_Closed(object sender, object e)
        {
            ServerFlyout.ShowAt(ApplicationTitle);
            _addServerFlyoutShowing = false;
        }

        private void ServerFlyout_Closed(object sender, object e)
        {
            if (!_addServerFlyoutShowing)
                SetLoginBarVisibility(Visibility.Visible);
        }

        private void ServerFlyout_Opening(object sender, object e)
        {
            SetLoginBarVisibility(Visibility.Collapsed);
        }

        private void ShowServerFlyout(object sender, RoutedEventArgs e)
        {
            if (Servers.Count <= 1 && !SalesforceApplication.ServerConfiguration.AllowNewConnections)
            {
                ListboxServers.SelectedIndex = 0;
                addAccount_Click(sender, e);
            }
            else
            {
                ListboxServers.SelectedIndex = -1;
                ServerFlyout.ShowAt(ApplicationTitle);
            }
        }

        public void StartLoginFlow(LoginOptions loginOptions)
        {
            if (loginOptions == null || String.IsNullOrWhiteSpace(loginOptions.CallbackUrl) ||
                String.IsNullOrWhiteSpace(loginOptions.LoginUrl))
                return;
            try
            {
                var loginUri = new Uri(OAuth2.ComputeAuthorizationUrl(loginOptions));
                var callbackUri = new Uri(loginOptions.CallbackUrl);
                WebAuthenticationBroker.AuthenticateAndContinue(loginUri, callbackUri, null,
                    WebAuthenticationOptions.None);
            }
            catch (Exception ex)
            {
                PlatformAdapter.SendToCustomLogger("AccountPage.StartLoginFlow - Exception occured", LoggingLevel.Critical);
                PlatformAdapter.SendToCustomLogger(ex, LoggingLevel.Critical);

                PlatformAdapter.Resolve<IAuthHelper>().StartLoginFlow();
            }
        }

        private void addConnection_Click(object sender, RoutedEventArgs e)
        {
            _addServerFlyoutShowing = true;
            HostName.Text = "";
            HostAddress.Text = "";
            try
            {
                AddServerFlyout.ShowAt(ApplicationTitle);
            }
            catch (ArgumentException)
            {
                Debug.WriteLine("Error displaying connection flyout");
            }
            
        }

        private void addCustomHostBtn_Click(object sender, RoutedEventArgs e)
        {
            string hname = HostName.Text;
            string haddress = HostAddress.Text;
            if (String.IsNullOrWhiteSpace(haddress))
            {
                return;
            }
            if (String.IsNullOrWhiteSpace(hname))
            {
                hname = haddress;
            }
            var server = new ServerSetting
            {
                ServerHost = haddress,
                ServerName = hname
            };
            SalesforceApplication.ServerConfiguration.AddServer(server);

            ServerFlyout.ShowAt(ApplicationTitle);
        }

        private void cancelCustomHostBtn_Click(object sender, RoutedEventArgs e)
        {
            ServerFlyout.ShowAt(ApplicationTitle);
        }

        private void LoginToSalesforce_OnClick(object sender, RoutedEventArgs e)
        {
            StartLoginFlow(ListboxServers.Items[0] as ServerSetting);
        }

        private void ClickServer(object sender, RoutedEventArgs e)
        {
            StartLoginFlow(ListboxServers.SelectedItem as ServerSetting);
        }

        private void DeleteServer(object sender, RoutedEventArgs e)
        {
            SalesforceApplication.ServerConfiguration.ServerList.Remove(ListboxServers.SelectedItem as ServerSetting);
            SalesforceApplication.ServerConfiguration.SaveConfig();
        }

        private void addAccount_Click(object sender, RoutedEventArgs e)
        {
            StartLoginFlow(ListboxServers.SelectedItem as ServerSetting);
        }

        private void StartLoginFlow(ServerSetting server)
        {
            if (server != null)
            {
                VisualStateManager.GoToState(this, LoggingUserInViewState, true);
                SalesforceApplication.ResetClientManager();
                SalesforceConfig config = SalesforceApplication.ServerConfiguration;
                var options = new LoginOptions(server.ServerHost, config.ClientId, config.CallbackUrl, config.Scopes);
                SalesforceConfig.LoginOptions = new LoginOptions(server.ServerHost, config.ClientId, config.CallbackUrl,
                    config.Scopes);
                StartLoginFlow(options);
            }
        }

        private void SetLoginBarVisibility(Visibility state)
        {
            LoginBar.Visibility = MultipleUserViewState.Equals(_currentState) ? state : Visibility.Collapsed;
        }

        private void CloseMessageButton_OnClick(object sender, RoutedEventArgs e)
        {
            MessageFlyout.Hide();
        }

        private void textBlockInTemplate_Loaded(object sender, RoutedEventArgs e)
        {
            var tb = sender as TextBlock;
            if (tb != null)
                tb.Foreground = this.Foreground;
        }
    }
}