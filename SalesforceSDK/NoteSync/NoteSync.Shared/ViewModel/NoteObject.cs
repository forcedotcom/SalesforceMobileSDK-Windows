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
using System.Runtime.CompilerServices;
using Newtonsoft.Json.Linq;
using Salesforce.SDK.SmartSync.Manager;
using Salesforce.SDK.SmartSync.Model;
using Salesforce.SDK.SmartSync.Util;

namespace NoteSync.ViewModel
{
    public class NoteObject : SalesforceObject, IComparable, IComparable<NoteObject>, INotifyPropertyChanged
    {
        public const string TitleField = "Title";
        public const string ContentField = "Content";
        public const string NoteSObjectType = "ContentNote";

        public static readonly string[] NoteFields =
        {
            Constants.Id, TitleField, ContentField, Constants.LastModifiedDate
        };

        public readonly string Synced = '\u2601'.ToString();
        public readonly string ToDelete = '\uE107'.ToString();
        public readonly string Unsynced = '\uE104'.ToString();

        private string _content;
        private bool _deleted;
        private string _title;
        private bool _updatedOrChanged;

        public NoteObject(JObject data)
            : base(data)
        {
            ObjectType = Constants.Contact;
            ObjectId = data.ExtractValue<string>(Constants.Id);
            Title = data.ExtractValue<string>(TitleField);
            Content = data.ExtractValue<string>(ContentField);

            UpdatedOrCreated =
                data.ExtractValue<bool>(SyncManager.LocallyUpdated) ||
                data.ExtractValue<bool>(SyncManager.LocallyCreated);
            Deleted = data.ExtractValue<bool>(SyncManager.LocallyDeleted);
        }

        public string Title
        {
            set
            {
                _title = value;
                OnPropertyChanged();
                OnPropertyChanged("Title");
            }
            get { return _title; }
        }

        public string Content
        {
            set
            {
                _content = value;
                OnPropertyChanged();
                OnPropertyChanged("Content");
            }
            get { return _content; }
        }

        public bool UpdatedOrCreated
        {
            set
            {
                _updatedOrChanged = value;
                OnPropertyChanged();
                OnPropertyChanged("SyncStatus");
            }
            get { return _updatedOrChanged; }
        }

        public bool Deleted
        {
            set
            {
                _deleted = value;
                OnPropertyChanged();
                OnPropertyChanged("SyncStatus");
            }
            get { return _deleted; }
        }

        public string SyncStatus
        {
            get
            {
                if (UpdatedOrCreated)
                {
                    return Unsynced;
                }
                if (Deleted)
                {
                    return ToDelete;
                }
                return Synced;
            }
        }

        public int CompareTo(object obj)
        {
            return Compare(obj as NoteObject);
        }

        public int CompareTo(NoteObject other)
        {
            return Compare(other);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private int Compare(object other)
        {
            NoteObject item1 = this;
            var item2 = other as NoteObject;
            if (item2 == null)
                return -1;
            int retVal = 0;
            retVal += String.Compare(item1.Title, item2.Title, StringComparison.CurrentCultureIgnoreCase);
            retVal += String.Compare(item1.Content, item2.Content, StringComparison.CurrentCultureIgnoreCase);
            return retVal;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}