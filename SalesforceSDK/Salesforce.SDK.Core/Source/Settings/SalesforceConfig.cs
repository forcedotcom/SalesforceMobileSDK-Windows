/*
 * Copyright (c) 2014, salesforce.com, inc.
 * All rights reserved.
 * Redistribution and use of this software in source and binary forms, with or
 * without modification, are permitted provided that the following conditions
 * are met:
 * - Redistributions of source code must retain the above copyright notice, this
 * list of conditions and the following disclaimer.
 * - Redistributions in binary form must reproduce the above copyright notice,
 * this list of conditions and the following disclaimer in the documentation
 * and/or other materials provided with the distribution.
 * - Neither the name of salesforce.com, inc. nor the names of its contributors
 * may be used to endorse or promote products derived from this software without
 * specific prior written permission of salesforce.com, inc.
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 */

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
        public virtual string ServerFilePath
        {
            get
            {
                return "Salesforce.SDK.Resources.servers.xml";
            }
        }

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
        public bool AllowNewConnections { get; set; }
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
            ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;
            string configJson = settings.Values[CONFIG_SETTINGS] as string;
            if (String.IsNullOrWhiteSpace(configJson))
            {
                SetupServers();
                SaveConfig();
            }
        }

        public void SaveConfig()
        {
            ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;
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

        /// <summary>
        /// Read the config file and load up the server information.  Uses the xml path from ServerFilePath, please see Salesforce.SDK.Resources.servers.xml for the default values to be used.
        /// The account dialog is affected by what is set in the serves.xml file.  The attribute allowNewConnections on the servers node will dictate if the "add connection" button is visible or not
        /// in the account creation UI.  If there is only a single server, and new connections is disabled, add account will immediately go to oauth.
        /// </summary>
        protected void SetupServers()
        {
            String xml = ConfigHelper.ReadConfigFromResource(ServerFilePath);
            XDocument servers = XDocument.Parse(xml);
            var connectionCheck = servers.Element("servers").Attribute("allowNewConnections");
            try
            {
                AllowNewConnections = connectionCheck == null || (bool)connectionCheck;
            } catch (FormatException)
            {
                AllowNewConnections = true;
            }
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
            ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;
            string configJson = settings.Values[CONFIG_SETTINGS] as string;
            if (String.IsNullOrWhiteSpace(configJson))
                return null;
            return JsonConvert.DeserializeObject<T>(Encryptor.Decrypt(configJson));
        }
    }
}
