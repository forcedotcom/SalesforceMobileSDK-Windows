using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salesforce.SDK.Hybrid.SmartStore
{
    public sealed class SmartStoreType
    {
        private SDK.SmartStore.Store.SmartStoreType _smartStoreType;

        private const string SmartTypeInteger = "integer";
        private const string SmartTypeString = "string";
        private const string SmartTypeFloating = "floating";

        private static readonly SmartStoreType _smartInteger = new SmartStoreType(SmartTypeInteger);
        private static readonly SmartStoreType _smartString = new SmartStoreType(SmartTypeString);
        private static readonly SmartStoreType _smartFloating = new SmartStoreType(SmartTypeFloating);

        public static SmartStoreType SmartInteger
        {
            get { return _smartInteger; }
        }

        public static SmartStoreType SmartString
        {
            get { return _smartString; }
        }

        public static SmartStoreType SmartFloating
        {
            get { return _smartFloating; }
        }

        public SmartStoreType()
        {
            _smartStoreType = new SDK.SmartStore.Store.SmartStoreType(SmartTypeString);
        }

        public SmartStoreType(string columnType)
        {
            _smartStoreType = new SDK.SmartStore.Store.SmartStoreType(columnType);
        }
    }
}
