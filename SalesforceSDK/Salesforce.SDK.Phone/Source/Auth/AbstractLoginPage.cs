using Microsoft.Phone.Controls;
using Salesforce.SDK.Adaptation;
using System;
using System.Collections.Generic;
using System.Net;
using System.Windows.Navigation;

namespace Salesforce.SDK.Auth
{
    public class AbstractLoginPage : PhoneApplicationPage
    {
        public virtual WebBrowser WebViewControl() { return null; }

        private LoginOptions _loginOptions;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            IDictionary<String, String> qs = NavigationContext.QueryString;
            _loginOptions = new LoginOptions(qs[AuthHelper.LOGIN_SERVER], qs[AuthHelper.CLIENT_ID], qs[AuthHelper.CALLBACK_URL], qs[AuthHelper.SCOPES].Split(' '));
            Uri loginUri = new Uri(OAuth2.ComputeAuthorizationUrl(_loginOptions));
            removeCookies(loginUri);
            WebViewControl().Source = loginUri;
            WebViewControl().Navigating += OnNavigating;
        }

        private void removeCookies(Uri uri)
        {
            HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(uri);
            CookieContainer cookieContainer = new CookieContainer();
            webRequest.CookieContainer = cookieContainer;
            foreach (Cookie cookie in cookieContainer.GetCookies(uri))
            {
                cookie.Discard = true;
                cookie.Expired = true;
            }
        }

        private void OnNavigating(object sender, NavigatingEventArgs e)
        {
            if (e.Uri.ToString().StartsWith(_loginOptions.CallbackUrl) && e.Uri.Fragment.Length > 0)
            {
                e.Cancel = true;
                AuthResponse authResponse = OAuth2.ParseFragment(e.Uri.Fragment.Substring(1));
                PlatformAdapter.Resolve<IAuthHelper>().EndLoginFlow(_loginOptions, authResponse);
            }
        }
    }

}