using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography.Core;
using Salesforce.SDK.Auth;
using Salesforce.SDK.Security;
using Salesforce.SDK.Settings;

namespace Salesforce.SDK.Upgrade
{
    public class SalesforceConfigUpgradeManager
    {
        private static SalesforceConfigUpgradeManager _instance = null;

        public static SalesforceConfigUpgradeManager GetInstance()
        {
            if (_instance == null)
            {
                _instance = new SalesforceConfigUpgradeManager();
            }
            return _instance;
        }

        public Task UpgradeSettingsAsync()
        {
            if (string.Equals(SDKManager.SDK_VERSION, "4.2.0"))
            {
                Encryptor.init(new EncryptionSettings(new HmacSHA256KeyGenerator(HashAlgorithmNames.Md5)));
                var config = SDKManager.InitializeConfigAsync<SalesforceConfig>().Result;
                Encryptor.ChangeSettings(new EncryptionSettings(new HmacSHA256KeyGenerator(HashAlgorithmNames.Sha256)));
                return config.SaveConfigAsync();
            }
            return Task.CompletedTask;
        }
    }
}
