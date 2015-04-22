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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Newtonsoft.Json.Linq;
using Salesforce.Sample.SmartSyncExplorer.Annotations;
using Salesforce.Sample.SmartSyncExplorer.Shared.Pages;
using Salesforce.Sample.SmartSyncExplorer.utilities;
using Salesforce.SDK.Auth;
using Salesforce.SDK.SmartStore.Store;
using Salesforce.SDK.SmartSync.Manager;
using Salesforce.SDK.SmartSync.Model;
using Salesforce.SDK.SmartSync.Util;

namespace Salesforce.Sample.SmartSyncExplorer.ViewModel
{
    public sealed class ContactSyncViewModel : INotifyPropertyChanged
    {
        public const string ContactSoup = "contacts";
        public const int Limit = 4000;
        private static readonly object _syncLock = new object();
        public event PropertyChangedEventHandler ContactsSynced;

        private static readonly IndexSpec[] ContactsIndexSpec =
        {
            new IndexSpec("Id", SmartStoreType.SmartString),
            new IndexSpec("FirstName", SmartStoreType.SmartString),
            new IndexSpec("LastName", SmartStoreType.SmartString),
            new IndexSpec(SyncManager.LocallyCreated, SmartStoreType.SmartString),
            new IndexSpec(SyncManager.LocallyUpdated, SmartStoreType.SmartString),
            new IndexSpec(SyncManager.LocallyDeleted, SmartStoreType.SmartString),
            new IndexSpec(SyncManager.Local, SmartStoreType.SmartString)
        };

        private readonly SmartStore _store;
        private readonly SyncManager _syncManager;

        private SortedObservableCollection<ContactObject> _contacts;
        private string _filter;
        private SortedObservableCollection<ContactObject> _filteredContacts;
        private ObservableCollection<string> _indexReference;
        private long syncId = -1;

        public ContactSyncViewModel()
        {
            Account account = AccountManager.GetAccount();
            if (account == null) return;
            _store = SmartStore.GetSmartStore(account);
            _syncManager = SyncManager.GetInstance(account);
            Contacts = new SortedObservableCollection<ContactObject>();
            FilteredContacts = new SortedObservableCollection<ContactObject>();
            IndexReference = new ObservableCollection<string>();
        }

        public SortedObservableCollection<ContactObject> Contacts
        {
            get { return _contacts; }
            private set
            {
                _contacts = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> IndexReference
        {
            get { return _indexReference; }
            private set
            {
                _indexReference = value;
                OnPropertyChanged();
            }
        }

        public string Filter
        {
            get { return _filter ?? String.Empty; }
            set
            {
                _filter = value;
            }
        }

        public bool FilterUsesContains { set; get; }

        public SortedObservableCollection<ContactObject> FilteredContacts
        {
            get { return _filteredContacts; }
            private set
            {
                _filteredContacts = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void SyncDownContacts()
        {
            RegisterSoup();
            if (syncId == -1)
            {
                string soqlQuery =
                    SOQLBuilder.GetInstanceWithFields(ContactObject.ContactFields)
                        .From(Constants.Contact)
                        .Limit(Limit)
                        .Build();
                SyncOptions options = SyncOptions.OptionsForSyncDown(SyncState.MergeModeOptions.LeaveIfChanged);
                SyncDownTarget target = new SoqlSyncDownTarget(soqlQuery);
                try
                {
                    SyncState sync = _syncManager.SyncDown(target, ContactSoup, HandleSyncUpdate);
                    syncId = sync.Id;
                }
                catch (SmartStoreException)
                {
                    // log here
                }
            }
            else
            {
                _syncManager.ReSync(syncId, HandleSyncUpdate);
            }
        }

        public void RegisterSoup()
        {
            _store.RegisterSoup(ContactSoup, ContactsIndexSpec);
        }

        public void SyncUpContacts()
        {
            RegisterSoup();
            SyncOptions options = SyncOptions.OptionsForSyncUp(ContactObject.ContactFields.ToList(), SyncState.MergeModeOptions.LeaveIfChanged);
            var target = new SyncUpTarget();
            _syncManager.SyncUp(target, options, ContactSoup, HandleSyncUpdate);
        }

        public async void DeleteObject(ContactObject contact)
        {
            if (contact == null) return;
            try
            {
                var item = _store.Retrieve(ContactSoup,
                    _store.LookupSoupEntryId(ContactSoup, Constants.Id, contact.ObjectId))[0].ToObject<JObject>();
                item[SyncManager.Local] = true;
                item[SyncManager.LocallyDeleted] = true;
                _store.Upsert(ContactSoup, item);
                contact.Deleted = true;
                await UpdateContact(contact);
            }
            catch (Exception)
            {
                Debug.WriteLine("Exception occurred while trying to delete");
            }
        }

        public async void SaveContact(ContactObject contact, bool isCreated)
        {
            if (contact == null) return;
            try
            {
                QuerySpec querySpec = QuerySpec.BuildExactQuerySpec(ContactSoup, Constants.Id, contact.ObjectId, 1);
                JArray returned = _store.Query(querySpec, 0);
                JObject item = returned.Count > 0 ? returned[0].ToObject<JObject>() : new JObject();
                item[ContactObject.FirstNameField] = contact.FirstName;
                item[ContactObject.LastNameField] = contact.LastName;
                item[Constants.NameField] = contact.ContactName;
                item[ContactObject.TitleField] = contact.Title;
                item[ContactObject.DepartmentField] = contact.Department;
                item[ContactObject.PhoneField] = contact.Phone;
                item[ContactObject.EmailField] = contact.Email;
                item[ContactObject.AddressField] = contact.Address;
                item[SyncManager.Local] = true;
                item[SyncManager.LocallyUpdated] = !isCreated;
                item[SyncManager.LocallyCreated] = isCreated;
                item[SyncManager.LocallyDeleted] = false;
                if (isCreated && item[Constants.Attributes.ToLower()] == null)
                {
                    item[Constants.Id] = "local_" + SmartStore.CurrentTimeMillis;
                    contact.ObjectId = item[Constants.Id].Value<string>();
                    var attributes = new JObject();
                    attributes[Constants.Type.ToLower()] = Constants.Contact;
                    item[Constants.Attributes.ToLower()] = attributes;
                    _store.Create(ContactSoup, item);
                }
                else
                {
                    _store.Upsert(ContactSoup, item);
                }
                contact.UpdatedOrCreated = true;
                await UpdateContact(contact);
            }
            catch (Exception)
            {
                Debug.WriteLine("Exception occurred while trying to save");
            }
        }

        private async Task<bool> UpdateContact(ContactObject obj)
        {
            if (obj == null || String.IsNullOrWhiteSpace(obj.ObjectId)) return false;
            CoreDispatcher core = CoreApplication.MainView.CoreWindow.Dispatcher;
            await core.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Contacts.Add(obj);
                OnPropertyChanged("Contacts");
            });
            return true;
        }

        private async Task<bool> RemoveContact(ContactObject obj)
        {
            bool result = false;
            CoreDispatcher core = CoreApplication.MainView.CoreWindow.Dispatcher;
            await core.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                result = Contacts.Remove(obj);
                OnPropertyChanged("Contacts");
            });
            return result;
        }

        public void ClearSmartStore()
        {
            _store.DropAllSoups();
            _store.ResetDatabase();
        }

        private void HandleSyncUpdate(SyncState sync)
        {
            if (SyncState.SyncStatusTypes.Done != sync.Status) return;
            switch (sync.SyncType)
            {
                case SyncState.SyncTypes.SyncUp:
                    RemoveDeleted();
                    ResetUpdated();
                    SyncDownContacts();
                    break;
                case SyncState.SyncTypes.SyncDown:
                    LoadDataFromSmartStore(false);
                    break;
            }
        }

        private async void RemoveDeleted()
        {
            CoreDispatcher core = CoreApplication.MainView.CoreWindow.Dispatcher;
            List<ContactObject> todelete = Contacts.Select(n => n).Where(n => n.IsLocallyModified || n.ObjectId.Contains("local")).ToList();
            foreach (var delete in todelete)
            {
                ContactObject delete1 = delete;
                await Task.Delay(10).ContinueWith(async a => await RemoveContact(delete1));
            }
        }

        private async void ResetUpdated()
        {
            CoreDispatcher core = CoreApplication.MainView.CoreWindow.Dispatcher;
            List<ContactObject> updated = Contacts.Select(n => n).Where(n => n.UpdatedOrCreated).ToList();
            for (int index = 0; index < updated.Count; index++)
            {
                ContactObject update = updated[index];
                await core.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    update.UpdatedOrCreated = false;
                    update.Deleted = false;
                });
                await UpdateContact(update);
            }
        }

        private void NotifyContactsSynced()
        {
            if (ContactsSynced != null)
                {
                    ContactsSynced(this, new PropertyChangedEventArgs("Contacts"));
                }
        }

        public void LoadDataFromSmartStore()
        {
            LoadDataFromSmartStore(true);
        }

        public async void LoadDataFromSmartStore(bool includeEdit)
        {
            if (!_store.HasSoup(ContactSoup))
            {
                NotifyContactsSynced();
                return;
            }
            QuerySpec querySpec = QuerySpec.BuildAllQuerySpec(ContactSoup, ContactObject.LastNameField,
                QuerySpec.SqlOrder.ASC,
                Limit);
            CoreDispatcher core = CoreApplication.MainView.CoreWindow.Dispatcher;
            JArray results = _store.Query(querySpec, 0);
            if (results == null)
            {
                NotifyContactsSynced(); 
                return;
            }
            ContactObject[] contacts;

            if (includeEdit)
            {
                contacts = (from contact in results
                    let model = new ContactObject(contact.Value<JObject>())
                    select model).ToArray();
            }
            else
            {
                contacts = (from contact in results
                            let model = new ContactObject(contact.Value<JObject>())
                            where !model.ObjectId.Contains("local")
                            orderby model.ContactName
                            select model).ToArray();
            }
            var references = (from contact in contacts let first = contact.ContactName[0].ToString().ToLower() group contact by first into g orderby g.Key select g.Key).ToArray();
            await core.RunAsync(CoreDispatcherPriority.Normal, () => IndexReference.Clear());
            await core.RunAsync(CoreDispatcherPriority.Normal, () => IndexReference.Add("all"));
            for (int i = 0, max = references.Length; i < max; i++)
            {
                var closure = i;
                await core.RunAsync(CoreDispatcherPriority.Normal, () => IndexReference.Add(references[closure]));
            }
            for (int i = 0, max = contacts.Length; i < max; i++)
            {
                ContactObject t = contacts[i];
                await UpdateContact(t);
            }
            NotifyContactsSynced();
            await core.RunAsync(CoreDispatcherPriority.Normal, () => MainPage.MainPageReference.UpdateTable());
        }

        public async void RunFilter()
        {
            List<ContactObject> contacts = _contacts.ToList();
            CoreDispatcher core = CoreApplication.MainView.CoreWindow.Dispatcher;

            await core.RunAsync(CoreDispatcherPriority.Normal, () => FilteredContacts.Clear());
            if (String.IsNullOrWhiteSpace(_filter))
            {
                await core.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    foreach (ContactObject contact in contacts)
                    {
                        FilteredContacts.Add(contact);
                    }
                });

                return;
            }
            List<ContactObject> filtered;
            if (FilterUsesContains)
            {
                filtered =
                    contacts.Where(
                        contact => !String.IsNullOrEmpty(contact.ContactName) && contact.ContactName.ToLower().Contains(_filter.ToLower()))
                        .ToList();
            }
            else
            {
                filtered =
                   contacts.Where(
                       contact => !String.IsNullOrEmpty(contact.ContactName) && contact.ContactName.StartsWith(_filter, StringComparison.CurrentCultureIgnoreCase))
                       .ToList();
            }
            await core.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                foreach (ContactObject contact in filtered)
                {
                    FilteredContacts.Add(contact);
                }
            });
        }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}