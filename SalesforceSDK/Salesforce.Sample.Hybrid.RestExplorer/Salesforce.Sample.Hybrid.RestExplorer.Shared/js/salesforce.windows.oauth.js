var sfdcOAuth = function () {
    this.logout = function(mouseEvent) {
        var rest = Salesforce.SDK.Hybrid.Rest;
        var cm = new rest.ClientManager();
        var client = cm.peekRestClient();
        if (client != null) {
            cm.logout().done(function() {
                location.reload();
            });
        }
    }

    this.login = function() {
        var auth = Salesforce.SDK.Hybrid.Auth;
        auth.HybridAccountManager.initEncryption();
        var account = auth.HybridAccountManager.getAccount();
        return new WinJS.Promise(function(resolve, reject, progress) {
            if (account == null) {
                WinJS.xhr({ url: "data/bootconfig.json" }).then(function complete(response) {
                    var jsonResult = JSON.parse(response.responseText);
                    var endUri = new Windows.Foundation.Uri(jsonResult.oauthRedirectURI);
                    var options = new auth.LoginOptions("https://test.salesforce.com/", jsonResult.remoteAccessConsumerKey, jsonResult.oauthRedirectURI, jsonResult.oauthScopes);
                    var startUriStr = auth.OAuth2.computeAuthorizationUrl(options);
                    var startUri = new Windows.Foundation.Uri(startUriStr);
                    Windows.Security.Authentication.Web.WebAuthenticationBroker.authenticateAsync(
                            Windows.Security.Authentication.Web.WebAuthenticationOptions.none, startUri, endUri)
                        .done(function(result) {
                            var responseResult = new Windows.Foundation.Uri(result.responseData);
                            var authResponse = responseResult.fragment.substring(1);
                            auth.HybridAccountManager.createNewAccount(options, authResponse).done(function(newAccount) {
                                if (newAccount != null) {
                                    resolve(newAccount);
                                } else {
                                    reject(result.errorCode);
                                }

                            });
                        }, function(err) {
                            reject(err);
                        });
                });

            } else {
                auth.OAuth2.refreshAuthToken(account);
                resolve(account);
            }
        });
    }
};

var SFAuth = new SfdcOAuth();