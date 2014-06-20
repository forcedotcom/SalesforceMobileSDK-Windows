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
using Salesforce.SDK.Source.Security;
using Salesforce.SDK.Strings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Salesforce.SDK.Auth
{
    public class PincodeManager
    {
        private static DispatcherTimer PinTimer = new DispatcherTimer();
        private static readonly string PinBackgroundedTimeKey = "pintimeKey";

        internal static string GenerateEncryptedPincode(string pincode)
        {
            HashAlgorithmProvider alg = HashAlgorithmProvider.OpenAlgorithm("MD5");
            IBuffer buff = CryptographicBuffer.ConvertStringToBinary(pincode, BinaryStringEncoding.Utf8);
            IBuffer hashed = alg.HashData(buff);
            string res = CryptographicBuffer.EncodeToHexString(hashed);
            return Encryptor.Encrypt(res);
        }

        public static bool ValidatePincode(string pincode, string encryptedPincode)
        {
            string compare = GenerateEncryptedPincode(pincode);
            return compare.Equals(encryptedPincode);
        }

        internal static void SavePinTimer()
        {
            Account account = AccountManager.GetAccount();
            if (account != null && account.Policy != null && account.Policy.ScreenLockTimeout > 0)
            {
                AuthStorageHelper auth = new AuthStorageHelper();
                auth.PersistData(PinBackgroundedTimeKey, DateTime.Now.ToUniversalTime().ToString(), true);
                StopTimer();
            }
        }

        internal static void TriggerBackgroundedPinTimer()
        {
            Account account = AccountManager.GetAccount();
            AuthStorageHelper auth = new AuthStorageHelper();
            if (account != null && account.Policy != null && account.Policy.ScreenLockTimeout > 0)
            {
                var policy = account.Policy;
                var time = auth.RetrieveData(PinBackgroundedTimeKey);
                if (time != null)
                {
                    DateTime previous = DateTime.Parse(time as string);
                    DateTime current = DateTime.Now.ToUniversalTime();
                    TimeSpan diff = current.Subtract(previous);
                    if (diff.Minutes >= policy.ScreenLockTimeout)
                    {
                        LaunchPincodeScreen();
                    }
                    else
                    {
                        MobilePolicy restartPolicy = new MobilePolicy()
                        {
                            PinLength = policy.PinLength,
                            ScreenLockTimeout = diff.Minutes
                        };
                        StartTimer(restartPolicy);
                    }
                }
            }
            else
            {
                auth.DeleteData(PinBackgroundedTimeKey);
            }
        }

        internal static void StartTimer(MobilePolicy policy)
        {
            if (PinTimer.IsEnabled)
            {
                PinTimer.Stop();
                PinTimer.Tick -= PinTimer_Tick;
            }
            PinTimer.Interval = TimeSpan.FromMinutes(policy.ScreenLockTimeout);
            PinTimer.Tick += PinTimer_Tick;
            PinTimer.Start();
        }

        internal static void StopTimer()
        {
            PinTimer.Stop();
        }

        static void PinTimer_Tick(object sender, object e)
        {
            PinTimer.Stop();
            
            if (sender is DispatcherTimer)
            {
                DispatcherTimer timer = sender as DispatcherTimer;
                if (timer.Interval.Minutes > 0)
                {
                    LaunchPincodeScreen();
                }
            }

        }

        public static async void LaunchPincodeScreen()
        {
            Frame frame = Window.Current.Content as Frame;
            if (frame != null)
            {
                await frame.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    Account account = AccountManager.GetAccount();
                    if (account != null)
                    {
                        PincodeOptions options = null;
                        if (account.Policy != null && String.IsNullOrWhiteSpace(account.PincodeHash))
                        {
                            options = new PincodeOptions(PincodeOptions.PincodeScreen.Create, account, "");
                        } else if (account.Policy != null)
                        {
                            options = new PincodeOptions(PincodeOptions.PincodeScreen.Locked, account, "");
                        }
                        if (options != null)
                        {
                            frame.Navigate(typeof(PincodeDialog), options);
                        }

                    }
                });
            }
        }
    }
}
