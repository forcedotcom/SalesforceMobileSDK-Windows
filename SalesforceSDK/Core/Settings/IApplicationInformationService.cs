using System.Threading.Tasks;

namespace Core.Settings
{
    public interface IApplicationInformationService
    {
        Task<string> GetApplicationDisplayNameAsync();

        Task<string> GenerateUserAgentHeaderAsync();

        Task<string> ReadApplicationFileAsync(string path);

        Task SaveConfigurationSettingsAsync(string config);

        Task<string> GetConfigurationSettingsAsync();

        Task ClearConfigurationSettingsAsync();
    }
}
