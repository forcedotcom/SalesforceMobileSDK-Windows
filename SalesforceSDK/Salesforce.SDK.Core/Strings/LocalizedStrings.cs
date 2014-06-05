using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Data;

namespace Salesforce.SDK.Strings
{
    public class LocalizedStrings : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return GetString(value as string);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        public static string GetString(string resourceName)
        {
            ResourceLoader loader = ResourceLoader.GetForCurrentView("Salesforce.SDK.Core/Resources");
            return loader.GetString(resourceName);
        }
    }
}
