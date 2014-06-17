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
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


namespace Salesforce.SDK.App
{
    /// <summary>
    /// Abstract application class used to provide access to functions in the SalesforceSDK.  use this for your main App.xaml to allow support for the
    /// SDK, an entry point for handling oauth and account switching, and providing a client manager that can be used across the app in a central location.
    /// </summary>
    public abstract class SalesforceApplication : Application
    {

        /// <summary>
        /// The global client manager is provided for ease of accessing clients such as the RestClient.
        /// </summary>
        public static ClientManager GlobalClientManager { get; private set; }
        /// <summary>
        /// Returns the Type of the root page; an assistive property to allow an app to return to root after things such as oauth login.
        /// </summary>
        public static Type RootApplicationPage { get; private set; }
        /// <summary>
        /// The current configuration for the application.
        /// </summary>
        public static SalesforceConfig ServerConfiguration { get; private set; }

        public SalesforceApplication() : base()
        {
            Suspending += OnSuspending;
            CreateClientManager(false);
            RootApplicationPage = SetRootApplicationPage();
        }

        /// <summary>
        /// Use this to initialize your custom SalesforceConfig source, and to set up the Encryptor to use your own app specific, unique salt, password, and key generator.
        /// An example of code that may go into this method would be as follows:
        /// 
        ///     protected override void InitializeConfig()
        ///     {
        ///         new Config();
        ///         EncryptionSettings settings = new EncryptionSettings(new HmacSHA256KeyGenerator())
        ///         {
        ///             Password = "mypassword",
        ///             Salt = "mysalt"
        ///         };
        ///         Encryptor.init(settings);
        ///     }
        /// </summary>
        ///
        protected abstract SalesforceConfig InitializeConfig();

        /// <summary>
        /// Implement to return the type of the root page to switch to once oauth completes.
        /// </summary>
        /// <returns>Type of the root page</returns>
        protected abstract Type SetRootApplicationPage();

        protected virtual async void OnSuspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            PincodeManager.SavePinTimer();
            var deferral = e.SuspendingOperation.GetDeferral();
            await SuspensionManager.SaveAsync();
            deferral.Complete();
        }

        protected async override void OnActivated(IActivatedEventArgs args)
        {
            if (ApplicationExecutionState.Terminated.Equals(args.PreviousExecutionState))
            {
                try
                {
                    await SuspensionManager.RestoreAsync();
                }
                catch (SuspensionManagerException)
                {
                    // Assume there is no state and continue
                }
            }
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.NavigationFailed += OnNavigationFailed;
            PincodeManager.TriggerBackgroundedPinTimer();
        }

        protected override void OnWindowCreated(WindowCreatedEventArgs args)
        {
            base.OnWindowCreated(args);
            SetupVisibilityHandler();
        }

        private async void SetupVisibilityHandler()
        {
            CoreWindow core = CoreWindow.GetForCurrentThread();
            await core.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                CoreWindow coreWindow = CoreApplication.MainView.CoreWindow;
                coreWindow.VisibilityChanged += CoreVisibilityChanged;
            });
        }

        private void CoreVisibilityChanged(CoreWindow sender, VisibilityChangedEventArgs args)
        {
            if (args.Visible)
            {
                PincodeManager.TriggerBackgroundedPinTimer();
            } else
            {
                PincodeManager.SavePinTimer();
            }
        }

        private async void OnNavigationFailed(object sender, Windows.UI.Xaml.Navigation.NavigationFailedEventArgs args)
        {
            if (GlobalClientManager != null)
            {
                await GlobalClientManager.Logout();
            }
        }

        public static void ResetClientManager()
        {
            GlobalClientManager = new ClientManager();
        }

        protected void CreateClientManager(bool reset)
        {
            if (GlobalClientManager == null || reset)
            {
                ServerConfiguration = InitializeConfig();
                GlobalClientManager = new ClientManager();
            }

            GlobalClientManager.GetRestClient();
        }
    }
}
