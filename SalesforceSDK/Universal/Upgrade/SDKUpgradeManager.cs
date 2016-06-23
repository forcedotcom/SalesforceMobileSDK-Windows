using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography.Core;
using Windows.Storage;
using Salesforce.SDK.Auth;
using Salesforce.SDK.Security;
using Salesforce.SDK.Settings;

namespace Salesforce.SDK.Upgrade
{
    public class SDKUpgradeManager
    {
        private static SDKUpgradeManager _instance = null;

        public static SDKUpgradeManager GetInstance()
        {
            if (_instance == null)
            {
                _instance = new SDKUpgradeManager();
            }
            return _instance;
        }

        public async Task UpgradeAsync()
        {
            if (ApplicationData.Current.Version.Equals(0))
            {
                await UpgradeFromEarlierThan4Dot2();
            }
            await ApplicationData.Current.SetVersionAsync(VersionConvertion(SDKManager.SDK_VERSION), VersionRequestHandler);
        }

        private async Task UpgradeFromEarlierThan4Dot2()
        {
            Encryptor.init(new EncryptionSettings(new HmacSHA256KeyGenerator(HashAlgorithmNames.Md5)));
            var authHelper = new AuthHelper();
            var account = authHelper.RetrieveCurrentAccount();
            Encryptor.ChangeSettings(new EncryptionSettings(new HmacSHA256KeyGenerator(HashAlgorithmNames.Sha256)));
            await authHelper.PersistCurrentAccountAsync(account);
            await authHelper.PersistCurrentPincodeAsync(account);
        }

        private uint VersionConvertion(string version)
        {
            return UInt32.Parse(new string(version.Where(Char.IsDigit).ToArray()));
        }

        private void VersionRequestHandler(SetVersionRequest request)
        {
            var defer = request.GetDeferral();
            foreach (var item in ApplicationData.Current.LocalSettings.Values)
            {
                switch (item.Key)
                {
                    case "Name":
                        ApplicationData.Current.LocalSettings.Values["Name"] = string.Format("V{0}_{1}", request.DesiredVersion, item.Value);
                        break;

                    default:
                        ApplicationData.Current.LocalSettings.Values[item.Key] = string.Format("{0}", item.Value);
                        break;
                }
            }
            ApplicationData.Current.SignalDataChanged();
            defer.Complete();
        }
    }
}
