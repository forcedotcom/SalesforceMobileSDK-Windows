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

        public SmartStoreType(string columnType)
        {
            _smartStoreType = new SDK.SmartStore.Store.SmartStoreType(columnType);
        }
    }
}
