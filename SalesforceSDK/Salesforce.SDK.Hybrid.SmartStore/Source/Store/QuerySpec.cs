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
        [JsonProperty]
        internal SDK.SmartStore.Store.QuerySpec SdkQuerySpec;

        internal string BeginKey
        {
            get { return SdkQuerySpec.BeginKey; }
        }

        internal string CountSmartSql
        {
            get { return SdkQuerySpec.CountSmartSql; }
        }

        internal string EndKey
        {
            get { return SdkQuerySpec.EndKey; }
        }

        internal string LikeKey
        {
            get { return SdkQuerySpec.LikeKey; }
        }

        internal string MatchKey
        {
            get { return SdkQuerySpec.MatchKey; }
        }

        internal SqlOrder Order
        {
            get
            {
                var order = JsonConvert.SerializeObject(SdkQuerySpec.Order);
                return JsonConvert.DeserializeObject<SqlOrder>(order);
            }
        }

        internal int PageSize
        {
            get { return SdkQuerySpec.PageSize; }
        }

        internal string Path
        {
            get { return SdkQuerySpec.Path; }
        }

        internal SmartQueryType QueryType
        {
            get
            {
                var queryType = JsonConvert.SerializeObject(SdkQuerySpec.QueryType);
                return JsonConvert.DeserializeObject<SmartQueryType>(queryType);
            }
        }

        internal string SmartSql
        {
            get { return SdkQuerySpec.SmartSql; }
        }

        internal string SoupName
        {
            get { return SdkQuerySpec.SoupName; }
        }

        public static QuerySpec BuildAllQuerySpec(string soupName, string path, SqlOrder order, int pageSize)
        {
            var sqlOrder = JsonConvert.SerializeObject(order);
            var nativeQuerySpec = SDK.SmartStore.Store.QuerySpec.BuildAllQuerySpec(soupName, path, JsonConvert.DeserializeObject<SDK.SmartStore.Store.QuerySpec.SqlOrder>(sqlOrder), pageSize);
            var querySpec = new QuerySpec {SdkQuerySpec = nativeQuerySpec};
            return querySpec;
        }

        public static QuerySpec BuildExactQuerySpec(string soupName, string path, string exactMatchKey, int pageSize)
        {
            var nativeQuerySpec = SDK.SmartStore.Store.QuerySpec.BuildExactQuerySpec(soupName, path, exactMatchKey,
                pageSize);
            var querySpec = new QuerySpec {SdkQuerySpec = nativeQuerySpec};
            return querySpec;
        }

        public static QuerySpec BuildRangeQuerySpec(string soupName, string path, string beginKey, string endKey,
            SqlOrder order, int pageSize)
        {
            var sqlOrder = JsonConvert.SerializeObject(order);
            var nativeQuerySpec = SDK.SmartStore.Store.QuerySpec.BuildRangeQuerySpec(soupName, path, beginKey, endKey, JsonConvert.DeserializeObject<SDK.SmartStore.Store.QuerySpec.SqlOrder>(sqlOrder), pageSize);
            var querySpec = new QuerySpec {SdkQuerySpec = nativeQuerySpec};
            return querySpec;
        }

        public static QuerySpec BuildLikeQuerySpec(string soupName, string path, string likeKey, SqlOrder order,
            int pageSize)
        {
            var sqlOrder = JsonConvert.SerializeObject(order);
            var nativeQuerySpec = SDK.SmartStore.Store.QuerySpec.BuildLikeQuerySpec(soupName, path, likeKey, JsonConvert.DeserializeObject<SDK.SmartStore.Store.QuerySpec.SqlOrder>(sqlOrder), pageSize);
            var querySpec = new QuerySpec {SdkQuerySpec = nativeQuerySpec};
            return querySpec;
        }

        public static QuerySpec BuildSmartQuerySpec(string smartSql, int pageSize)
        {
            var nativeQuerySpec = SDK.SmartStore.Store.QuerySpec.BuildSmartQuerySpec(smartSql, pageSize);
            var querySpec = new QuerySpec { SdkQuerySpec = nativeQuerySpec };
            return querySpec;
        }
    }
}
