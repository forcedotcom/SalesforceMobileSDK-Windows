using Salesforce.Sample.ContactExplorer.Pages;
using Salesforce.Sample.ContactExplorer.Settings;
using Salesforce.SDK.App;
using Salesforce.SDK.Source.Security;
using Salesforce.SDK.Source.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227

namespace Salesforce.Sample.ContactExplorer
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : SalesforceApplication
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App() : base()
        {
            this.InitializeComponent();
        }

        protected override Salesforce.SDK.Source.Settings.SalesforceConfig InitializeConfig()
        {
            EncryptionSettings settings = new EncryptionSettings(new HmacSHA256KeyGenerator())
            {
                Password = "supercalifragilisticexpialidocious",
                Salt = "friesaresaltyforsure"
            };
            Encryptor.init(settings);
            Config config = SalesforceConfig.RetrieveConfig<Config>();
            if (config == null)
                config = new Config();
            return config;
        }

        protected override Type SetRootApplicationPage()
        {
            return typeof(MainPage);
        }
    }
}