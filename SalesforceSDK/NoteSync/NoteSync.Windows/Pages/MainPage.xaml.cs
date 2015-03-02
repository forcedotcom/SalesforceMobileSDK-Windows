/*
 * Copyright (c) 2015, salesforce.com, inc.
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
using System.ComponentModel;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Newtonsoft.Json.Linq;
using NoteSync.Controls;
using NoteSync.ViewModel;
using Salesforce.SDK.App;
using Salesforce.SDK.Auth;
using Salesforce.SDK.Native;

namespace NoteSync.Pages
{
    public sealed partial class MainPage : NativeMainPage
    {
        private bool _firstSync;

        public MainPage()
        {
            InitializeComponent();
            FilterBox.TextChanged += FilterBox_TextChanged;
        }

        public static NoteSyncViewModel NotesDataModel { private set; get; }
        public static MainPage MainPageReference { private set; get; }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            Account account = AccountManager.GetAccount();
            MainPageReference = this;
            NotesTable.DataContext = this;
            _firstSync = true;
            if (account != null)
            {
                NotesDataModel = new NoteSyncViewModel();
                NotesDataModel.NotessSynced += NotesDataModel_ContactsSynced;
                NotesDataModel.LoadDataFromSmartStore();
                NotesTable.ItemsSource = NotesDataModel.Notes;
                IndexTable.ItemsSource = NotesDataModel.IndexReference;
                NotesDataModel.Filter = String.Empty;
            }
            else
            {
                base.OnNavigatedTo(e);
            }
            TitleBlock.Loaded += TitleBlock_Loaded;
        }

        private void TitleBlock_Loaded(object sender, RoutedEventArgs e)
        {
            TitleBlock.Loaded -= TitleBlock_Loaded;
            DisplayProgressFlyout("Loading Notes...");
        }

        private void NotesDataModel_ContactsSynced(object sender, PropertyChangedEventArgs e)
        {
            if (_firstSync && NotesDataModel.Notes.Count == 0)
            {
                _firstSync = false;
                NotesDataModel.SyncDownNotes();
            }
            else
            {
                MessageFlyout.Hide();
            }
        }

        private async void Logout(object sender, RoutedEventArgs e)
        {
            NotesDataModel.ClearSmartStore();
            if (SalesforceApplication.GlobalClientManager != null)
            {
                await SalesforceApplication.GlobalClientManager.Logout();
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
            NotesTable.ItemsSource = !String.IsNullOrWhiteSpace(NotesDataModel.Filter)
                ? NotesDataModel.FilteredNotes
                : NotesDataModel.Notes;
        }

        public void UpdateTable()
        {
            NotesTable.UpdateLayout();
        }

        private void Synchronize(object sender, RoutedEventArgs e)
        {
            DisplayProgressFlyout("Synchronizing Data...");
            try
            {
                NotesDataModel.SyncUpNotes();
            }
            catch (Exception)
            {
                NotesDataModel.RegisterSoup();
            }
        }

        private void CreateNote(object sender, RoutedEventArgs e)
        {
            EditCardPanel.DeleteVisible(false);
            new NoteObject(new JObject()) {UpdatedOrCreated = true};
            EditCardPanel.AttachedFlyout = EditCardFlyout;
            EditCardFlyout.ShowAt(TitleBlock);
        }

        private void Search(object sender, RoutedEventArgs e)
        {
            FilterBox.Text = NotesDataModel.Filter;
            FilterBoxFlyout.ShowAt(Commands);
        }

        private void ClearSearch(object sender, RoutedEventArgs e)
        {
            NotesDataModel.Filter = String.Empty;
            NotesDataModel.RunFilter();
        }


        private void FilterBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = FilterBox.Text;
            NotesDataModel.FilterUsesContains = true;
            NotesDataModel.Filter = text;
            NotesDataModel.RunFilter();
            NotesTable.ItemsSource = String.IsNullOrEmpty(text) ? NotesDataModel.Notes : NotesDataModel.FilteredNotes;
        }

        private void NotesTable_OnItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem != null)
            {
                EditNoteCard edit = EditCardPanel;
                edit.DeleteVisible(true);
                var data = e.ClickedItem as NoteObject;
                if (data != null)
                {
                    edit.Note = data;
                    edit.AttachedFlyout = EditCardFlyout;
                    DependencyObject container = NotesTable.ContainerFromItem(data);
                    EditCardFlyout.ShowAt(container as FrameworkElement);
                }
            }
        }
    }
}