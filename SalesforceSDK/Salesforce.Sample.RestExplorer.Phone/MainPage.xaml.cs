using Microsoft.Phone.Controls;
using Salesforce.SDK.Auth;
using Salesforce.SDK.Rest;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Salesforce.Sample.RestExplorer.Phone
{
    public partial class MainPage : PhoneApplicationPage
    {
        private Button[] _buttons;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            _buttons = new Button[] { btnVersions, btnResources, btnDescribeGlobal, btnDescribe, btnMetadata, btnCreate, btnRetrieve, btnUpdate, btnUpsert, btnDelete, btnQuery, btnSearch, btnManual, btnLogout };

            foreach (Button button in _buttons)
            {
                button.Click += OnAnyButtonClicked;
            }
        }

        private void OnAnyButtonClicked(object sender, RoutedEventArgs e)
        {
            if (sender == btnDescribe)
            {
                NavigationService.Navigate(new Uri("/DescribePage.xaml", UriKind.Relative));
            }
            else 
            {
                MessageBox.Show(((Button)sender).Name);
            }
        }

        // Load data for the ViewModel Items
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //ClientManager cm = new ClientManager(((App)Application.Current).LoginOptions);
            //RestClient rc = cm.GetRestClient();
            //if (rc != null)
            //{
            //    System.Diagnostics.Debug.WriteLine("Logged in!");
            //}
        }
    }
}