using Salesforce.SDK.App;
using Salesforce.SDK.Strings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Salesforce.SDK.Auth
{
    public sealed partial class PincodeDialog : Page
    {
        private PincodeOptions Options;
        /// <summary>
        /// Defines number of retries of locked screen. We log user out when RetryCounter hits 1.
        /// </summary>
        private static readonly int MaximumRetries = 4;
        private static int RetryCounter = MaximumRetries;
        private static readonly int MinimumWidthForBarMode = 500;
        private static readonly int BarModeHeight = 400;

        public PincodeDialog()
        {
            this.InitializeComponent();
            Passcode.PlaceholderText = LocalizedStrings.GetString("passcode");
            ErrorFlyout.Closed += ErrorFlyout_Closed;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter == null || !(e.Parameter is PincodeOptions))
            {
                Options = new PincodeOptions(PincodeOptions.PincodeScreen.Create, AccountManager.GetAccount(), "");
            }
            else
            {
                Options = e.Parameter as PincodeOptions;
            }
            switch (Options.Screen)
            {
                case PincodeOptions.PincodeScreen.Locked:
                    SetupLocked();
                    break;
                case PincodeOptions.PincodeScreen.Confirm:
                    SetupConfirm();
                    break;
                default:
                    SetupCreate();
                    break;
            }
            Passcode.Password = "";
            var bounds = Window.Current.Bounds;
            // This screen will adjust size for "narrow" views; if the view is sufficiently narrow it will fill the screen and move text to the top, so it can be seen with virtual keyboard.
            if (bounds.Width < MinimumWidthForBarMode)
            {
                PincodeContainer.Height = bounds.Height;
                PincodeWindow.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Top;
            } else
            {
                // Act as a "bar" for screens that have enough horizontal resolution.
                PincodeWindow.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Center;
                PincodeContainer.Height = BarModeHeight;
            }
  
        }

        /// <summary>
        /// Configures the dialog for creating a pincode.
        /// </summary>
        private void SetupCreate()
        {
            Title.Text = LocalizedStrings.GetString("passcode_create_title");
            Content.Text = LocalizedStrings.GetString("passcode_create_security");
            ContentFooter.Visibility = Windows.UI.Xaml.Visibility.Visible;
            ContentFooter.Text = String.Format(LocalizedStrings.GetString("passcode_length"), Options.User.Policy.PinLength);
            Passcode.KeyDown += CreateClicked;
        }

        /// <summary>
        /// Configures the dialog to act as a lockscreen.
        /// </summary>
        private void SetupLocked()
        {
            Title.Text = LocalizedStrings.GetString("passcode_enter_code_title");
            Content.Text = LocalizedStrings.GetString("passcode_confirm");
            ContentFooter.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            Passcode.KeyDown += LockedClick;
            RetryCounter = MaximumRetries;
        }

        /// <summary>
        /// Configures the dialog to act as a confirmation for the entered pincode.
        /// </summary>
        private void SetupConfirm()
        {
            Title.Text = LocalizedStrings.GetString("passcode_reenter");
            Content.Text = LocalizedStrings.GetString("passcode_confirm");
            ContentFooter.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            Passcode.KeyDown += ConfirmClicked;
        }

        private void CreateClicked(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key != VirtualKey.Accept && e.Key != VirtualKey.Enter)
                return;
            e.Handled = true;
            if (Passcode.Password.Length >= Options.User.Policy.PinLength)
            {
                PincodeOptions options = new PincodeOptions(PincodeOptions.PincodeScreen.Confirm, Options.User, Passcode.Password);
                Frame.Navigate(typeof(PincodeDialog), options);
            }
            else
            {
                DisplayErrorFlyout(String.Format(LocalizedStrings.GetString("passcode_length"), Options.User.Policy.PinLength));
            }
        }

        private void ConfirmClicked(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key != VirtualKey.Accept && e.Key != VirtualKey.Enter)
                return;
            e.Handled = true;
            if (Passcode.Password.Equals(Options.Passcode))
            {
                AuthStorageHelper auth = new AuthStorageHelper();
                Account account = Options.User;
                account.PincodeHash = PincodeManager.GenerateEncryptedPincode(Options.Passcode);
                auth.PersistCredentials(account);
                Frame.Navigate(SalesforceApplication.RootApplicationPage);
            }
            else
            {
                DisplayErrorFlyout(LocalizedStrings.GetString("passcode_must_match"));
            }
        }

        private async void LockedClick(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key != VirtualKey.Accept && e.Key != VirtualKey.Enter)
                return;
            e.Handled = true;
            Account account = AccountManager.GetAccount();
            if (PincodeManager.ValidatePincode(Passcode.Password, account.PincodeHash))
            {
                if (Frame.CanGoBack)
                {
                    Frame.GoBack();
                } else
                {
                    Frame.Navigate(SalesforceApplication.RootApplicationPage);
                }
            }
            else
            {
                if (RetryCounter <= 1)
                {
                    await SalesforceApplication.GlobalClientManager.Logout();
                }
                else
                {
                    RetryCounter--;
                    ContentFooter.Text = String.Format(LocalizedStrings.GetString("passcode_incorrect"), RetryCounter);
                    ContentFooter.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
            }
        }

        private void DisplayErrorFlyout(string text)
        {
            ErrorContent.Text = text;
            ErrorFlyout.ShowAt(Passcode);
        }

        private void ErrorFlyoutClicked(object sender, RoutedEventArgs e)
        {
            ErrorFlyout.Hide();
        }

        void ErrorFlyout_Closed(object sender, object e)
        {
            if (PincodeOptions.PincodeScreen.Confirm == Options.Screen)
            {
                PincodeOptions options = new PincodeOptions(PincodeOptions.PincodeScreen.Create, Options.User, "");
                Frame.Navigate(typeof(PincodeDialog), options);
            }
        }
    }
}
