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
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Newtonsoft.Json.Linq;
using Salesforce.Sample.SmartSyncExplorer.Controls;
using Salesforce.Sample.SmartSyncExplorer.utilities;
using Salesforce.Sample.SmartSyncExplorer.ViewModel;
using Salesforce.SDK.App;
using Salesforce.SDK.Auth;
using Salesforce.SDK.Native;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Salesforce.Sample.SmartSyncExplorer.Shared.Pages
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : NativeMainPage
    {
        public MainPage()
        {
            InitializeComponent();
            FilterBox.TextChanged += FilterBox_TextChanged;
        }

        public static ContactSyncViewModel ContactsDataModel { private set; get; }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            Account account = AccountManager.GetAccount();
            if (account != null)
            {
                ContactsDataModel = new ContactSyncViewModel();
                ContactsDataModel.SyncDownContacts();
                ContactsTable.DataContext = this;
                ContactsTable.ItemsSource = ContactsDataModel.Contacts;
            }
            else
            {
                base.OnNavigatedTo(e);
            }
        }

        private async void Logout(object sender, RoutedEventArgs e)
        {
            ContactsDataModel.ClearSmartStore();
            if (SalesforceApplication.GlobalClientManager != null)
            {
                await SalesforceApplication.GlobalClientManager.Logout();
            }
            AccountManager.SwitchAccount();
        }


        private void DisplayProgressFlyout(string text)
        {
            MessageContent.Text = text;
            MessageFlyout.ShowAt(ContactsTable);
        }

        private void ContactsTable_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems != null && e.AddedItems.Count > 0)
            {
                EditCard edit = EditCardPanel;
                edit.DeleteVisible(true);
                var data = e.AddedItems[0] as ContactObject;
                if (data != null)
                {
                    edit.Contact = data;
                    edit.AttachedFlyout = EditCardFlyout;
                    DependencyObject container = ContactsTable.ContainerFromItem(data);
                    EditCardFlyout.ShowAt(container as FrameworkElement);
                }
            }
        }

        private void Synchronize(object sender, RoutedEventArgs e)
        {
            try
            {
                ContactsDataModel.SyncUpContacts();
            }
            catch (Exception)
            {
                ContactsDataModel.RegisterSoup();
            }
        }

        private void CreateContact(object sender, RoutedEventArgs e)
        {
            EditCardPanel.DeleteVisible(false);
            EditCardPanel.Contact = new ContactObject(new JObject()) {UpdatedOrCreated = true};
            EditCardPanel.AttachedFlyout = EditCardFlyout;
            EditCardFlyout.ShowAt(TitleBlock);
        }

        private void Search(object sender, RoutedEventArgs e)
        {
            FilterBox.Text = ContactsDataModel.Filter;
            FilterBoxFlyout.ShowAt(Commands);
        }

        private void ClearSearch(object sender, RoutedEventArgs e)
        {
            ContactsDataModel.Filter = String.Empty;
        }


        void FilterBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var text = FilterBox.Text;
            ContactsDataModel.Filter = text;
            if (String.IsNullOrEmpty(text))
            {
                ContactsTable.ItemsSource = ContactsDataModel.Contacts;
            }
            else
            {
                ContactsTable.ItemsSource = ContactsDataModel.FilteredContacts;
            }
        }
    }
}