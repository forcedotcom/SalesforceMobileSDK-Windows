using Microsoft.Phone.Controls;
using Salesforce.SDK.Adaptation;
using Salesforce.SDK.Auth;
using Salesforce.SDK.Rest;
using System;
using System.Collections.Generic;
using System.Net;
using System.Windows.Navigation;

namespace Salesforce.SDK.Auth
{
    public partial class LoginPage : AbstractLoginPage
    {
        public override WebBrowser WebViewControl() { return wbLogin; }
        
        public LoginPage()
        {
            InitializeComponent();
        }
    }
}