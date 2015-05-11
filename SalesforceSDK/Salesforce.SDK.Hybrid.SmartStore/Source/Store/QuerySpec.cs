using Newtonsoft.Json;
using Salesforce.SDK.SmartStore.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salesforce.SDK.Hybrid.SmartStore
{
    public enum SmartQueryType
    {
        Smart,
        Exact,
        Range,
        Like
    };

    public enum SqlOrder
    {
        ASC,
        DESC
    };
    public sealed class QuerySpec
    {
        public static QuerySpec BuildAllQuerySpec(string soupName, string path, SqlOrder order, int pageSize)
        {
            var sqlOrder = JsonConvert.SerializeObject(order);
            var nativeQuerySpec = SDK.SmartStore.Store.QuerySpec.BuildAllQuerySpec(soupName, path, JsonConvert.DeserializeObject<SDK.SmartStore.Store.QuerySpec.SqlOrder>(sqlOrder), pageSize);
            var querySpec = JsonConvert.SerializeObject(nativeQuerySpec);
            return JsonConvert.DeserializeObject<QuerySpec>(querySpec);
        }

        public static QuerySpec BuildExactQuerySpec(string soupName, string path, string exactMatchKey, int pageSize)
        {
            var nativeQuerySpec = SDK.SmartStore.Store.QuerySpec.BuildExactQuerySpec(soupName, path, exactMatchKey,
                pageSize);
            var querySpec = JsonConvert.SerializeObject(nativeQuerySpec);
            return JsonConvert.DeserializeObject<QuerySpec>(querySpec);
        }

        public static QuerySpec BuildRangeQuerySpec(string soupName, string path, string beginKey, string endKey,
            SqlOrder order, int pageSize)
        {
            var sqlOrder = JsonConvert.SerializeObject(order);
            var nativeQuerySpec = SDK.SmartStore.Store.QuerySpec.BuildRangeQuerySpec(soupName, path, beginKey, endKey, JsonConvert.DeserializeObject<SDK.SmartStore.Store.QuerySpec.SqlOrder>(sqlOrder), pageSize);
            var querySpec = JsonConvert.SerializeObject(nativeQuerySpec);
            return JsonConvert.DeserializeObject<QuerySpec>(querySpec);
        }

        public static QuerySpec BuildLikeQuerySpec(string soupName, string path, string likeKey, SqlOrder order,
            int pageSize)
        {
            var sqlOrder = JsonConvert.SerializeObject(order);
            var nativeQuerySpec = SDK.SmartStore.Store.QuerySpec.BuildLikeQuerySpec(soupName, path, likeKey, JsonConvert.DeserializeObject<SDK.SmartStore.Store.QuerySpec.SqlOrder>(sqlOrder), pageSize);
            var querySpec = JsonConvert.SerializeObject(nativeQuerySpec);
            return JsonConvert.DeserializeObject<QuerySpec>(querySpec);
        }

        public static QuerySpec BuildSmartQuerySpec(string smartSql, int pageSize)
        {
            var nativeQuerySpec = SDK.SmartStore.Store.QuerySpec.BuildSmartQuerySpec(smartSql, pageSize);
            var querySpec = JsonConvert.SerializeObject(nativeQuerySpec);
            return JsonConvert.DeserializeObject<QuerySpec>(querySpec);
        }
    }
}
