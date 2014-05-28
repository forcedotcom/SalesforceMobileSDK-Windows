using Salesforce.SDK.Auth;
using Salesforce.SDK.Hybrid;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Resources.Core;

namespace Salesforce.SDK.Source.Settings
{
    public abstract class SalesforceConfig
    {
        public ServerSetting[] ServerList { set;  get; }
        public ServerSetting Server
        {
            get
            {
                return ServerList[SelectedServer];
            }
        }
        public abstract string ClientId { get; }
        public abstract string CallbackUrl { get; }
        public abstract string[] Scopes { get; }

        protected int SelectedServer;

        private readonly string ServerFilePath = "Salesforce.SDK.Resources.servers.xml";
        public SalesforceConfig()
        {
            SetupServers();
        }

        public static LoginOptions LoginOptions { set; get; }

        protected virtual void SetupServers()
        {
            String xml = ConfigHelper.ReadConfigFromResource(ServerFilePath);
            XDocument servers = XDocument.Parse(xml);
            var data = from query in servers.Descendants("server")
                       select new ServerSetting
                       {
                           ServerName = (string)query.Attribute("name"),
                           ServerHost = (string)query.Attribute("url")
                       };
            ServerList = data.ToArray();
            SelectedServer = 0;
        } 
    }
}
