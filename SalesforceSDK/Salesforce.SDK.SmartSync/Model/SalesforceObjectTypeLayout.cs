using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Salesforce.SDK.SmartStore.Store;
using Salesforce.SDK.SmartSync.Util;

namespace Salesforce.SDK.SmartSync.Model
{
    public class SalesforceObjectTypeLayout
    {
        public int Limit { private set; get; }
        public string ObjectType { private set; get; }
        public JObject RawData { private set; get; }
        public List<SalesforceObjectLayoutColumn> Columns { private set; get; }

        public SalesforceObjectTypeLayout(string objType, JObject rawData)
        {
            if (rawData == null)
            {
                throw new SmartStoreException("rawData parameter cannot be null");
            }
            ObjectType = objType;
            RawData = rawData;
            ParseFields();
        }

        private void ParseFields()
        {
            Limit = RawData.ExtractValue<int>(Constants.LayoutLimitsField);
            var searchColumns = RawData.ExtractValue<JArray>(Constants.LayoutColumnsField);
            if (searchColumns != null)
            {
                for (int i = 0, max = searchColumns.Count; i < max; i++)
                {
                    var columnData = searchColumns[i].Value<JObject>();
                    if (columnData != null)
                    {
                        Columns.Add(new SalesforceObjectLayoutColumn(columnData));
                    }
                }
            }
        }

        public override string ToString()
        {
            return String.Format("objectType: [{0}], limit: [{1}], rawData: [{2}]", ObjectType, Limit, RawData);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is SalesforceObjectTypeLayout))
            {
                return false;
            }
            var salesforceObject = (SalesforceObjectTypeLayout) obj;
            if (ObjectType == null || !ObjectType.Equals(salesforceObject.ObjectType))
            {
                return false;
            }
            return CompareColumns(salesforceObject);
        }

        public override int GetHashCode()
        {
            return ObjectType.GetHashCode();
        }

        private bool CompareColumns(SalesforceObjectTypeLayout obj) {
    	if (obj == null) {
    		return false;
    	}
    	List<SalesforceObjectLayoutColumn> objColumns = obj.Columns;
    	if ((objColumns == null || objColumns.Count == 0)
    			&& (Columns == null || Columns.Count == 0)) {
    		return true;
    	}
            if (objColumns != null)
            {
                int objColumnSize = objColumns.Count;
                if (objColumnSize != Columns.Count) {
                    return false;
                }
            }
            return objColumns.All(objColumn => Columns.Contains(objColumn));
        }
    }
}