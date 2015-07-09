using Salesforce.Sample.NativeSmartStoreSample.Shared.Pages;
using Salesforce.SDK.App;
using Salesforce.SDK.Auth;
using Salesforce.SDK.Source.Security;
using System;
using Windows.UI.Xaml.Navigation;
using Salesforce.Sample.NativeSmartStore.Settings;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=402347&clcid=0x409

namespace Salesforce.Sample.NativeSmartStore
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : SalesforceApplication
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
            : base()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// InitializeConfig should implement the commented out code. You should come up with your own, unique password and salt and for added security
        /// you should implement your own key generator using the IKeyGenerator interface.  
        /// </summary>
        /// <returns></returns>
        protected override async void InitializeConfig()
        {
            var config = await SDKManager.InitializeConfigAsync<Config>(new EncryptionSettings(new HmacSHA256KeyGenerator()));
            config.SaveConfig();
        }

        /// <summary>
        /// This returns the root application of your application. Please adjust to match your actual root page if you use something different.
        /// </summary>
        /// <returns></returns>
        protected override Type SetRootApplicationPage()
        {
            return typeof(MainPage);
        }
    }
}
