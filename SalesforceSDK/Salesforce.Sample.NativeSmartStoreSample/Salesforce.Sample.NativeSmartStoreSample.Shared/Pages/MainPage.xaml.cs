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
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Salesforce.Sample.NativeSmartStoreSample.utilities;
using Salesforce.SDK.App;
using Salesforce.SDK.Auth;
using Salesforce.SDK.Native;
using Salesforce.SDK.Rest;
using Account = Salesforce.Sample.NativeSmartStoreSample.utilities.Account;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Salesforce.Sample.NativeSmartStoreSample.Shared.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : NativeMainPage
    {
        private const string ApiVersion = "v30.0";
        private SmartStoreExtension _store;
        public static ObservableCollection<Salesforce.Sample.NativeSmartStoreSample.utilities.Account> Accounts;
        public static ObservableCollection<Opportunity> Opportunities; 
 
        public MainPage()
            : base()
        {
            InitializeComponent();
            Accounts = new ObservableCollection<Account>();
            Opportunities = new ObservableCollection<Opportunity>();
            AccountTable.ItemsSource = Accounts;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var account = AccountManager.GetAccount();
            if (account != null)
            {
                _store = new SmartStoreExtension();
            }
            else
            {
                base.OnNavigatedTo(e);
            }
        }

        private async void Logout(object sender, RoutedEventArgs e)
        {

            if (SDKManager.GlobalClientManager != null)
            {
                await SDKManager.GlobalClientManager.Logout();
            }
            AccountManager.SwitchAccount();
        }

        private async Task<bool> SendRequest(string soql, string obj)
        {
            RestRequest restRequest = RestRequest.GetRequestForQuery(ApiVersion, soql);
            RestClient client = SDKManager.GlobalClientManager.GetRestClient();
            RestResponse response = await client.SendAsync(restRequest);
            if (response.Success)
            {
                var records = response.AsJObject.GetValue("records").ToObject<JArray>();
                if ("Account".Equals(obj, StringComparison.CurrentCultureIgnoreCase))
                {
                    _store.InsertAccounts(records);
                }
                else if ("Opportunity".Equals(obj, StringComparison.CurrentCultureIgnoreCase))
                {
                    _store.InsertOpportunities(records);
                }
                else
                {
                    /*
                    * If the object is not an account or opportunity,
                    * we do nothing. This block can be used to save
                    * other types of records.
                    */
                }
                return true;
            }
            return false;
        }

        private void LoadData()
        {
            DisplayProgressFlyout("Loading Local Data, please wait!");
            var accounts = _store.GetAccounts();
            var opps = _store.GetOpportunities();
            AccountTable.Visibility = Visibility.Visible;
            foreach (var account in accounts)
            {
                Accounts.Add(JsonConvert.DeserializeObject<Account>(account[0].ToString()));
            }
            foreach (var opp in opps)
            {
                Opportunities.Add(JsonConvert.DeserializeObject<Opportunity>(opp[0].ToString()));
            }
            MessageFlyout.Hide();
        }

        private async void SaveOffline_OnClick(object sender, RoutedEventArgs e)
        {

            try
            {
                DisplayProgressFlyout("Loading Remote Data, please wait!");
                await SendRequest("SELECT Name, Id, OwnerId FROM Account", "Account");
                await SendRequest("SELECT Name, Id, AccountId, OwnerId, Amount FROM Opportunity", "Opportunity");
            }
            catch
            {
            }
            finally
            {
                MessageFlyout.Hide();
            }
        }

        private void ClearOffline_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                DisplayProgressFlyout("Deleting local data, please wait!");
                AccountTable.Visibility = Visibility.Collapsed;
                _store.DeleteAccountsSoup();
                _store.DeleteOpportunitiesSoup();
                _store.CreateAccountsSoup();
                _store.CreateOpportunitiesSoup();
                Accounts.Clear();
                Opportunities.Clear();
            }
            catch
            {
            }
            finally
            {
                MessageFlyout.Hide();
            }
           
        }

        private void DisplayProgressFlyout(string text)
        {
            MessageContent.Text = text;
            MessageFlyout.ShowAt(RunReport);
        }

        private void RunReport_OnClick(object sender, RoutedEventArgs e)
        {
           LoadData();
        }
    }
}
