using Salesforce.SDK.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salesforce.Sample.RestExplorer.Phone
{
    public class Config
    {
        private const String LOGIN_URL = "https://test.salesforce.com";
        private const String CLIENT_ID = "3MVG92.uWdyphVj51aXI_W16JGEwzme6Hj3YodbDRU0FHC86IddELDABTLsS5HGHNzNN_vQTA_XDuL.QtdF7G";
        private const String CALLBACK_URL = "sfdc:///axm/detect/oauth/done";
        private static String[] SCOPES = new String[] { "api" };

        private static LoginOptions _loginOptions;
        public static LoginOptions LoginOptions
        {
            get
            {
                if (_loginOptions == null)
                {
                    _loginOptions = new LoginOptions(LOGIN_URL, CLIENT_ID, CALLBACK_URL, SCOPES);
                }
                return _loginOptions;
            }

        }
    }
}
