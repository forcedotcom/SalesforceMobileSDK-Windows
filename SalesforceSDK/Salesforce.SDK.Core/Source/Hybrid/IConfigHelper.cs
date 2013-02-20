
namespace Salesforce.SDK.Hybrid
{
    /// <summary>
    /// Interface for config related operations that are implemented in the platform specific assemblies
    /// </summary>
    public interface IConfigHelper
    {
        /// <summary>
        /// Return string containing contents of resource file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        string ReadConfigFromResource(string path);
    }
}
