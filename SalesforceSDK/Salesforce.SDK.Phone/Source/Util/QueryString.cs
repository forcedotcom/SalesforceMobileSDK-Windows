using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salesforce.SDK.Source.Util
{
    /// <summary>
    /// Copied from http://dotnetautor.de/blog/2012/06/25/WindowsPhoneParseQueryString.aspx
    /// </summary>
    public class QueryString
    {
        private readonly Dictionary<string, string> _parameters;

        public QueryString()
        {
            _parameters = new Dictionary<string, string>();
        }

        public QueryString(Dictionary<string, string> parameters)
        {
            _parameters = parameters;
        }


        public QueryString(Uri uri) : this(uri.ToString()) { }

        public QueryString(string uri)
            : this()
        {
            Append(uri);
        }

        public void Append(string uri)
        {
            var query = (uri.IndexOf('?') > -1) ? uri.Substring(uri.IndexOf('?') + 1) : uri;
            var parts = query.Split('&');
            foreach (var data in parts.Select(s => s.Split('=')))
            {
                _parameters.Add(data[0], data[1]);
            }
        }

        public string this[string index]
        {
            get { return (_parameters.ContainsKey(index)) ? _parameters[index] : null; }
            set
            {
                if (_parameters.ContainsKey(index))
                {
                    _parameters[index] = value;
                }
                else
                {
                    _parameters.Add(index, value);
                }
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var parameter in _parameters)
            {
                if (sb.Length > 0) sb.Append('&');
                sb.AppendFormat("{0}={1}", parameter.Key, parameter.Value);
            }
            return sb.ToString();
        }
    }
}
