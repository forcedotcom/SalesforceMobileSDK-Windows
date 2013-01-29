using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Salesforce.SDK.Rest;

namespace Salesforce.Sample.RestExplorer.Phone
{
    public partial class DescribePage : PhoneApplicationPage
    {
        public DescribePage()
        {
            InitializeComponent();

            tbResult.IsReadOnly = true;
            btnDescribe.Click += OnDescribeClicked;
        }

        private void OnDescribeClicked(object sender, RoutedEventArgs e)
        {
            ClientManager cm = new ClientManager(((App)Application.Current).LoginOptions);
            RestClient rc = cm.GetRestClient();
            if (rc != null)
            {
                tbResult.Text = rc.SendSync(RestRequest.GetRequestForDescribe("v26.0", tbObjectType.Text)).AsString;
            }
        }
    }
}