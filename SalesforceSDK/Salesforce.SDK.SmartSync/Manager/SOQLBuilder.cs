/*
 * Copyright (c) 2014, salesforce.com, inc.
 * All rights reserved.
 * Redistribution and use of this software in source and binary forms, with or
 * without modification, are permitted provided that the following conditions
 * are met:
 * - Redistributions of source code must retain the above copyright notice, this
 * list of conditions and the following disclaimer.
 * - Redistributions in binary form must reproduce the above copyright notice,
 * this list of conditions and the following disclaimer in the documentation
 * and/or other materials provided with the distribution.
 * - Neither the name of salesforce.com, inc. nor the names of its contributors
 * may be used to endorse or promote products derived from this software without
 * specific prior written permission of salesforce.com, inc.
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Collections.Generic;
using System.Text;
using Salesforce.SDK.SmartSync.Util;

namespace Salesforce.SDK.SmartSync.Manager
{
    public class SOQLBuilder
    {
        private readonly Dictionary<string, object> _properties;

        private SOQLBuilder()
        {
            _properties = new Dictionary<string, object>();
        }

        public static SOQLBuilder GetInstanceWithFields(string fields)
        {
            var instance = new SOQLBuilder();
            instance.Fields(fields);
            instance.Limit(0);
            instance.Offset(0);
            return instance;
        }

        public static SOQLBuilder GetInstanceWithFields(params string[] fields)
        {
            return GetInstanceWithFields(String.Join(", ", fields));
        }

        public SOQLBuilder Fields(string fields)
        {
            _properties.Add("fields", fields);
            return this;
        }

        public SOQLBuilder From(string from)
        {
            _properties.Add("from", from);
            return this;
        }

        public SOQLBuilder Where(string where)
        {
            _properties.Add("where", where);
            return this;
        }

        public SOQLBuilder With(string with)
        {
            _properties.Add("with", with);
            return this;
        }

        public SOQLBuilder GroupBy(string groupBy)
        {
            _properties.Add("groupBy", groupBy);
            return this;
        }

        public SOQLBuilder Having(string having)
        {
            _properties.Add("having", having);
            return this;
        }

        public SOQLBuilder OrderBy(string orderBy)
        {
            _properties.Add("orderBy", orderBy);
            return this;
        }

        public SOQLBuilder Limit(int limit)
        {
            _properties.Add("limit", limit);
            return this;
        }

        public SOQLBuilder Offset(int offset)
        {
            _properties.Add("offset", offset);
            return this;
        }

        public string BuildAndEncode()
        {
            return Uri.EscapeUriString(Build());
        }

        public string BuildAndEncodeWithPath(string path)
        {
            string result = BuildWithPath(path);
            if (!String.IsNullOrWhiteSpace(result))
            {
                result = Uri.EscapeUriString(result);
            }
            return result;
        }

        public string BuildWithPath(string path)
        {
            string result = null;
            if (!String.IsNullOrWhiteSpace(path))
            {
                result = path.EndsWith("/")
                    ? String.Format("{0}query/?q={1}", path, Build())
                    : String.Format("{0}/query/?q={1}", path, Build());
            }
            return result;
        }

        public string Build()
        {
            var query = new StringBuilder();
            var fieldList = _properties.Get<string>("fields");
            var from = _properties.Get<string>("from");
            if (String.IsNullOrWhiteSpace(fieldList) || String.IsNullOrWhiteSpace(from))
            {
                return null;
            }
            query.Append("select ");
            query.Append(fieldList);
            query.Append(" from ");
            query.Append(from);
            var where = _properties.Get<string>("where");
            if (!String.IsNullOrWhiteSpace(where))
            {
                query.Append(" where ");
                query.Append(where);
            }
            var groupBy = _properties.Get<string>("groupBy");
            if (!String.IsNullOrWhiteSpace(groupBy))
            {
                query.Append(" group by ");
                query.Append(groupBy);
            }
            var having = _properties.Get<string>("having");
            if (!String.IsNullOrWhiteSpace(having))
            {
                query.Append(" having ");
                query.Append(having);
            }
            var orderBy = _properties.Get<string>("orderBy");
            if (!String.IsNullOrWhiteSpace(orderBy))
            {
                query.Append(" order by ");
                query.Append(orderBy);
            }
            var limit = _properties.Get<int>("limit");
            if (limit > 0)
            {
                query.Append(" limit ");
                query.Append(limit);
            }
            var offset = _properties.Get<int>("offset");
            if (offset > 0)
            {
                query.Append(" offset ");
                query.Append(offset);
            }
            return query.ToString();
        }
    }
}