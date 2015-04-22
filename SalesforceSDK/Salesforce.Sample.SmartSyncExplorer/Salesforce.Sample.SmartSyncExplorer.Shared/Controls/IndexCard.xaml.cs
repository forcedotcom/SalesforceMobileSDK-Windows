using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236
using Microsoft.VisualBasic.CompilerServices;
using Salesforce.Sample.SmartSyncExplorer.Shared.Pages;

namespace Salesforce.Sample.SmartSyncExplorer.Controls
{
    public sealed partial class IndexCard : UserControl
    {
        public static readonly DependencyProperty IndexStringProperty = DependencyProperty.Register("IndexString",
            typeof(string), typeof(IndexCard), null);

        public string IndexString
        {
            get { return (string)GetValue(IndexStringProperty); }
            set { SetValue(IndexStringProperty, value); }
        }

        public IndexCard()
        {
            this.InitializeComponent();
        }

        private void ReferenceClick(object sender, RoutedEventArgs e)
        {
            MainPage.ContactsDataModel.FilterUsesContains = false;
            var searchText = ReferenceName.Text.Trim();
            MainPage.ContactsDataModel.Filter = "all".Equals(searchText, StringComparison.CurrentCultureIgnoreCase) ? String.Empty : searchText;
            MainPage.MainPageReference.ZoomIn();
            MainPage.ContactsDataModel.RunFilter();
        }
    }
}
