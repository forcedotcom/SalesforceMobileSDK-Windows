/*
 * Copyright (c) 2013, salesforce.com, inc.
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
using Salesforce.SDK.Rest;

namespace Salesforce.Sample.RestExplorer.Shared
{
    /// <summary>
    ///     Helper class for Rest Explorer main page (phone and store)
    /// </summary>
    public class RestActionViewHelper
    {
        /// <summary>
        ///     Returns controls to show by name (since control actual types are different on phone and store)
        /// </summary>
        /// <param name="restActionStr"></param>
        /// <returns></returns>
        public static HashSet<string> GetNamesOfControlsToShow(string restActionStr)
        {
            var names = new HashSet<string>();
            var restAction = (RestAction) Enum.Parse(typeof (RestAction), restActionStr);
            switch (restAction)
            {
                case RestAction.Versions:
                    break;
                case RestAction.Resources:
                    names.Add("tbApiVersion");
                    break;
                case RestAction.DescribeGlobal:
                    names.Add("tbApiVersion");
                    break;
                case RestAction.Metadata:
                    names.Add("tbApiVersion");
                    names.Add("tbObjectType");
                    break;
                case RestAction.Describe:
                    names.Add("tbApiVersion");
                    names.Add("tbObjectType");
                    break;
                case RestAction.Create:
                    names.Add("tbApiVersion");
                    names.Add("tbObjectType");
                    names.Add("tbFields");
                    break;
                case RestAction.Retrieve:
                    names.Add("tbApiVersion");
                    names.Add("tbObjectType");
                    names.Add("tbObjectId");
                    names.Add("tbFieldList");
                    break;
                case RestAction.Upsert:
                    names.Add("tbApiVersion");
                    names.Add("tbObjectType");
                    names.Add("tbExternalIdField");
                    names.Add("tbExternalId");
                    names.Add("tbFields");
                    break;
                case RestAction.Update:
                    names.Add("tbApiVersion");
                    names.Add("tbObjectType");
                    names.Add("tbObjectId");
                    names.Add("tbFields");
                    break;
                case RestAction.Delete:
                    names.Add("tbApiVersion");
                    names.Add("tbObjectType");
                    names.Add("tbObjectId");
                    break;
                case RestAction.Query:
                    names.Add("tbApiVersion");
                    names.Add("tbSoql");
                    break;
                case RestAction.Search:
                    names.Add("tbApiVersion");
                    names.Add("tbSosl");
                    break;
                case RestAction.Manual:
                    names.Add("tbRequestPath");
                    names.Add("tbRequestBody");
                    names.Add("tbRequestMethod");
                    break;
            }
            return names;
        }

        /// <summary>
        ///     Build HTML from a RestResponse
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static string BuildHtml(RestResponse response)
        {
            string[] blocks = (response == null
                ? null
                : new[] {"<b>Status Code:</b>" + response.StatusCode, "<b>Body:</b>\n" + response.PrettyBody});

            string htmlHead = @"
            <head>
                <meta name=""viewport"" content=""width=device-width, initial-scale=1.0, maximum-scale=1.0; user-scalable=no"" />
                <style>
                    body { background-color: black; color: white; }
                    pre {border: 1px solid white; padding: 5px; word-wrap: break-word;}
                </style>
            </head>
            ";

            var sb = new StringBuilder(htmlHead);

            sb.Append("<body>");

            if (blocks != null)
            {
                foreach (string block in blocks)
                {
                    sb.Append("<pre>").Append(block).Append("</pre>");
                }
            }
            sb.Append("</body>");

            return sb.ToString();
        }
    }
}