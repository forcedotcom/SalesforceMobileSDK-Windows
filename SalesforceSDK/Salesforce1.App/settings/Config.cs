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
            get { return "3MVG94DzwlYDSHS7X_sg6NktSIw.TO72dzPDBjGfVmqUpPjXSYVs.hZsvOFH5OU2z6GgyaPE6uEhd4QvRgXge"; }
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
