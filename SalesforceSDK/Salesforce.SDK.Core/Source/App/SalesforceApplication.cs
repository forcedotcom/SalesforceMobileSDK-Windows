using Salesforce.SDK.Adaptation;
using Salesforce.SDK.Auth;
using Salesforce.SDK.Rest;
using Salesforce.SDK.Source.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


namespace Salesforce.SDK.App
{
    public abstract class SalesforceApplication : Application
    {

        public static ClientManager GlobalClientManager { get; private set; }
        public static Type RootApplicationPage { get; private set; }
        public static SalesforceConfig ServerConfiguration { get; private set; }

        public SalesforceApplication() : base()
        {
            Suspending += OnSuspending;
            CreateClientManager(false);
            RootApplicationPage = SetRootApplicationPage();
        }

        /// <summary>
        /// Use this to initialize your custom SalesforceConfig source, and to set up the Encryptor to use your own app specific, unique salt, password, and key generator.
        /// An example of code that may go into this method would be as follows:
        /// 
        ///     protected override void InitializeConfig()
        ///     {
        ///         new Config();
        ///         EncryptionSettings settings = new EncryptionSettings(new HmacSHA256KeyGenerator())
        ///         {
        ///             Password = "mypassword",
        ///             Salt = "mysalt"
        ///         };
        ///         Encryptor.init(settings);
        ///     }
        /// </summary>
        ///
        protected abstract SalesforceConfig InitializeConfig();

        protected abstract Type SetRootApplicationPage();

        protected virtual async void OnSuspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            await SuspensionManager.SaveAsync();
            deferral.Complete();
        }

        protected async override void OnActivated(IActivatedEventArgs args)
        {
            if (ApplicationExecutionState.Terminated.Equals(args.PreviousExecutionState))
            {
                try
                {
                    await SuspensionManager.RestoreAsync();
                }
                catch (SuspensionManagerException)
                {
                    // Assume there is no state and continue
                }
            }
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.NavigationFailed += OnNavigationFailed;
        }

        private async void OnNavigationFailed(object sender, Windows.UI.Xaml.Navigation.NavigationFailedEventArgs args)
        {
            if (GlobalClientManager != null)
            {
                await GlobalClientManager.Logout();
            }
        }

        public static void ResetClientManager()
        {
            GlobalClientManager = new ClientManager();
        }

        protected void CreateClientManager(bool reset)
        {
            if (GlobalClientManager == null || reset)
            {
                ServerConfiguration = InitializeConfig();
                GlobalClientManager = new ClientManager();
            }

            GlobalClientManager.GetRestClient();
        }
    }
}
