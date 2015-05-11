using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Salesforce.SDK.Hybrid.Auth;

namespace Salesforce.SDK.Hybrid.SmartStore
{
    public sealed class DBOpenHelper
    {
        public string DatabaseFile { private set; get; }

        public static DBOpenHelper GetOpenHelper(Account account)
        {
            var nativeAccountJson = JsonConvert.SerializeObject(account);
            var nativeDbOpenHelper = JsonConvert.SerializeObject(SDK.SmartStore.Store.DBOpenHelper.GetOpenHelper(JsonConvert.DeserializeObject<SDK.Auth.Account>(nativeAccountJson)));
            return JsonConvert.DeserializeObject<DBOpenHelper>(nativeDbOpenHelper);
        }

        public static DBOpenHelper GetOpenHelper(Account account, string communityId)
        {
            var nativeAccountJson = JsonConvert.SerializeObject(account);
            var nativeDbOpenHelper = JsonConvert.SerializeObject(SDK.SmartStore.Store.DBOpenHelper.GetOpenHelper(JsonConvert.DeserializeObject<SDK.Auth.Account>(nativeAccountJson), communityId));
            return JsonConvert.DeserializeObject<DBOpenHelper>(nativeDbOpenHelper);
        }

        public static DBOpenHelper GetOpenHelper(string dbNamePrefix, Account account, string communityId)
        {
            var nativeAccountJson = JsonConvert.SerializeObject(account);
            var nativeDbOpenHelper = JsonConvert.SerializeObject(SDK.SmartStore.Store.DBOpenHelper.GetOpenHelper(dbNamePrefix, JsonConvert.DeserializeObject<SDK.Auth.Account>(nativeAccountJson), communityId));
            return JsonConvert.DeserializeObject<DBOpenHelper>(nativeDbOpenHelper);
        }
    }
}
