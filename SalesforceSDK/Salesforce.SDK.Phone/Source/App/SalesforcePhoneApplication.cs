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
using Windows.Phone.UI.Input;
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
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            ContinuationManager = new ContinuationManagerImpl();
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            Frame frame = Window.Current.Content as Frame;
            if (frame == null)
            {
                return;
            }

            if (frame.SourcePageType.Equals(typeof(PincodeDialog)))
            {
                PlatformAdapter.Resolve<IAuthHelper>().StartLoginFlow();
                e.Handled = true;
            }
            else if (frame.CanGoBack)
            {
                frame.GoBack();
                e.Handled = true;
            }
        }

        protected async override void OnLaunched(LaunchActivatedEventArgs e)
        {
            if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
                // Restore the saved session state only when appropriate
                try
                {
                    await SuspensionManager.RestoreAsync();
                }
                catch (SuspensionManagerException)
                {
                    //Something went wrong restoring state.
                    //Assume there is no state and continue
                }
            }
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

        protected async override void OnSuspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            await SuspensionManager.SaveAsync();
            ContinuationManager.MarkAsStale();
            deferral.Complete();
        }
    }
}
