using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Newtonsoft.Json;
using Salesforce.SDK.Auth;

namespace Salesforce.SDK.Hybrid.SmartSync.Manager
{
    public sealed class SOQLBuilder
    {
        private static SDK.SmartSync.Manager.SOQLBuilder _soqlBuilder;
        
        [DefaultOverload]
        public static SOQLBuilder GetInstanceWithFields(string fields)
        {
            var builder =  SDK.SmartSync.Manager.SOQLBuilder.GetInstanceWithFields(fields);
            _soqlBuilder = builder;
            var hybridBuilder = JsonConvert.SerializeObject(builder);
            return JsonConvert.DeserializeObject<SOQLBuilder>(hybridBuilder);
        }

        public static SOQLBuilder GetInstanceWithFields([ReadOnlyArray()] string[] fields)
        {
            var builder = SDK.SmartSync.Manager.SOQLBuilder.GetInstanceWithFields(fields);
            var hybridBuilder = JsonConvert.SerializeObject(builder);
            return JsonConvert.DeserializeObject<SOQLBuilder>(hybridBuilder);
        }

        public SOQLBuilder Fields(string fields)
        {
            var nativeFields = _soqlBuilder.Fields(fields);
            var hybridFields = JsonConvert.SerializeObject(nativeFields);
            return JsonConvert.DeserializeObject<SOQLBuilder>(hybridFields);
        }

        public SOQLBuilder From(string from)
        {
            var nativeFrom = _soqlBuilder.From(from);
            var hybridFrom = JsonConvert.SerializeObject(nativeFrom);
            return JsonConvert.DeserializeObject<SOQLBuilder>(hybridFrom);
        }

        public SOQLBuilder Where(string where)
        {
            var nativeWhere = _soqlBuilder.Where(where);
            var hybridWhere = JsonConvert.SerializeObject(nativeWhere);
            return JsonConvert.DeserializeObject<SOQLBuilder>(hybridWhere);
        }

        public SOQLBuilder With(string with)
        {
            var nativeWith = _soqlBuilder.With(with);
            var hybridWith = JsonConvert.SerializeObject(nativeWith);
            return JsonConvert.DeserializeObject<SOQLBuilder>(hybridWith);
        }

        public SOQLBuilder GroupBy(string groupBy)
        {
            var nativeGroupBy = _soqlBuilder.GroupBy(groupBy);
            var hybridGroupBy = JsonConvert.SerializeObject(nativeGroupBy);
            return JsonConvert.DeserializeObject<SOQLBuilder>(hybridGroupBy);
        }

        public SOQLBuilder Having(string having)
        {
            var nativeHaving = _soqlBuilder.Having(having);
            var hybridHaving = JsonConvert.SerializeObject(nativeHaving);
            return JsonConvert.DeserializeObject<SOQLBuilder>(hybridHaving);
        }

        public SOQLBuilder OrderBy(string orderBy)
        {
            var nativeOrderBy = _soqlBuilder.OrderBy(orderBy);
            var hybridOrderBy = JsonConvert.SerializeObject(nativeOrderBy);
            return JsonConvert.DeserializeObject<SOQLBuilder>(hybridOrderBy);
        }

        public SOQLBuilder Limit(int limit)
        {
            var nativeLimit = _soqlBuilder.Limit(limit);
            var hybridLimit = JsonConvert.SerializeObject(nativeLimit);
            return JsonConvert.DeserializeObject<SOQLBuilder>(hybridLimit);
        }

        public SOQLBuilder Offset(int offset)
        {
            var nativeOffset = _soqlBuilder.Limit(offset);
            var hybridOffset = JsonConvert.SerializeObject(nativeOffset);
            return JsonConvert.DeserializeObject<SOQLBuilder>(hybridOffset);
        }

        public string BuildAndEncode()
        {
            return _soqlBuilder.BuildAndEncode();
        }

        public string BuildAndEncodeWithPath(string path)
        {
            return _soqlBuilder.BuildAndEncodeWithPath(path);
        }

        public string BuildWithPath(string path)
        {
            return _soqlBuilder.BuildWithPath(path);
        }

        public string Build()
        {
            return _soqlBuilder.Build();
        }
    }
}
