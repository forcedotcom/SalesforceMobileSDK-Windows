/*
 * Copyright (c) 2013, salesforce.com, inc.
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
using Salesforce.SDK.Hybrid;

namespace Salesforce.SDK.Hybrid
{
    /// <summary>
    ///     Cordova plugin for Salesforce OAuth
    /// </summary>
    public class SalesforceOAuthPlugin
    {
        /// <summary>
        ///     Native implementation for "authenticate" action
        /// </summary>
        public void authenticate(object jsVersion)
        {
            /*
            PhoneHybridMainPage.GetInstance().Authenticate(this);

            // Done
            PluginResult noop = new PluginResult(PluginResult.Status.NO_RESULT);
            noop.KeepCallback = true;
            DispatchCommandResult(noop);
             * */
        }

        /// <summary>
        ///     Native implementation for "getAuthCredentials" action.
        /// </summary>
        public void getAuthCredentials(object jsVersion)
        {
            JSONCredentials credentials = HybridMainPage.GetInstance().GetJSONCredentials();
            if (credentials == null)
            {
                // DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR, "Never authenticated"));
            }
            else
            {
                OnAuthenticateSuccess(credentials);
            }
        }

        /// <summary>
        ///     Native implementation for "getAppHomeUrl" action
        /// </summary>
        public void getAppHomeUrl(object jsVersion)
        {
            String appHomeUrl = HybridMainPage.GetInstance().GetAppHomeUrl();
            // PluginResult result = new PluginResult(PluginResult.Status.OK, appHomeUrl);
            // DispatchCommandResult(result);
        }

        /// <summary>
        ///     Native implementation for "logoutCurrentUser" action
        /// </summary>
        public void logoutCurrentUser(object jsVersion)
        {
            HybridMainPage.GetInstance().LogoutCurrentUser();
            // PluginResult result = new PluginResult(PluginResult.Status.OK);
            //  DispatchCommandResult(result);
        }

        /// <summary>
        ///     Called when authenticate succeeded
        /// </summary>
        /// <param name="credentials"></param>
        public void OnAuthenticateSuccess(JSONCredentials credentials)
        {
            //  PluginResult result = new PluginResult(PluginResult.Status.OK);
            // Doing the serialization ourselfves: we are using JSON.Net but Cordova is using DataContractJsonSerializer
            // result.Message = JsonConvert.SerializeObject(credentials);
            //  DispatchCommandResult(result);
        }

        /// <summary>
        ///     Called when authenticate failed
        /// </summary>
        /// <param name="webException"></param>
        public void OnAuthenticateError(string message)
        {
            // PluginResult result = new PluginResult(PluginResult.Status.ERROR, message);
            //  DispatchCommandResult(result);
        }
    }
}

namespace com.salesforce
{
    /// <summary>
    ///     Cordova on Windows Phone doesn't take into account the value property on plugin elements in config.xml
    ///     So we have to define a com.salesforce.oauth class since that's the plugin name on the javascript side
    /// </summary>
    public class oauth : SalesforceOAuthPlugin
    {
    }
}