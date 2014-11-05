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
    public class SOSLReturningBuilder
    {
        private readonly Dictionary<string, object> _properties;

        private SOSLReturningBuilder()
        {
            _properties = new Dictionary<string, object>();
        }

        public static SOSLReturningBuilder GetInstanceWithObjectName(string name)
        {
            var instance = new SOSLReturningBuilder();
            instance.ObjectName(name);
            instance.Limit(0);
            return instance;
        }

        public SOSLReturningBuilder Fields(string fields)
        {
            _properties.Add("fields", fields);
            return this;
        }

        public SOSLReturningBuilder Where(string where)
        {
            _properties.Add("where", where);
            return this;
        }

        public SOSLReturningBuilder OrderBy(string orderBy)
        {
            _properties.Add("orderBy", orderBy);
            return this;
        }

        public SOSLReturningBuilder ObjectName(string objectName)
        {
            _properties.Add("objectName", objectName);
            return this;
        }

        public SOSLReturningBuilder Limit(int limit)
        {
            _properties.Add("limit", limit);
            return this;
        }

        public SOSLReturningBuilder WithNetwork(string withNetwork)
        {
            _properties.Add("withNetwork", withNetwork);
            return this;
        }

        public string Build()
        {
            var query = new StringBuilder();
            var objectName = _properties.Get<string>("objectName");
            if (String.IsNullOrWhiteSpace(objectName))
            {
                return null;
            }
            query.Append(" ");
            query.Append(objectName);
            var fields = _properties.Get<string>("fields");
            if (!String.IsNullOrWhiteSpace(objectName))
            {
                query.Append(String.Format("(%s", fields));
                var where = _properties.Get<string>("where");
                if (!String.IsNullOrWhiteSpace(where))
                {
                    query.Append(" where ");
                    query.Append(where);
                }
                var orderBy = _properties.Get<string>("orderBy");
                if (!String.IsNullOrWhiteSpace(orderBy))
                {
                    query.Append(" order by ");
                    query.Append(orderBy);
                }
                var withNetwork = _properties.Get<string>("withNetwork");
                if (!String.IsNullOrWhiteSpace(withNetwork))
                {
                    query.Append(" with network = ");
                    query.Append(withNetwork);
                }
                var limit = _properties.Get<int>("limit");
                if (limit > 0)
                {
                    query.Append(" limit ");
                    query.Append(String.Format("%d", limit));
                }
                query.Append(")");
            }
            return query.ToString();
        }
    }
}