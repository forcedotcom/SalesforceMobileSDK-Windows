using Newtonsoft.Json;
using Salesforce.SDK.Auth;
using Salesforce.SDK.Hybrid;
using Salesforce.SDK.Source.Security;
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
using Windows.Storage;
using System.Collections.ObjectModel;

namespace Salesforce.SDK.Source.Settings
{
    public abstract class SalesforceConfig
    {

        #region Private fields
        /// <summary>
        /// Path to the server.xml that comes with the SDK.
        /// </summary>
        private readonly string ServerFilePath = "Salesforce.SDK.Resources.servers.xml";
        /// <summary>
        /// Settings key for config.
        /// </summary>
        private const string CONFIG_SETTINGS = "salesforceConfig";
        #endregion

        #region Public properties & fields
        /// <summary>
        /// Property that provides a list of all the servers currently in use by the app - built in and added by user.
        /// </summary>
        public ObservableCollection<ServerSetting> ServerList { set; get; }
        /// <summary>
        /// Property that provides the currently selected server, name and host.
        /// </summary>
        public ServerSetting Server
        {
            get
            {
                return ServerList[SelectedServer];
            }
        }
        #endregion

        #region Static fields
        /// <summary>
        /// Globally accessible login options; these are the current login settings that will be used when oauth2 is launched.
        /// </summary>
        public static LoginOptions LoginOptions { set; get; }
        #endregion

        #region Protected
        // The currently selected server.
        protected int SelectedServer;
        #endregion
        /// <summary>
        /// Implement this to define your client ID for oauth.  This should match your application settings generated in Salesforce.
        /// </summary>
        public abstract string ClientId { get; }
        /// <summary>
        /// Implement to define your callback url when oauth authentication is complete.  This should match your application settings generated in Salesforce.
        /// </summary>
        public abstract string CallbackUrl { get; }
        /// <summary>
        /// Implement to define the scopes your app will use such as web or api. 
        /// </summary>
        public abstract string[] Scopes { get; }



        public SalesforceConfig()
        {
            ApplicationDataContainer settings = ApplicationData.Current.RoamingSettings;
            string configJson = settings.Values[CONFIG_SETTINGS] as string;
            if (String.IsNullOrWhiteSpace(configJson))
            {
                SetupServers();
                SaveConfig();
            }
        }

        public void SaveConfig()
        {
            ApplicationDataContainer settings = ApplicationData.Current.RoamingSettings;
            SalesforceConfig.LoginOptions = new LoginOptions(Server.ServerHost, ClientId, CallbackUrl, Scopes);
            String configJson = JsonConvert.SerializeObject(this);
            settings.Values[CONFIG_SETTINGS] = Encryptor.Encrypt(configJson);
        }

        public void SetSelectedServer(int index)
        {
            if (index >= 0 && index < ServerList.Count)
            {
                SelectedServer = index;
                SaveConfig();
            }
        }

        public void AddServer(ServerSetting server)
        {
            if (!String.IsNullOrWhiteSpace(server.ServerName) && !String.IsNullOrWhiteSpace(server.ServerHost))
            {
                ServerSetting old = ServerList.FirstOrDefault(item => item.ServerHost.Equals(server.ServerHost, StringComparison.CurrentCultureIgnoreCase));
                if (old != null)
                {
                    old.ServerHost = server.ServerHost;
                } else
                {
                    ServerList.Add(server);
                }
                SaveConfig();
            }
        }

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
            ServerList = new ObservableCollection<ServerSetting>(data);
            SelectedServer = 0;
        }

        public static T RetrieveConfig<T>() where T : SalesforceConfig
        {
            ApplicationDataContainer settings = ApplicationData.Current.RoamingSettings;
            string configJson = settings.Values[CONFIG_SETTINGS] as string;
            if (String.IsNullOrWhiteSpace(configJson))
                return null;
            return JsonConvert.DeserializeObject<T>(Encryptor.Decrypt(configJson));
        }
    }
}
