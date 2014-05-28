﻿using System;
using System.IO;
using System.Reflection;
using System.Windows;
using Windows.UI.Xaml;

namespace Salesforce.SDK.Hybrid
{
    /// <summary>
    /// Helper for reading files from resources.
    /// </summary>
    public class ConfigHelper
    {
        /// <summary>
        ///  Return string containing contents of resource file
        ///  Throws a FileNotFoundException if the file cannot be found
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ReadConfigFromResource(string path)
        {
            Assembly assembly = typeof(ConfigHelper).GetTypeInfo().Assembly;
            using (var resource = assembly.GetManifestResourceStream(path))
            {
                if (resource != null)
                {
                    using (var reader = new StreamReader(resource))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            throw new FileNotFoundException("Resource file not found", path);
        }
    }
}
