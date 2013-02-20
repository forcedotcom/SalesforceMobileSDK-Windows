using System;
using System.IO;
using System.Windows;
using System.Windows.Resources;

namespace Salesforce.SDK.Source.Config
{
    /// <summary>
    /// Phone specific implementation if IConfigHelper
    /// </summary>
    public class ConfigHelper : IConfigHelper
    {
        /// <summary>
        ///  Return string containing contents of resource file
        ///  Throws a FileNotFoundException if the file cannot be found
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string ReadConfigFromResource(string path)
        {
            StreamResourceInfo streamInfo = Application.GetResourceStream(new Uri(path, UriKind.Relative));
            if (streamInfo != null)
            {
                StreamReader reader = new StreamReader(streamInfo.Stream);
                return reader.ReadToEnd();
            }
            throw new FileNotFoundException("Resource file not found", path);
        }
    }
}
