using Newtonsoft.Json;
using System;

namespace Salesforce.SDK.Hybrid
{
    /// <summary>
    /// Cordova plugin for Salesforce OAuth
    /// </summary>
    public class SalesforceOAuthPlugin 
    {
        /// <summary>
        /// Native implementation for "authenticate" action
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
        /// Native implementation for "getAuthCredentials" action.
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
        /// Native implementation for "getAppHomeUrl" action
        /// </summary>
        public void getAppHomeUrl(object jsVersion)
        {
            String appHomeUrl = HybridMainPage.GetInstance().GetAppHomeUrl();
           // PluginResult result = new PluginResult(PluginResult.Status.OK, appHomeUrl);
           // DispatchCommandResult(result);
        }

        /// <summary>
        /// Native implementation for "logoutCurrentUser" action
        /// </summary>
        public void logoutCurrentUser(object jsVersion)
        {
            HybridMainPage.GetInstance().LogoutCurrentUser();
           // PluginResult result = new PluginResult(PluginResult.Status.OK);
          //  DispatchCommandResult(result);
        }

        /// <summary>
        /// Called when authenticate succeeded
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
        /// Called when authenticate failed
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
    /// Cordova on Windows Phone doesn't take into account the value property on plugin elements in config.xml
    /// So we have to define a com.salesforce.oauth class since that's the plugin name on the javascript side
    /// </summary>
    public class oauth : Salesforce.SDK.Hybrid.SalesforceOAuthPlugin
    {
    }
}
