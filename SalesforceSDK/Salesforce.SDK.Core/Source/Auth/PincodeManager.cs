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
using System;
using System.Diagnostics;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Newtonsoft.Json;
using Salesforce.SDK.Source.Security;

namespace Salesforce.SDK.Auth
{
    public class PincodeManager
    {
        private const string PinBackgroundedTimeKey = "pintimeKey";
        private const string PincodeRequired = "pincodeRequired";
        private static readonly DispatcherTimer IdleTimer;

        static PincodeManager()
        {
            IdleTimer = new DispatcherTimer();
            IdleTimer.Tick += IdleTimer_Tick;
        }

        internal static string GenerateEncryptedPincode(string pincode)
        {
            HashAlgorithmProvider alg = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha256);
            IBuffer buff = CryptographicBuffer.ConvertStringToBinary(pincode, BinaryStringEncoding.Utf8);
            IBuffer hashed = alg.HashData(buff);
            string res = CryptographicBuffer.EncodeToHexString(hashed);
            return Encryptor.Encrypt(res);
        }

        /// <summary>
        ///     Validate the given pincode against the stored pincode.
        /// </summary>
        /// <param name="pincode">Pincode to validate</param>
        /// <returns>True if pincode matches</returns>
        public static bool ValidatePincode(string pincode)
        {
            string compare = GenerateEncryptedPincode(pincode);
            try
            {
                string retrieved = AuthStorageHelper.GetAuthStorageHelper().RetrievePincode();
                var policy = JsonConvert.DeserializeObject<MobilePolicy>(retrieved);
                return compare.Equals(Encryptor.Decrypt(policy.PincodeHash, pincode));
            }
            catch (Exception)
            {
                Debug.WriteLine("Error validating pincode");
            }

            return false;
        }

        /// <summary>
        ///     Returns the global mobile policy stored.
        /// </summary>
        /// <returns></returns>
        private static MobilePolicy GetMobilePolicy()
        {
            string retrieved = AuthStorageHelper.GetAuthStorageHelper().RetrievePincode();
            if (retrieved != null)
            {
                return JsonConvert.DeserializeObject<MobilePolicy>(retrieved);
            }
            return null;
        }

        /// <summary>
        ///     Stores the pincode and associated mobile policy information including pin length and screen lock timeout.
        /// </summary>
        /// <param name="policy"></param>
        /// <param name="pincode"></param>
        public static void StorePincode(MobilePolicy policy, string pincode)
        {
            string hashed = GenerateEncryptedPincode(pincode);
            var mobilePolicy = new MobilePolicy
            {
                ScreenLockTimeout = policy.ScreenLockTimeout,
                PinLength = policy.PinLength,
                PincodeHash = Encryptor.Encrypt(hashed, pincode)
            };
            AuthStorageHelper.GetAuthStorageHelper().PersistPincode(mobilePolicy);
        }

        /// <summary>
        ///     This will wipe out the pincode and associated data.
        /// </summary>
        public static void WipePincode()
        {
            AuthStorageHelper auth = AuthStorageHelper.GetAuthStorageHelper();
            auth.DeletePincode();
            auth.DeleteData(PinBackgroundedTimeKey);
            auth.DeleteData(PincodeRequired);
        }

        /// <summary>
        ///     This will return true if there is a master pincode set.
        /// </summary>
        /// <returns></returns>
        public static bool IsPincodeSet()
        {
            return AuthStorageHelper.GetAuthStorageHelper().RetrievePincode() != null;
        }

        /// <summary>
        ///     This will return true if a pincode is required before the app can be accessed.
        /// </summary>
        /// <returns></returns>
        public static bool IsPincodeRequired()
        {
            AuthStorageHelper auth = AuthStorageHelper.GetAuthStorageHelper();
            // a flag is set if the timer was exceeded at some point. Automatically return true if the flag is set.
            bool required = auth.RetrieveData(PincodeRequired) != null;
            if (required)
            {
                return true;
            }
            if (IsPincodeSet())
            {
                MobilePolicy policy = GetMobilePolicy();
                if (policy != null)
                {
                    string time = auth.RetrieveData(PinBackgroundedTimeKey);
                    if (time != null)
                    {
                        DateTime previous = DateTime.Parse(time);
                        DateTime current = DateTime.Now.ToUniversalTime();
                        TimeSpan diff = current.Subtract(previous);
                        if (diff.Minutes >= policy.ScreenLockTimeout)
                        {
                            // flag that requires pincode to be entered in the future. Until the flag is deleted a pincode will be required.
                            auth.PersistData(true, PincodeRequired, time);
                            return true;
                        }
                    }
                }
            }
            // We aren't requiring pincode, so remove the flag.
            auth.DeleteData(PincodeRequired);
            return false;
        }

        /// <summary>
        ///     Clear the pincode flag.
        /// </summary>
        internal static void Unlock()
        {
            AuthStorageHelper.GetAuthStorageHelper().DeleteData(PincodeRequired);
            SavePinTimer();
        }

        public static void SavePinTimer()
        {
            MobilePolicy policy = GetMobilePolicy();
            Account account = AccountManager.GetAccount();
            if (account != null && policy != null && policy.ScreenLockTimeout > 0)
            {
                AuthStorageHelper.GetAuthStorageHelper()
                    .PersistData(true, PinBackgroundedTimeKey, DateTime.Now.ToUniversalTime().ToString());
            }
        }

        public static void TriggerBackgroundedPinTimer()
        {
            Account account = AccountManager.GetAccount();
            MobilePolicy policy = GetMobilePolicy();
            bool required = IsPincodeRequired();
            if (account != null)
            {
                if (required || (policy == null && account.Policy != null))
                {
                    LaunchPincodeScreen();
                }
            }
            else if (!required)
            {
                AuthStorageHelper.GetAuthStorageHelper().DeleteData(PinBackgroundedTimeKey);
            }
        }

        /// <summary>
        ///     This method will launch the pincode screen if the policy requires it.
        ///     If determined that no pincode screen is required, the flag requiring the pincode will be cleared.
        /// </summary>
        public static async void LaunchPincodeScreen()
        {
            var frame = Window.Current.Content as Frame;
            if (frame != null && !(typeof (PincodeDialog).Equals(frame.SourcePageType)))
            {
                await frame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    Account account = AccountManager.GetAccount();
                    if (account != null)
                    {
                        PincodeOptions options = null;
                        bool required = IsPincodeRequired();
                        if (account.Policy != null && !IsPincodeSet())
                        {
                            options = new PincodeOptions(PincodeOptions.PincodeScreen.Create, account, "");
                        }
                        else if (required)
                        {
                            MobilePolicy policy = GetMobilePolicy();
                            if (account.Policy != null)
                            {
                                if (policy.ScreenLockTimeout < account.Policy.ScreenLockTimeout)
                                {
                                    policy.ScreenLockTimeout = account.Policy.ScreenLockTimeout;
                                    AuthStorageHelper.GetAuthStorageHelper().PersistPincode(policy);
                                }
                                if (policy.PinLength < account.Policy.PinLength)
                                {
                                    options = new PincodeOptions(PincodeOptions.PincodeScreen.Create, account, "");
                                }
                                else
                                {
                                    options = new PincodeOptions(PincodeOptions.PincodeScreen.Locked, account, "");
                                }
                            }
                            else
                            {
                                options = new PincodeOptions(PincodeOptions.PincodeScreen.Locked, account, "");
                            }
                        }
                        if (options != null)
                        {
                            frame.Navigate(typeof (PincodeDialog), options);
                        }
                    }
                });
            }
        }

        public static void StartIdleTimer()
        {
            if (IsPincodeSet())
            {
                MobilePolicy policy = GetMobilePolicy();
                IdleTimer.Interval = TimeSpan.FromMinutes(policy.ScreenLockTimeout);
                if (IdleTimer.IsEnabled)
                {
                    IdleTimer.Stop();
                }
                SavePinTimer();
                IdleTimer.Start();
            }
        }


        private static void IdleTimer_Tick(object sender, object e)
        {
            TriggerBackgroundedPinTimer();
        }
    }
}