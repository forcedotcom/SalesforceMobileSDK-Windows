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
using System.Runtime.Serialization;
using Windows.ApplicationModel.Contacts;
using Newtonsoft.Json.Linq;
using Salesforce.SDK.SmartSync.Manager;
using Salesforce.SDK.SmartSync.Model;
using Salesforce.SDK.SmartSync.Util;

namespace Salesforce.Sample.SmartSyncExplorer.utilities
{
    public class ContactObject : SalesforceObject, IComparable, IComparable<ContactObject>
    {
        public const string FirstNameField = "FirstName";
        public const string LastNameField = "LastName";
        public const string TitleField = "Title";
        public const string PhoneField = "HomePhone";
        public const string EmailField = "Email";
        public const string DepartmentField = "Department";
        public const string AddressField = "MailingStreet";
        public readonly string Synced = '\u2601'.ToString();
        public readonly string Unsynced = '\uE104'.ToString();
        public readonly string ToDelete = '\uE107'.ToString();

        public static readonly string[] ContactFields =
        {
            Constants.Id, FirstNameField, LastNameField, TitleField,
            PhoneField, EmailField, DepartmentField, AddressField
        };

        public ContactObject(JObject data)
            : base(data)
        {
            ObjectType = Constants.Contact;
            ObjectId = data.ExtractValue<string>(Constants.Id);
            FirstName = data.ExtractValue<string>(FirstNameField);
            LastName = data.ExtractValue<string>(LastNameField);
            Name = FirstName + " " + LastName;
            UpdatedOrCreated =
                data.ExtractValue<bool>(SyncManager.LocallyUpdated) ||
                                        data.ExtractValue<bool>(SyncManager.LocallyCreated);
            Deleted = data.ExtractValue<bool>(SyncManager.LocallyDeleted);
            Title = data.ExtractValue<string>(TitleField);
            Phone = data.ExtractValue<string>(PhoneField);
            Email = data.ExtractValue<string>(EmailField);
            Department = data.ExtractValue<string>(DepartmentField);
            Address = data.ExtractValue<string>(AddressField);
        }

        public bool UpdatedOrCreated { set; get; }
        public bool Deleted { set; get; }

        public bool IsLocallyModified
        {
            get { return UpdatedOrCreated || Deleted; }
        }

        public string FirstName { set; get; }
        public string LastName { set; get; }
        public string Title { set; get; }
        public string Phone { set; get; }
        public string Email { set; get; }
        public string Department { set; get; }
        public string Address { set; get; }

        public ContactObject Self
        {
            get { return this; }
        }

        public string SyncStatus
        {
            get
            {
                if (UpdatedOrCreated)
                {
                    return Unsynced;
                }
                else if (Deleted)
                {
                    return ToDelete;
                }
                else
                {
                    return Synced;
                }
            }
        }

        public string ContactName
        {
            get { return Name; }
            set { Name = value; }
        }

        public string TitleDept
        {
            get
            {
                if (String.IsNullOrEmpty(Title) && String.IsNullOrEmpty(Department))
                    return String.Empty;
                if (!String.IsNullOrEmpty(Title) && String.IsNullOrEmpty(Department))
                    return Title;
                if (String.IsNullOrEmpty(Title) && !String.IsNullOrEmpty(Department))
                    return Department;
                return Title + " / " + Department;
            }
        }

        public int CompareTo(object obj)
        {
            return Compare(obj);
        }
        public int CompareTo(ContactObject other)
        {
            return Compare(other);
        }

        private int Compare(object other)
        {
            var item1 = this;
            var item2 = other as ContactObject;
            if (item2 == null)
                return -1;
            int retVal = 0;
            if (item1.IsLocallyModified && !item2.IsLocallyModified)
                retVal -= 2;
            else if (!item1.IsLocallyModified && item2.IsLocallyModified)
                retVal += 2;
            retVal += String.Compare(item1.ContactName, item2.ContactName, StringComparison.CurrentCultureIgnoreCase);
            return retVal;
        }
    }
}