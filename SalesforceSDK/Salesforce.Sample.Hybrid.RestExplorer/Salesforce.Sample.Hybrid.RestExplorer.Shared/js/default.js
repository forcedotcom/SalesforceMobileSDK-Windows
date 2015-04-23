// For an introduction to the Blank template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkID=392286
(function () {
    "use strict";

    var app = WinJS.Application;
    var activation = Windows.ApplicationModel.Activation;
    var authzInProgress = false; 

    function test() {
        var object = window.Salesforce;
        var accounts = Salesforce.SDK.Hybrid.Auth.HybridAccountManager.getAccounts();

        for (i = 0; i < accounts.length; ++i) {
            console.log(accounts[i]);
            console.log(accounts[i].UserId);
        }
    }

    function sfdcLogin() {
        WinJS.xhr({ url: "data/bootconfig.json" }).done(function complete(response) {
            var auth = Salesforce.SDK.Hybrid.Auth;
            auth.HybridAccountManager.initEncryption();
            var jsonResult = JSON.parse(response.responseText);
            var endUri = new Windows.Foundation.Uri(jsonResult.oauthRedirectURI);
            var options = new auth.LoginOptions("https://test.salesforce.com/", jsonResult.remoteAccessConsumerKey, jsonResult.oauthRedirectURI, jsonResult.oauthScopes);
            var startUriStr = auth.HybridAccountManager.computeAuthorizationUrl(options);
            var startUri = new Windows.Foundation.Uri(startUriStr);
            authzInProgress = true;
            Windows.Security.Authentication.Web.WebAuthenticationBroker.authenticateAsync(
                    Windows.Security.Authentication.Web.WebAuthenticationOptions.none, startUri, endUri)
                .done(function(result) {
                var responseResult = new Windows.Foundation.Uri(result.responseData);
                var authResponse = responseResult.fragment.substring(1);
                auth.HybridAccountManager.createNewAccount(options, authResponse).then(function () {
                    authzInProgress = false;
                    WinJS.Navigation.navigate(jsonResult.startPage);
                });
            }, function(err) {
                WinJS.log("Error returned by WebAuth broker: " + err, "Web Authentication SDK Sample", "error");
                document.getElementById("AnyServiceDebugArea").value += " Error Message: " + err.message + "\r\n";
                authzInProgress = false;
            });
        });
    }

    app.onactivated = function (args) {
        if (args.detail.kind === activation.ActivationKind.launch) {
            if (args.detail.previousExecutionState !== activation.ApplicationExecutionState.terminated) {
                // TODO: This application has been newly launched. Initialize
                var p = WinJS.UI.processAll().then(function() {
                    sfdcLogin();
                });
            } else {
                // TODO: This application has been reactivated from suspension.
                // Restore application state here.
            }
            args.setPromise(WinJS.UI.processAll());
        }
    };

    app.oncheckpoint = function (args) {
        // TODO: This application is about to be suspended. Save any state
        // that needs to persist across suspensions here. You might use the
        // WinJS.Application.sessionState object, which is automatically
        // saved and restored across suspension. If you need to complete an
        // asynchronous operation before your application is suspended, call
        // args.setPromise().
    };

    app.start();
})();