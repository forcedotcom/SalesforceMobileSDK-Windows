/// <reference path="../typings/WinJS-3.0.d.ts"/>
/// <reference path="../typings/Salesforce.SDK.Hybrid.d.ts"/>
/// <reference path="../typings/winrt.d.ts"/>

module SalesforceJS {

    export class BootConfig {
        public remoteAccessConsumerKey: string;
        public oauthRedirectURI: string;
        public oauthScopes: string[];
        public isLocal: boolean;
        public startPage: string;
        public errorPage: string;
        public shouldAuthenticate: boolean;
        public attemptOfflineLoad: boolean;
    }

    export class Server {
        name: string;
        address: string;

        constructor(n: string, a: string) {
            this.name = n;
            this.address = a;
        }
    }

    export class ServerConfig {
        public allowNewConnections: boolean;
        public serverList: Array<Server>;
    }

    export class OAuth2 {

        private config: BootConfig;
        private servers: ServerConfig;

        constructor() {
            this.config = new BootConfig();
            this.servers = new ServerConfig();
        }

        public configureOAuth(bootConfig: string, serverConfig: string) {
            if ((/^\s*$/).test(bootConfig)) {
                bootConfig = "data/bootconfig.json";
            }
            this.servers = new ServerConfig();
            var self = this;
            return new WinJS.Promise(function(resolve) {
                WinJS.xhr({ url: bootConfig }).then((response) => {
                    self.loadBootConfig(response.responseText);
                }).then(() => {
                    WinJS.xhr({ url: "data/servers.xml" }).done((response) => {
                        self.loadServerXml(response);
                        resolve();
                    });
                });
            });
        }

        private loadBootConfig(response: string) {
            this.config = JSON.parse(response);
        }

        private loadServerXml(response: XMLHttpRequest) {
            var data = response.responseXML;
            var serverItems = data.querySelectorAll("servers > server");
            var serversList = new Array<Server>();
            for (var i = 0; i < serverItems.length; i++) {
                var server = serverItems[i];
                serversList.push({ name: server.attributes.getNamedItem("name").value, address: server.attributes.getNamedItem("url").value });
            }
            this.servers.serverList = serversList;
        }

        public loginDefaultServer() {
            return this.login(this.servers.serverList[0]);
        }

        public login(server: Server) {
            var auth = Salesforce.SDK.Hybrid.Auth;
            auth.HybridAccountManager.initEncryption();
            var boot = this.config;
            var account = auth.HybridAccountManager.getAccount();
            return new WinJS.Promise(function(resolve, reject, progress) {
                if (account == null) {
                    var options = new auth.LoginOptions(server.address, boot.remoteAccessConsumerKey, boot.oauthRedirectURI, boot.oauthScopes);
                    var startUriStr = auth.OAuth2.computeAuthorizationUrl(options);
                    var startUri = new Windows.Foundation.Uri(startUriStr);
                    var endUri = new Windows.Foundation.Uri(boot.oauthRedirectURI);
                    Windows.Security.Authentication.Web.WebAuthenticationBroker.authenticateAsync(
                            Windows.Security.Authentication.Web.WebAuthenticationOptions.none, startUri, endUri)
                        .done(function(result) {
                            var responseResult = new Windows.Foundation.Uri(result.responseData);
                            var authResponse = responseResult.fragment.substring(1);
                            auth.HybridAccountManager.createNewAccount(options, authResponse).done(function(newAccount) {
                                if (newAccount != null) {
                                    resolve(newAccount);
                                } else {
                                    reject(result.responseErrorDetail);
                                }

                            });
                        }, function(err) {
                            reject(err);
                        });
                } else {
                    auth.OAuth2.refreshAuthToken(account);
                    resolve(account);
                }
            });
        }
    }
}
