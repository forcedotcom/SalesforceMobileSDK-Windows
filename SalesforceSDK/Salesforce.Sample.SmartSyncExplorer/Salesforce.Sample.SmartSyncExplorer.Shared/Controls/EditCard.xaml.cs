// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236
using System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Salesforce.Sample.SmartSyncExplorer.Shared.Pages;
using Salesforce.Sample.SmartSyncExplorer.utilities;

namespace Salesforce.Sample.SmartSyncExplorer.Controls
{
    public sealed partial class EditCard : UserControl
    {
        private ContactObject _contact;

        public EditCard()
        {
            InitializeComponent();
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