using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Salesforce.SDK.Hybrid.SmartSync.Manager
{
    public sealed class SOSLReturningBuilder
    {
        private static SDK.SmartSync.Manager.SOSLReturningBuilder _soslReturningBuilder;

        public static SOSLReturningBuilder GetInstanceWithObjectName(string name)
        {
            _soslReturningBuilder = SDK.SmartSync.Manager.SOSLReturningBuilder.GetInstanceWithObjectName(name);
            var nativeBuilder = _soslReturningBuilder;
            var hybridBuilder = JsonConvert.SerializeObject(nativeBuilder);
            return JsonConvert.DeserializeObject<SOSLReturningBuilder>(hybridBuilder);
        }

        public SOSLReturningBuilder Fields(string fields)
        {
            var nativeBuilder = _soslReturningBuilder.Fields(fields);
            var hybridBuilder = JsonConvert.SerializeObject(nativeBuilder);
            return JsonConvert.DeserializeObject<SOSLReturningBuilder>(hybridBuilder); 
        }

        public SOSLReturningBuilder Where(string where)
        {
            var nativeBuilder = _soslReturningBuilder.Where(where);
            var hybridBuilder = JsonConvert.SerializeObject(nativeBuilder);
            return JsonConvert.DeserializeObject<SOSLReturningBuilder>(hybridBuilder);
        }

        public SOSLReturningBuilder OrderBy(string orderBy)
        {
            var nativeBuilder = _soslReturningBuilder.OrderBy(orderBy);
            var hybridBuilder = JsonConvert.SerializeObject(nativeBuilder);
            return JsonConvert.DeserializeObject<SOSLReturningBuilder>(hybridBuilder);
        }

        public SOSLReturningBuilder ObjectName(string objectName)
        {
            var nativeBuilder = _soslReturningBuilder.ObjectName(objectName);
            var hybridBuilder = JsonConvert.SerializeObject(nativeBuilder);
            return JsonConvert.DeserializeObject<SOSLReturningBuilder>(hybridBuilder);
        }

        public SOSLReturningBuilder Limit(int limit)
        {
            var nativeBuilder = _soslReturningBuilder.Limit(limit);
            var hybridBuilder = JsonConvert.SerializeObject(nativeBuilder);
            return JsonConvert.DeserializeObject<SOSLReturningBuilder>(hybridBuilder);
        }

        public SOSLReturningBuilder WithNetwork(string withNetwork)
        {
            var nativeBuilder = _soslReturningBuilder.WithNetwork(withNetwork);
            var hybridBuilder = JsonConvert.SerializeObject(nativeBuilder);
            return JsonConvert.DeserializeObject<SOSLReturningBuilder>(hybridBuilder);
        }

        public string Build()
        {
            return _soslReturningBuilder.Build();
        }
    }
}
