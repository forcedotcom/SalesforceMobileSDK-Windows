/// <reference path="../typings/WinJS-3.0.d.ts"/>
/// <reference path="../typings/Salesforce.SDK.Hybrid.d.ts"/>
/// <reference path="../typings/winrt.d.ts"/>
var SalesforceJS;
(function (SalesforceJS) {
    var BootConfig = (function () {
        function BootConfig() {
        }
        return BootConfig;
    })();
    SalesforceJS.BootConfig = BootConfig;
    var Server = (function () {
        function Server(n, a) {
            this.name = n;
            this.address = a;
        }
        return Server;
    })();
    SalesforceJS.Server = Server;
    var ServerConfig = (function () {
        function ServerConfig() {
        }
        return ServerConfig;
    })();
    SalesforceJS.ServerConfig = ServerConfig;
    var OAuth2 = (function () {
        function OAuth2() {
            this.config = new BootConfig();
            this.servers = new ServerConfig();
        }
        OAuth2.prototype.configureOAuth = function (bootConfig, serverConfig) {
            if ((/^\s*$/).test(bootConfig)) {
                bootConfig = "data/bootconfig.json";
            }
            this.servers = new ServerConfig();
            var self = this;
            return new WinJS.Promise(function (resolve) {
                WinJS.xhr({ url: bootConfig }).then(function (response) {
                    self.loadBootConfig(response.responseText);
                }).then(function () {
                    WinJS.xhr({ url: "data/servers.xml" }).done(function (response) {
                        self.loadServerXml(response);
                        resolve();
                    });
                });
            });
        };
        OAuth2.prototype.loadBootConfig = function (response) {
            this.config = JSON.parse(response);
        };
        OAuth2.prototype.loadServerXml = function (response) {
            var data = response.responseXML;
            var serverItems = data.querySelectorAll("servers > server");
            var serversList = new Array();
            for (var i = 0; i < serverItems.length; i++) {
                var server = serverItems[i];
                serversList.push({ name: server.attributes.getNamedItem("name").value, address: server.attributes.getNamedItem("url").value });
            }
            this.servers.serverList = serversList;
        };
        OAuth2.prototype.loginDefaultServer = function () {
            return this.login(this.servers.serverList[0]);
        };
        OAuth2.prototype.login = function (server) {
            var auth = Salesforce.SDK.Hybrid.Auth;
            auth.HybridAccountManager.initEncryption();
            var boot = this.config;
            var account = auth.HybridAccountManager.getAccount();
            return new WinJS.Promise(function (resolve, reject, progress) {
                if (account == null) {
                    var options = new auth.LoginOptions(server.address, boot.remoteAccessConsumerKey, boot.oauthRedirectURI, boot.oauthScopes);
                    var startUriStr = auth.OAuth2.computeAuthorizationUrl(options);
                    var startUri = new Windows.Foundation.Uri(startUriStr);
                    var endUri = new Windows.Foundation.Uri(boot.oauthRedirectURI);
                    Windows.Security.Authentication.Web.WebAuthenticationBroker.authenticateAsync(Windows.Security.Authentication.Web.WebAuthenticationOptions.none, startUri, endUri).done(function (result) {
                        var responseResult = new Windows.Foundation.Uri(result.responseData);
                        var authResponse = responseResult.fragment.substring(1);
                        auth.HybridAccountManager.createNewAccount(options, authResponse).done(function (newAccount) {
                            if (newAccount != null) {
                                resolve(newAccount);
                            }
                            else {
                                reject(result.responseErrorDetail);
                            }
                        });
                    }, function (err) {
                        reject(err);
                    });
                }
                else {
                    auth.OAuth2.refreshAuthToken(account);
                    resolve(account);
                }
            });
        };
        return OAuth2;
    })();
    SalesforceJS.OAuth2 = OAuth2;
})(SalesforceJS || (SalesforceJS = {}));
//# sourceMappingURL=salesforce.windows.core.js.map