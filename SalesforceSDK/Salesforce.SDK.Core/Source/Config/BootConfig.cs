using Newtonsoft.Json;
using Salesforce.SDK.Adaptation;
using System;

namespace Salesforce.SDK.Source.Config
{
    /// <summary>
    /// Object representing application configuration (read from www/bootconfig.json)
    /// </summary>
    public class BootConfig
    {
        private const String BOOTCONFIG_JSON = "www\\bootconfig.json";
        private static BootConfig _instance = null;

        /// <summary>
        /// OAuth client id
        /// </summary>
        [JsonProperty(PropertyName = "remoteAccessConsumerKey")]
        public string ClientId { get; set; }

        /// <summary>
        /// Oauth callback url
        /// </summary>
        [JsonProperty(PropertyName = "oauthRedirectURI")]
        public string CallbackURL { get; set; }

        /// <summary>
        /// OAuth scopes
        /// </summary>
        [JsonProperty(PropertyName = "oauthScopes")]
        public string[] Scopes { get; set; }

        /// <summary>
        /// True for hybrid local application (meaning the html/js/css is bundled in the application)
        /// False for hybrid remote application (menaing the html/js/css is served from a server)
        /// </summary>
        [JsonProperty(PropertyName = "isLocal")]
        public bool IsLocal { get; set; }

        /// <summary>
        /// Start page
        /// </summary>
        [JsonProperty(PropertyName = "startPage")]
        public string StartPage { get; set; }

        /// <summary>
        /// Error page
        /// </summary>
        [JsonProperty(PropertyName = "errorPage")]
        public string ErrorPage { get; set; }

        /// <summary>
        /// When true, authentication is attempted when application first load
        /// </summary>
        [JsonProperty(PropertyName = "shouldAuthenticate")]
        public bool ShouldAuthenticate { get; set; }

        /// <summary>
        /// When true and offline, application tries to load from cache
        /// </summary>
        [JsonProperty(PropertyName = "attemptOfflineLoad")]
        public bool AttemptOfflineLoad { get; set; }

        
        // Return  the singleton instance (build it the first time it is called)
        public static BootConfig GetBootConfig()
        {
            if (_instance == null)
            {
                String configStr = PlatformAdapter.Resolve<IConfigHelper>().ReadConfigFromResource(BOOTCONFIG_JSON);
                _instance = JsonConvert.DeserializeObject<BootConfig>(configStr);
            }
            return _instance;
        }
    }
}
