using Salesforce.SDK.Adaptation;
using Salesforce.SDK.Auth;
using Salesforce.SDK.Rest;
using Salesforce.SDK.Source.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Salesforce.SDK.App
{
    public abstract class SalesforcePhoneApplication : SalesforceApplication
    {
        public static ContinuationManagerImpl ContinuationManager { get; private set; }

        public SalesforcePhoneApplication()
            : base()
        {
            ContinuationManager = new ContinuationManagerImpl();
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {
            base.OnActivated(args);
            if (args is IContinuationActivatedEventArgs)
            {
                var continueEvents = args as IContinuationActivatedEventArgs;
                ContinuationManager.Continue(continueEvents);
            }
        }

        protected override void OnSuspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            base.OnSuspending(sender, e);
            var deferral = e.SuspendingOperation.GetDeferral();
            ContinuationManager.MarkAsStale();
            deferral.Complete();
        }
    }
}
