using Salesforce.SDK.Adaptation;
using Salesforce.SDK.Auth;
using Salesforce.SDK.Source.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Authentication.Web;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Salesforce.SDK.Source.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SalesforceLoginPage : Page, IWebAuthenticationContinuable
    {
        public SalesforceLoginPage()
        {
            this.InitializeComponent();
            Loaded += SalesforceLoginPage_Loaded;
        }

        async void SalesforceLoginPage_Loaded(object sender, RoutedEventArgs e)
        {
            await Frame.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
               {
                   StartLoginFlow(SalesforceConfig.LoginOptions);
               });
        }

        public void StartLoginFlow(LoginOptions loginOptions)
        {
            Uri loginUri = new Uri(OAuth2.ComputeAuthorizationUrl(loginOptions));
            Uri callbackUri = new Uri(loginOptions.CallbackUrl);
            WebAuthenticationBroker.AuthenticateAndContinue(loginUri, callbackUri, null, WebAuthenticationOptions.None);
        }


        public void ContinueWebAuthentication(WebAuthenticationBrokerContinuationEventArgs args)
        {
            var webResult = args.WebAuthenticationResult;
            if (webResult.ResponseStatus == WebAuthenticationStatus.Success)
            {
                Uri responseUri = new Uri(webResult.ResponseData.ToString());
                AuthResponse authResponse = OAuth2.ParseFragment(responseUri.Fragment.Substring(1));
                PlatformAdapter.Resolve<IAuthHelper>().EndLoginFlow(SalesforceConfig.LoginOptions, authResponse);
            }
        }
    }
}
