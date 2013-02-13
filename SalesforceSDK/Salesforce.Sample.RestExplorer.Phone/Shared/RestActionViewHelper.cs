using Salesforce.SDK.Rest;
using System;
using System.Collections.Generic;
using System.Text;

namespace Salesforce.Sample.RestExplorer.Shared
{
    public class RestActionViewHelper
    {
        public static HashSet<String> GetNamesOfControlsToShow(String restActionStr)
        {
            HashSet<String> names = new HashSet<String>();
            RestAction restAction = (RestAction)Enum.Parse(typeof(RestAction), restActionStr);
            switch (restAction)
            {
                case RestAction.VERSIONS:
                    break;
                case RestAction.RESOURCES:
                    names.Add("tbApiVersion");
                    break;
                case RestAction.DESCRIBE_GLOBAL:
                    names.Add("tbApiVersion");
                    break;
                case RestAction.METADATA:
                    names.Add("tbApiVersion");
                    names.Add("tbObjectType");
                    break;
                case RestAction.DESCRIBE:
                    names.Add("tbApiVersion");
                    names.Add("tbObjectType");
                    break;
                case RestAction.CREATE:
                    names.Add("tbApiVersion");
                    names.Add("tbObjectType");
                    names.Add("tbFields");
                    break;
                case RestAction.RETRIEVE:
                    names.Add("tbApiVersion");
                    names.Add("tbObjectType");
                    names.Add("tbObjectId");
                    names.Add("tbFieldList");
                    break;
                case RestAction.UPSERT:
                    names.Add("tbApiVersion");
                    names.Add("tbObjectType");
                    names.Add("tbExternalIdField");
                    names.Add("tbExternalId");
                    names.Add("tbFields");
                    break;
                case RestAction.UPDATE:
                    names.Add("tbApiVersion");
                    names.Add("tbObjectType");
                    names.Add("tbObjectId");
                    names.Add("tbFields");
                    break;
                case RestAction.DELETE:
                    names.Add("tbApiVersion");
                    names.Add("tbObjectType");
                    names.Add("tbObjectId");
                    break;
                case RestAction.QUERY:
                    names.Add("tbApiVersion");
                    names.Add("tbSoql");
                    break;
                case RestAction.SEARCH:
                    names.Add("tbApiVersion");
                    names.Add("tbSosl");
                    break;
                case RestAction.MANUAL:
                    names.Add("tbRequestPath");
                    names.Add("tbRequestBody");
                    names.Add("tbRequestMethod");
                    break;
            }
            return names;
        }

        public static String BuildHtml(RestResponse response)
        {
            String[] blocks = (response == null
                ? null
                : new String[] { "<b>Status Code:</b>" + response.StatusCode, "<b>Body:</b>\n" + response.PrettyBody });

            String htmlHead = @"
            <head>
                <meta name=""viewport"" content=""width=device-width, initial-scale=1.0, maximum-scale=1.0; user-scalable=no"" />
                <style>
                    body { background-color: black; color: white; }
                    pre {border: 1px solid white; padding: 5px; word-wrap: break-word;}
                </style>
            </head>
            ";

            StringBuilder sb = new StringBuilder(htmlHead);

            sb.Append("<body>");

            if (blocks != null)
            {
                foreach (String block in blocks)
                {
                    sb.Append("<pre>").Append(block).Append("</pre>");
                }
            }
            sb.Append("</body>");

            return sb.ToString();
        }
    }
}
