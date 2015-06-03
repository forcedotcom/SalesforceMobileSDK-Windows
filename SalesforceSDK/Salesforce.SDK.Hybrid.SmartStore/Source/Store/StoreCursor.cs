#region Copyright
// 
//  Copyright (c) 2015, salesforce.com, inc.
//  All rights reserved.
//  Company Confidential
//  
#endregion

using System;
using Newtonsoft.Json.Linq;
using Salesforce.SDK.Hybrid.SmartStore.Source.Store;

namespace Salesforce.SDK.Hybrid.SmartStore
{
    public sealed class StoreCursor
    {
        private static int _lastId;

        private readonly QuerySpec _querySpec;
        private readonly int _totalPages;
        private readonly long _totalEntries;
        private int _currentPageIndex;
        public int CursorId { get; }

        public StoreCursor()
        {
            throw new NotImplementedException();
        }

        public StoreCursor(ISmartStore store, QuerySpec querySpec)
        {
            if (store == null || querySpec == null || querySpec.PageSize <= 0)
            {
                throw new ArgumentException();
            }

            CursorId = _lastId++;
            _querySpec = querySpec;
            _totalEntries = store.CountQuery(querySpec);
            _totalPages = (int)Math.Ceiling(((double)_totalEntries) / querySpec.PageSize);
            _currentPageIndex = 0;
        }

        public void MoveToPageIndex(int newPageIndex)
        {
            _currentPageIndex = newPageIndex < 0 ? 0 : newPageIndex >= _totalPages ? _totalPages - 1 : newPageIndex;
        }

        public string GetCursorData(ISmartStore smartStore)
        {
            return new JObject
            {
                {"cursorId", CursorId},
                {"currentPageIndex", _currentPageIndex},
                {"pageSize", _querySpec.PageSize},
                {"totalEntries", _totalEntries},
                {"totalPages", _totalPages},
                {"currentPageOrderedEntries", smartStore.Query(_querySpec, _currentPageIndex)}
            }.ToString();
        }
    }
}
