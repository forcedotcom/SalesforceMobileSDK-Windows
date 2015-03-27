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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Newtonsoft.Json.Linq;
using NoteSync.Data;
using NoteSync.Pages;
using Salesforce.SDK.Auth;
using Salesforce.SDK.SmartStore.Store;
using Salesforce.SDK.SmartSync.Manager;
using Salesforce.SDK.SmartSync.Model;
using Salesforce.SDK.SmartSync.Util;

namespace NoteSync.ViewModel
{
    public class NoteSyncViewModel : INotifyPropertyChanged
    {
        public const string NotesSoup = "notes";
        private const int Limit = 25;

        private static readonly IndexSpec[] NotesIndexSpec =
        {
            new IndexSpec("Id", SmartStoreType.SmartString),
            new IndexSpec("Title", SmartStoreType.SmartString),
            new IndexSpec("Content", SmartStoreType.SmartString),
            new IndexSpec(SyncManager.LocallyCreated, SmartStoreType.SmartString),
            new IndexSpec(SyncManager.LocallyUpdated, SmartStoreType.SmartString),
            new IndexSpec(SyncManager.LocallyDeleted, SmartStoreType.SmartString),
            new IndexSpec(SyncManager.Local, SmartStoreType.SmartString)
        };

        private static readonly object _syncLock = new object();

        private readonly SmartStore _store;
        private readonly SyncManager _syncManager;
        private string _filter;
        private SortedObservableCollection<NoteObject> _filteredNotes;
        private ObservableCollection<string> _indexReference;
        private SortedObservableCollection<NoteObject> _notes;
        private long syncId = -1;

        public NoteSyncViewModel()
        {
            Account account = AccountManager.GetAccount();
            if (account == null) return;
            _store = SmartStore.GetSmartStore();
            _syncManager = SyncManager.GetInstance(account);
            Notes = new SortedObservableCollection<NoteObject>();
            FilteredNotes = new SortedObservableCollection<NoteObject>();
            IndexReference = new ObservableCollection<string>();
        }

        public SortedObservableCollection<NoteObject> Notes
        {
            get { return _notes; }
            private set
            {
                _notes = value;
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
            set { _filter = value; }
        }

        public bool FilterUsesContains { set; get; }

        public SortedObservableCollection<NoteObject> FilteredNotes
        {
            get { return _filteredNotes; }
            private set
            {
                _filteredNotes = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangedEventHandler NotessSynced;

        public void RegisterSoup()
        {
            _store.RegisterSoup(NotesSoup, NotesIndexSpec);
        }

        public void ClearSmartStore()
        {
            _store.DropAllSoups();
            _store.ResetDatabase();
        }

        public void SyncDownNotes()
        {
            RegisterSoup();
            string soqlQuery =
                SOQLBuilder.GetInstanceWithFields(NoteObject.NoteFields)
                    .From(NoteObject.NoteSObjectType)
                    .Limit(Limit)
                    .Build();
            SyncOptions options = SyncOptions.OptionsForSyncDown(SyncState.MergeModeOptions.LeaveIfChanged);
            SyncDownTarget target = ContentSoqlSyncDownTarget.TargetForSOQLSyncDown(soqlQuery);
            try
            {
                SyncState sync = _syncManager.SyncDown(target, NotesSoup, HandleSyncUpdate, options);
                syncId = sync.Id;
            }
            catch (SmartStoreException)
            {
                // log here
            }
        }

        public void SyncUpNotes()
        {
            RegisterSoup();
            SyncOptions options = SyncOptions.OptionsForSyncUp(NoteObject.NoteFields.ToList(),
                SyncState.MergeModeOptions.LeaveIfChanged);
            var target = new SyncUpTarget();
            _syncManager.SyncUp(target, options, NotesSoup, HandleSyncUpdate);
        }

        private void HandleSyncUpdate(SyncState sync)
        {
            if (SyncState.SyncStatusTypes.Done != sync.Status) return;
            switch (sync.SyncType)
            {
                case SyncState.SyncTypes.SyncUp:
                    SyncDownNotes();
                    break;
                case SyncState.SyncTypes.SyncDown:
                    LoadDataFromSmartStore(false);
                    break;
            }
        }

        private void NotifyNotesSynced()
        {
            if (NotessSynced != null)
            {
                NotessSynced(this, new PropertyChangedEventArgs("Notes"));
            }
        }

        public void LoadDataFromSmartStore()
        {
            LoadDataFromSmartStore(true);
        }

        public async void LoadDataFromSmartStore(bool includeEdit)
        {
            if (!_store.HasSoup(NotesSoup))
            {
                NotifyNotesSynced();
                return;
            }
            QuerySpec querySpec = QuerySpec.BuildAllQuerySpec(NotesSoup, NoteObject.TitleField,
                QuerySpec.SqlOrder.ASC,
                Limit);
            CoreDispatcher core = CoreApplication.MainView.CoreWindow.Dispatcher;
            JArray results = _store.Query(querySpec, 0);
            if (results == null)
            {
                NotifyNotesSynced();
                return;
            }
            NoteObject[] notes;

            if (includeEdit)
            {
                notes = (from note in results
                    let model = new NoteObject(note.Value<JObject>())
                    select model).ToArray();
            }
            else
            {
                notes = (from note in results
                    let model = new NoteObject(note.Value<JObject>())
                    where !model.ObjectId.Contains("local")
                    orderby model.Title
                    select model).ToArray();
            }
            string[] references =
                (from note in notes
                    let title = note.Title[0].ToString().ToLower()
                    group note by title
                    into g
                    orderby g.Key
                    select g.Key).ToArray();
            await core.RunAsync(CoreDispatcherPriority.Normal, () => IndexReference.Clear());
            await core.RunAsync(CoreDispatcherPriority.Normal, () => IndexReference.Add("all"));
            for (int i = 0, max = references.Length; i < max; i++)
            {
                int closure = i;
                await core.RunAsync(CoreDispatcherPriority.Normal, () => IndexReference.Add(references[closure]));
            }
            for (int i = 0, max = notes.Length; i < max; i++)
            {
                NoteObject t = notes[i];
                await UpdateNote(t);
            }
            NotifyNotesSynced();
            await core.RunAsync(CoreDispatcherPriority.Normal, () => MainPage.MainPageReference.UpdateTable());
        }

        private async Task<bool> UpdateNote(NoteObject obj)
        {
            if (obj == null || String.IsNullOrWhiteSpace(obj.ObjectId)) return false;
            CoreDispatcher core = CoreApplication.MainView.CoreWindow.Dispatcher;
            await core.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Notes.Add(obj);
                OnPropertyChanged("Notes");
            });
            return true;
        }

        public async void RunFilter()
        {
            List<NoteObject> notes = _notes.ToList();
            CoreDispatcher core = CoreApplication.MainView.CoreWindow.Dispatcher;

            await core.RunAsync(CoreDispatcherPriority.Normal, () => FilteredNotes.Clear());
            if (String.IsNullOrWhiteSpace(_filter))
            {
                await core.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    foreach (NoteObject note in notes)
                    {
                        FilteredNotes.Add(note);
                    }
                });

                return;
            }
            List<NoteObject> filtered;
            if (FilterUsesContains)
            {
                filtered =
                    notes.Where(
                        note =>
                            !String.IsNullOrEmpty(note.Content) && note.Content.ToLower().Contains(_filter.ToLower()))
                        .ToList();
            }
            else
            {
                filtered =
                    notes.Where(
                        note =>
                            !String.IsNullOrEmpty(note.Content) &&
                            note.Content.StartsWith(_filter, StringComparison.CurrentCultureIgnoreCase))
                        .ToList();
            }
            await core.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                foreach (NoteObject note in filtered)
                {
                    FilteredNotes.Add(note);
                }
            });
        }

        public async void SaveNote(NoteObject note, bool isCreated)
        {
            if (note == null) return;
            try
            {
                QuerySpec querySpec = QuerySpec.BuildExactQuerySpec(NotesSoup, Constants.Id, note.ObjectId, 1);
                JArray returned = _store.Query(querySpec, 0);
                JObject item = returned.Count > 0 ? returned[0].ToObject<JObject>() : new JObject();

                item[NoteObject.TitleField] = note.Title;
                item[NoteObject.ContentField] = Convert.ToBase64String(Encoding.UTF8.GetBytes(note.Content));
                item[SyncManager.Local] = true;
                item[SyncManager.LocallyUpdated] = !isCreated;
                item[SyncManager.LocallyCreated] = isCreated;
                item[SyncManager.LocallyDeleted] = false;
                if (isCreated && item[Constants.Attributes.ToLower()] == null)
                {
                    item[Constants.Id] = "local_" + SmartStore.CurrentTimeMillis;
                    note.ObjectId = item[Constants.Id].Value<string>();
                    var attributes = new JObject();
                    attributes[Constants.Type.ToLower()] = Constants.Contact;
                    item[Constants.Attributes.ToLower()] = attributes;
                    _store.Create(NotesSoup, item);
                }
                else
                {
                    _store.Upsert(NotesSoup, item);
                }
                note.UpdatedOrCreated = true;
                await UpdateNote(note);
            }
            catch (Exception)
            {
                Debug.WriteLine("Exception occurred while trying to save");
            }
        }

        public async void DeleteObject(NoteObject note)
        {
            if (note == null) return;
            try
            {
                var item = _store.Retrieve(NotesSoup,
                    _store.LookupSoupEntryId(NotesSoup, Constants.Id, note.ObjectId))[0].ToObject<JObject>();
                item[SyncManager.Local] = true;
                item[SyncManager.LocallyDeleted] = true;
                _store.Upsert(NotesSoup, item);
                note.Deleted = true;
                await UpdateNote(note);
            }
            catch (Exception)
            {
                Debug.WriteLine("Exception occurred while trying to delete");
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}