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
            get { return "3MVG9ytVT1SanXDkvbx5XRMc.mVU3633YHCdPbP3DsFj53GLlB0la25M3BQjpAA1HsP3lXmjKSssihnQpKu9x"; }
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
