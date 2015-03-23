using System;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using Salesforce.SDK.SmartStore.Store;

namespace Salesforce.Sample.NativeSmartStoreSample.utilities
{
    public class SmartStoreExtension
    {
        public const string AccountsSoup = "Account";
        public const string OpportunitiesSoup = "Opportunity";

        public readonly IndexSpec[] AccountsIndexSpecs = new IndexSpec[]
        {
            new IndexSpec("Name", SmartStoreType.SmartString),
            new IndexSpec("Id", SmartStoreType.SmartString),
            new IndexSpec("OwnerId", SmartStoreType.SmartString)
        };

        public readonly IndexSpec[] OpportunitiesIndexSpecs = new IndexSpec[]
        {
            new IndexSpec("Name", SmartStoreType.SmartString),
            new IndexSpec("Id", SmartStoreType.SmartString),
            new IndexSpec("AccountId", SmartStoreType.SmartString),
            new IndexSpec("OwnerId", SmartStoreType.SmartString),
            new IndexSpec("Amount", SmartStoreType.SmartInteger),
        };

        private readonly SmartStore _store;

        public SmartStoreExtension()
        {
            _store = SmartStore.GetSmartStore();
            CreateAccountsSoup();
            CreateOpportunitiesSoup();
        }

        public void CreateAccountsSoup()
        {
            _store.RegisterSoup(AccountsSoup, AccountsIndexSpecs);
        }

        public void CreateOpportunitiesSoup()
        {
            _store.RegisterSoup(OpportunitiesSoup, OpportunitiesIndexSpecs);
        }

        public void DeleteAccountsSoup()
        {
            if (_store.HasSoup(AccountsSoup))
            {
                _store.DropSoup(AccountsSoup);
            }
        }

        public void DeleteOpportunitiesSoup()
        {
            if (_store.HasSoup(OpportunitiesSoup))
            {
                _store.DropSoup(OpportunitiesSoup);
            }
        }

        public void InsertAccounts(JArray accounts)
        {
            if (accounts != null && accounts.Count > 0)
            {
                foreach (var account in accounts.Values<JObject>())
                {
                    InsertAccount(account);
                }
            }
        }

        public void InsertAccount(JObject account)
        {
            if (account != null)
            {
                _store.Upsert(AccountsSoup, account);
            }
        }

        public void InsertOpportunities(JArray opportunities)
        {
            if (opportunities != null && opportunities.Count > 0)
            {
                foreach (var opportunity in opportunities.Values<JObject>())
                {
                    InsertOpportunity(opportunity);
                }
            }
        }

        public void InsertOpportunity(JObject opportunity)
        {
            if (opportunity != null)
            {
                JToken doubleAmount;
                if (opportunity.TryGetValue("Amount", StringComparison.CurrentCultureIgnoreCase, out doubleAmount))
                {
                    try
                    {
                        double amount = Double.Parse(doubleAmount.ToString());
                        opportunity["Amount"] = amount;
                    }
                    catch (FormatException e)
                    {
                        opportunity["Amount"] = 0;
                    }
                }
                else
                {
                    opportunity.Add("Amount", 0);
                }
                _store.Upsert(OpportunitiesSoup, opportunity);
            }
        }

        /// <summary>
        /// Returns saved opportunities.
        /// </summary>
        /// <returns></returns>
        public JArray GetOpportunities()
        {
            return Query("SELECT {Opportunity:_soup} FROM {Opportunity}");
        }

        /// <summary>
        /// Returns saved accounts.
        /// </summary>
        /// <returns></returns>
        public JArray GetAccounts()
        {
            return Query("SELECT {Account:_soup} FROM {Account}");
        }

        public JArray Query(string smartSql)
        {
            JArray result = null;
            QuerySpec querySpec = QuerySpec.BuildSmartQuerySpec(smartSql, 10);
            var count = (int)_store.CountQuery(querySpec);
            querySpec = QuerySpec.BuildSmartQuerySpec(smartSql, count);
            try
            {
                result = _store.Query(querySpec, 0);
            }
            catch (SmartStoreException e)
            {
                Debug.WriteLine("Error occurred while attempting to run query. Please verify validity of the query.");
            }
            return result;
        }
    }
}
