// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Salesforce.Sample.SmartSyncExplorer.utilities;

namespace Salesforce.Sample.SmartSyncExplorer.Controls
{
    internal sealed partial class BusinessCard : UserControl
    {
        public static readonly DependencyProperty ContactProperty = DependencyProperty.Register("ContactName",
            typeof (string), typeof (BusinessCard), null);

        public static readonly DependencyProperty TitleDeptProperty = DependencyProperty.Register("TitleDept",
            typeof (string), typeof (BusinessCard), null);

        public static readonly DependencyProperty PhoneProperty = DependencyProperty.Register("Phone",
            typeof (string), typeof (BusinessCard), null);

        public static readonly DependencyProperty EmailProperty = DependencyProperty.Register("Email",
            typeof (string), typeof (BusinessCard), null);

        public static readonly DependencyProperty AddressProperty = DependencyProperty.Register("Address",
            typeof (string), typeof (BusinessCard), null);

        public static readonly DependencyProperty ContactIdProperty = DependencyProperty.Register("ContactId",
            typeof (string), typeof (BusinessCard), null);

        public static readonly DependencyProperty SyncStatusProperty = DependencyProperty.Register("SyncStatusIcon",
            typeof(string), typeof(BusinessCard), null);

        public static readonly DependencyProperty ContactObjectProperty = DependencyProperty.Register("Contact",
            typeof(ContactObject), typeof(BusinessCard), null);

        public BusinessCard()
        {
            InitializeComponent();
        }

        public string ContactName
        {
            get { return (string) GetValue(ContactProperty); }
            set { SetValue(ContactProperty, value); }
        }

        public string TitleDept
        {
            get { return (string) GetValue(TitleDeptProperty); }
            set { SetValue(TitleDeptProperty, value); }
        }

        public string Phone
        {
            get { return (string) GetValue(PhoneProperty); }
            set { SetValue(PhoneProperty, value); }
        }

        public string Email
        {
            get { return (string) GetValue(EmailProperty); }
            set { SetValue(EmailProperty, value); }
        }

        public string Address
        {
            get { return (string) GetValue(AddressProperty); }
            set { SetValue(AddressProperty, value); }
        }

        public string ContactId
        {
            get { return (string) GetValue(ContactIdProperty); }
            set { SetValue(ContactIdProperty, value); }
        }

        public string SyncStatusIcon
        {
            get { return (string)GetValue(SyncStatusProperty); }
            set { SetValue(SyncStatusProperty, value); }
        }

        public ContactObject Contact
        {
            get { return (ContactObject)GetValue(ContactObjectProperty); }
            set
            {
                SetValue(ContactObjectProperty, value);
                SyncStatusIcon = value.SyncStatus;
            }
        }
    }
}