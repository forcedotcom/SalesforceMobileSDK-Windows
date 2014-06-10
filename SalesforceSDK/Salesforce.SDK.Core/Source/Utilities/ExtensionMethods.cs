using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Salesforce.SDK.Source.Utilities
{
    public static class ExtensionMethods
    {
        private static readonly Regex QUERY_PARAMS = new Regex(@"[?|&](\w+)=([^?|^&]+)");

        public static Dictionary<string, string> ParseQueryString(this string queryString)
        {
            var match = QUERY_PARAMS.Match(queryString);
            Dictionary<string, string> results = new Dictionary<string, string>();
            while (match.Success)
            {
                results.Add(match.Groups[1].Value, match.Groups[2].Value);
                match = match.NextMatch();
            }
            return results;
        }

        public static Dictionary<string, string> ParseQueryString(this Uri uri)
        {
            return ParseQueryString(uri.PathAndQuery);
        }
    }
}
