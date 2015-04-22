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
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Newtonsoft.Json.Linq;
using Salesforce.Sample.SmartSyncExplorer.Controls;
using Salesforce.Sample.SmartSyncExplorer.utilities;
using Salesforce.Sample.SmartSyncExplorer.ViewModel;
using Salesforce.SDK.Auth;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238
using Salesforce.SDK.Native;

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
        public static MainPage MainPageReference { private set; get; }
        private bool _firstSync;

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            Account account = AccountManager.GetAccount();
            MainPageReference = this;
            ContactsTable.DataContext = this;
            _firstSync = true;
            if (account != null)
            {
                ContactsDataModel = new ContactSyncViewModel();
                ContactsDataModel.ContactsSynced += ContactsDataModel_ContactsSynced;
                ContactsDataModel.LoadDataFromSmartStore();
                ContactsTable.ItemsSource = ContactsDataModel.Contacts;
                IndexTable.ItemsSource = ContactsDataModel.IndexReference;
                ContactsDataModel.Filter = String.Empty;
            }
            else
            {
                base.OnNavigatedTo(e);
            }
            TitleBlock.Loaded += TitleBlock_Loaded;
        }

        void TitleBlock_Loaded(object sender, RoutedEventArgs e)
        {
            TitleBlock.Loaded -= TitleBlock_Loaded;
            DisplayProgressFlyout("Loading Contacts...");
        }

        void ContactsDataModel_ContactsSynced(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_firstSync && ContactsDataModel.Contacts.Count == 0)
            {
                _firstSync = false;
                ContactsDataModel.SyncDownContacts();
            }
            else
            {
                MessageFlyout.Hide();
            }
        }

        private async void Logout(object sender, RoutedEventArgs e)
        {
            ContactsDataModel.ClearSmartStore();
            if (SDKManager.GlobalClientManager != null)
            {
                await SDKManager.GlobalClientManager.Logout();
            }
            AccountManager.SwitchAccount();
        }


        private void DisplayProgressFlyout(string text)
        {
            MessageContent.Text = text;
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => MessageFlyout.ShowAt(TitleBlock));
        }

        public void ZoomIn()
        {
            Zoom.IsZoomedInViewActive = true;
            if (!String.IsNullOrWhiteSpace(ContactsDataModel.Filter))
            {
                ContactsTable.ItemsSource = ContactsDataModel.FilteredContacts;
            }
            else
            {
                ContactsTable.ItemsSource = ContactsDataModel.Contacts;
            }
        }

        public void UpdateTable()
        {
            ContactsTable.UpdateLayout();
        }

        private void Synchronize(object sender, RoutedEventArgs e)
        {
            DisplayProgressFlyout("Synchronizing Data...");
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
            ContactsDataModel.RunFilter();
        }


        void FilterBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var text = FilterBox.Text;
            ContactsDataModel.FilterUsesContains = true;
            ContactsDataModel.Filter = text;
            ContactsDataModel.RunFilter();
            ContactsTable.ItemsSource = String.IsNullOrEmpty(text) ? ContactsDataModel.Contacts : ContactsDataModel.FilteredContacts;
        }

        private void ContactsTable_OnItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem != null)
            {
                EditCard edit = EditCardPanel;
                edit.DeleteVisible(true);
                var data = e.ClickedItem as ContactObject;
                if (data != null)
                {
                    edit.Contact = data;
                    edit.AttachedFlyout = EditCardFlyout;
                    DependencyObject container = ContactsTable.ContainerFromItem(data);
                    EditCardFlyout.ShowAt(container as FrameworkElement);
                }
            }
        }
    }
}