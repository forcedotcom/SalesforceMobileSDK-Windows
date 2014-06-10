using Salesforce.SDK.Source.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salesforce.One.App.settings
{
    class Config : SalesforceConfig
    {
        public override string ClientId
        {
            get { return "SfdcMobileChatterAndroid"; }
        }

        public override string CallbackUrl
        {
            get { return "sfdc:///axm/detect/oauth/done"; }
        }

        public override string[] Scopes
        {
            get { return new string[] { "web", "api" }; }
        }
    }
}
