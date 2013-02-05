using Microsoft.Phone.Controls;
using Salesforce.SDK.Rest;
using System.Windows;

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
                RestRequest request = RestRequest.GetRequestForDescribe("v26.0", tbObjectType.Text);
                AsyncRequestCallback callback = (response) => { tbResult.Text = response.AsString; };
                rc.SendAsync(request, (response) => Dispatcher.BeginInvoke(() => { callback(response); }));
            }
        }
    }
}