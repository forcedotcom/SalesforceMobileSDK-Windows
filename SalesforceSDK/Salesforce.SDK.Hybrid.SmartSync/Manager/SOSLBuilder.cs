using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Salesforce.SDK.Hybrid.SmartSync.Manager
{
    public sealed class SOSLBuilder
    {
        private static SDK.SmartSync.Manager.SOSLBuilder _soslBuilder;

        public static SOSLBuilder GetInstanceWithSearchTerm(string searchTerm)
        {
            _soslBuilder = SDK.SmartSync.Manager.SOSLBuilder.GetInstanceWithSearchTerm(searchTerm);
            var nativeBuilder = _soslBuilder;
            var hybridBuilder = JsonConvert.SerializeObject(nativeBuilder);
            return JsonConvert.DeserializeObject<SOSLBuilder>(hybridBuilder);
        }

        public SOSLBuilder SearchGroup(string searchGroup)
        {
            var nativeBuilder = _soslBuilder.SearchGroup(searchGroup);
            var hybridBuilder = JsonConvert.SerializeObject(nativeBuilder);
            return JsonConvert.DeserializeObject<SOSLBuilder>(hybridBuilder); 
        }

        public SOSLBuilder Returning(SOSLReturningBuilder returningSpec)
        {
            var returningSpecString = JsonConvert.SerializeObject(returningSpec);
            var nativeBuilder =
                _soslBuilder.Returning(
                    JsonConvert.DeserializeObject<SDK.SmartSync.Manager.SOSLReturningBuilder>(returningSpecString));
            var hybridBuilder = JsonConvert.SerializeObject(nativeBuilder);
            return JsonConvert.DeserializeObject<SOSLBuilder>(hybridBuilder);
        }

        public SOSLBuilder DivisionFilter(string filter)
        {
            var nativeBuilder = _soslBuilder.DivisionFilter(filter);
            var hybridBuilder = JsonConvert.SerializeObject(nativeBuilder);
            return JsonConvert.DeserializeObject<SOSLBuilder>(hybridBuilder);
        }

        public SOSLBuilder DataCategory(string dataCategory)
        {
            var nativeBuilder = _soslBuilder.DataCategory(dataCategory);
            var hybridBuilder = JsonConvert.SerializeObject(nativeBuilder);
            return JsonConvert.DeserializeObject<SOSLBuilder>(hybridBuilder);
        }

        public SOSLBuilder Limit(int limit)
        {
            var nativeBuilder = _soslBuilder.Limit(limit);
            var hybridBuilder = JsonConvert.SerializeObject(nativeBuilder);
            return JsonConvert.DeserializeObject<SOSLBuilder>(hybridBuilder);
        }

        public string BuildAndEncode()
        {
            return _soslBuilder.BuildAndEncode();
        }

        public string BuildAndEncodeWithPath(string path)
        {
            return _soslBuilder.BuildAndEncodeWithPath(path);
        }

        public string BuildWithPath(string path)
        {
            return _soslBuilder.BuildWithPath(path);
        }

        public string Build()
        {
            return _soslBuilder.Build();
        }
    }
}
