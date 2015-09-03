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
using Windows.UI;
using Salesforce.SDK.Source.Settings;

namespace Salesforce.Sample.NativeSmartStore.Settings
{
    /// <summary>
    /// Implement this class to configure the settings for your application.  You can find instructions on how to create a connected app from the included website.
    /// https://help.salesforce.com/apex/HTViewHelpDoc?id=connected_app_create.htm
    /// </summary>
    class Config : SalesforceConfig
    {
        /// <summary>
        /// This should return the client id generated when you create a connected app through Salesforce.
        /// </summary>
        public override string ClientId
        {
            get { return "3MVG9Iu66FKeHhINkB1l7xt7kR8czFcCTUhgoA8Ol2Ltf1eYHOU4SqQRSEitYFDUpqRWcoQ2.dBv_a1Dyu5xa"; }
        }

        /// <summary>
        /// This should return the callback url generated when you create a connected app through Salesforce.
        /// </summary>
        public override string CallbackUrl
        {
            get { return "testsfdc:///mobilesdk/detect/oauth/done"; }
        }

        /// <summary>
        /// Return the scopes that you wish to use in your app. Limit to what you actually need, try to refrain from listing all scopes.
        /// </summary>
        public override string[] Scopes
        {
            get { return new string[] { "api", "web" }; }
        }

        public override Color? LoginBackgroundColor
        {
            get { return Colors.DarkSeaGreen; }
        }

        public override string ApplicationTitle
        {
            get { return "Native SmartStore Sample"; }
        }

        public override Uri LoginBackgroundLogo
        {
            get { return null; }
        }

        /// <summary>
        /// Set this propeert to true if you want the Title to show up on start screen of the app.
        /// </summary>
        public override bool IsApplicationTitleVisible
        {
            get { return true; }
        }
    }
}
