using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salesforce.SDK.Source.Settings
{
    public class ServerSetting
    {
        public static readonly string HTTPS_SCHEME = "https://";
        private string serverName;

        public string ServerName
        {
            set
            {
                serverName = value.Trim();
            }
            get
            {
                return serverName;
            }
        }
        private string serverHost;
        public string ServerHost
        {
            set
            {
                serverHost = value.Trim();
                if (!Uri.IsWellFormedUriString(serverHost, System.UriKind.Absolute))
                {
                    serverHost = HTTPS_SCHEME + serverHost;
                }
            }
            get
            {
                return serverHost;
            }
        }

        public override string ToString()
        {
            return ServerName + " - " + ServerHost;
        }
    }
}
