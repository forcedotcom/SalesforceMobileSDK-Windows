using Salesforce.SDK.Auth;
using Salesforce.SDK.Core;
using Salesforce.SDK.Logging;
using Salesforce.SDK.Security;
using Salesforce.SDK.Settings;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Salesforce.SDK.App
{
    public class ApplicationService : IApplicationInformationService
    {
        private static IEncryptionService EncryptionService => SDKServiceLocator.Get<IEncryptionService>();
        private static ILoggingService LoggingService => SDKServiceLocator.Get<ILoggingService>();
        private const string UserAgentHeaderFormat = "SalesforceMobileSDK/3.1 ({0}/{1} {2}) {3}";

        /// <summary>
        ///     Settings key for config.
        /// </summary>
        private const string ConfigSettings = "salesforceConfig";

        private const string DefaultServerPath = "Salesforce.SDK.Resources.servers.xml";

        public Task ClearConfigurationSettingsAsync()
        {
            ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;
            return Task.FromResult<bool>(settings.Values.Remove(ConfigSettings));
        }

        public async Task<bool> DoesFileExistAsync(string path)
        {
            IRandomAccessStreamWithContentType stream = null;
            bool fileExists = false;
            try
            {
                var folder = ApplicationData.Current.LocalFolder;
                var file = await folder.GetFileAsync(path);
                stream = await file.OpenReadAsync();
                fileExists = true;
            }
            catch (UnauthorizedAccessException)
            {
                fileExists = true;
            }
            catch (Exception)
            {
                fileExists = false;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Dispose();
                }
            }
            return fileExists;
        }

        public async Task<string> GenerateUserAgentHeaderAsync()
        {
            var appName = await GetApplicationDisplayNameAsync();
            PackageVersion packageVersion = Package.Current.Id.Version;
            string packageVersionString = packageVersion.Major + "." + packageVersion.Minor + "." +
                                          packageVersion.Build;
            var UserAgentHeader = String.Format(UserAgentHeaderFormat, appName,
            packageVersionString, "native", "");
            return UserAgentHeader;
        }

        public async Task<string> GetApplicationDisplayNameAsync()
        {
            string displayName = String.Empty;

            try
            {
                var config = SDKManager.ServerConfiguration;
                if (config == null)
                {
                    throw new Exception();
                }
                if (!String.IsNullOrWhiteSpace(config.ApplicationTitle))
                {
                    displayName = config.ApplicationTitle;
                }
                else
                {
                    //If no Application title is passed from the consumer of the SDK, fall back to display name from app manifest.
                    StorageFile file = await Package.Current.InstalledLocation.GetFileAsync("AppxManifest.xml");
                    string manifestXml = await FileIO.ReadTextAsync(file);
                    XDocument doc = XDocument.Parse(manifestXml);
                    XNamespace packageNamespace = "http://schemas.microsoft.com/appx/2010/manifest";
                    displayName = (from name in doc.Descendants(packageNamespace + "DisplayName")
                                   select name.Value).First();
                }
            }
            catch (Exception)
            {
                //If ApplicationTitle and Display Name both fail, fall back to Package Id
                LoggingService.Log("Error retrieving application name; using package id name instead", LoggingLevel.Warning);
                displayName = Package.Current.Id.Name;
            }
            return displayName;
        }

        public string GetApplicationLocalFolderPath()
        {
            return ApplicationData.Current.LocalFolder.Path;
        }

        public Task<string> GetConfigurationSettingsAsync()
        {
            ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;
            if (settings.Values.ContainsKey(ConfigSettings))
            {
                return Task.FromResult<string>(EncryptionService.Decrypt(settings.Values[ConfigSettings].ToString()));
            }
            return Task.FromResult<string>(String.Empty);
        }

        public async Task<string> ReadApplicationFileAsync(string path)
        {
            var fileUri = new Uri(@"ms-appx:///" + path);
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(fileUri);
            if (file != null)
            {
                Stream stream = await file.OpenStreamForReadAsync();
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
            throw new FileNotFoundException("Resource file not found", path);
        }

        public Task SaveConfigurationSettingsAsync(string config)
        {
            ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;
            settings.Values[ConfigSettings] = EncryptionService.Encrypt(config);
            return Task.FromResult<bool>(true);
        }
    }
}
