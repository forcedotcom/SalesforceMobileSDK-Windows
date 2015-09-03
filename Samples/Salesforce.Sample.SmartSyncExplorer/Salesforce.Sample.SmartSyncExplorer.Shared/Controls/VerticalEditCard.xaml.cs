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
using Salesforce.Sample.SmartSyncExplorer.Shared.Pages;
using Salesforce.Sample.SmartSyncExplorer.utilities;

namespace Salesforce.Sample.SmartSyncExplorer.Controls
{
    public sealed partial class VerticalEditCard : UserControl
    {
        private ContactObject _contact;

        public VerticalEditCard()
        {
            this.InitializeComponent();
        }

        public ContactObject Contact
        {
            get { return _contact; }
            set
            {
                _contact = value;
                SetFields();
            }
        }

        public Flyout AttachedFlyout { get; set; }

        public string ContactName
        {
            get { return ContactNameBox.Text; }
            private set { ContactNameBox.Text = value; }
        }

        public string Title
        {
            get { return TitleBox.Text; }
            private set { TitleBox.Text = value; }
        }

        public string Department
        {
            get { return DepartmentBox.Text; }
            private set { DepartmentBox.Text = value; }
        }

        public string Phone
        {
            get { return PhoneBox.Text; }
            private set { PhoneBox.Text = value; }
        }

        public string Email
        {
            get { return EmailBox.Text; }
            private set { EmailBox.Text = value; }
        }

        public string Address
        {
            get { return AddressBox.Text; }
            private set { AddressBox.Text = value; }
        }

        private void SetFields()
        {
            if (_contact == null) return;
            ContactName = _contact.Name ?? String.Empty;
            Title = _contact.Title ?? String.Empty;
            Department = _contact.Department ?? String.Empty;
            Phone = _contact.Phone ?? String.Empty;
            Email = _contact.Email ?? String.Empty;
            Address = _contact.Address ?? String.Empty;
        }
        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            Contact.Address = Address;
            string name = ContactName.Trim();
            Contact.Name = ContactName;
            int index = name.IndexOf(" ", StringComparison.CurrentCultureIgnoreCase);
            if (index == -1)
            {
                Contact.FirstName = name;
                Contact.LastName = String.Empty;
            }
            else
            {
                Contact.FirstName = name.Substring(0, index).Trim();
                Contact.LastName = name.Substring(index, name.Length - index).Trim();
            }
            Contact.Title = Title;
            Contact.Department = Department;
            Contact.Phone = Phone;
            Contact.Email = Email;
            Contact.Address = Address;
            MainPage.ContactsDataModel.SaveContact(Contact, String.IsNullOrWhiteSpace(Contact.ObjectId));
            HideFlyout();
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            HideFlyout();
        }

        private void DeleteButton_OnClick(object sender, RoutedEventArgs e)
        {
            MainPage.ContactsDataModel.DeleteObject(Contact);
            HideFlyout();
        }

        private async void HideFlyout()
        {
            if (AttachedFlyout != null)
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => AttachedFlyout.Hide());
            }
        }

        public void DeleteVisible(bool visible)
        {
            if (visible)
            {
                DeleteButton.Visibility = Visibility.Visible;
            }
            else
            {
                DeleteButton.Visibility = Visibility.Collapsed;
            }
        }
    }
}
