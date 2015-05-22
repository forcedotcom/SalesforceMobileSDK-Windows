// For an introduction to the Blank template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkId=232509
(function() {
    "use strict";

    var app = WinJS.Application;
    var activation = Windows.ApplicationModel.Activation;
    var oauth = new SalesforceJS.OAuth2();

    var fetchServers = function () {
        var listItemsHtml = document.querySelector('#servers');
        for (var i = 0; i < oauth.servers.serverList.length; i++) {
            var server = oauth.servers.serverList[i];
            var buttonli = document.createElement("li");
            var buttondiv = document.createElement("div");
            var userButton = document.createElement("button");
            buttonli.setAttribute("class", "table-view-cell");
            buttonli.setAttribute("align", "center");
            buttondiv.setAttribute("class", "media-body");
            userButton.addEventListener("click", serverItem(server), false);
            userButton.innerText = "Login User to: " + server.address;
            buttondiv.appendChild(userButton);
            buttonli.appendChild(buttondiv);
            listItemsHtml.appendChild(buttonli);
        }
    }

    var fetchUsers = function() {
        var accounts;
        var account;

        oauth.getUser(function success(result) {
            account = result;
        });
        oauth.getUsers(function success(result) {
            accounts = result;
        });
        var buttonli;
        var buttondiv;
        var userButton;
        var listItemsHtml = document.querySelector('#users');
        if (accounts != null) {
            for (var x = 0; x < accounts.length; x++) {
                var current = accounts[x];
                if (current.userId == account.userId) {
                    var li = document.createElement("li");
                    var div = document.createElement("div");

                    li.setAttribute("class", "table-view-cell");
                    div.setAttribute("class", "media-body");

                    div.innerHTML = current.userName;
                    li.appendChild(div);
                    listItemsHtml.appendChild(li);
                } else {
                    buttonli = document.createElement("li");
                    buttondiv = document.createElement("div");
                    userButton = document.createElement("button");
                    buttonli.setAttribute("class", "table-view-cell");
                    buttonli.setAttribute("align", "center");
                    buttondiv.setAttribute("class", "media-body");
                    userButton.addEventListener("click", userItem(current), false);
                    userButton.innerText = current.userName;
                    buttondiv.appendChild(userButton);
                    buttonli.appendChild(buttondiv);
                    listItemsHtml.appendChild(buttonli);
                }
            }
            buttonli = document.createElement("li");
            buttondiv = document.createElement("div");
            var logoutButton = document.createElement("button");
            buttonli.setAttribute("class", "table-view-cell");
            buttonli.setAttribute("align", "center");
            buttondiv.setAttribute("class", "media-body");
            logoutButton.addEventListener("click", logout, false);
            logoutButton.innerText = "Logout";
            buttondiv.appendChild(logoutButton);
            buttonli.appendChild(buttondiv);
            listItemsHtml.appendChild(buttonli);
        }
    }

    var userItem = function(account) {
        return function () { switchUser(account); }
    }

    var serverItem = function (server) {
        return function () {
            oauth.login(server).done(function () {
                refresh();
            }, function (error) {
                startup();
            });
        }
    }

    var fetchContacts = function () {
        var soql = 'SELECT Id, Name FROM Contact LIMIT 10';
        var rest = Salesforce.SDK.Hybrid.Rest;
        var cm = new rest.ClientManager();
        var client = cm.peekRestClient();

        var request = rest.RestRequest.getRequestForQuery("v31.0", soql);

        var response = client.sendAsync(request).then(function (data) {
            var smart = Salesforce.SDK.Hybrid.SmartStore;
            var smartstore = smart.SmartStore.getSmartStore();
            var users = JSON.parse(data.asString).records;
            var indexspec = [
                new smart.IndexSpec("Id", smart.SmartStoreType.smartString),
              new smart.IndexSpec("FirstName", smart.SmartStoreType.smartString),
              new smart.IndexSpec("LastName", smart.SmartStoreType.smartString) 
            ];
            smartstore.registerSoup("user", indexspec);
            var registered = smartstore.hasSoup("user");
            var listItemsHtml = document.querySelector('#contacts');
            for (var i = 0; i < users.length; i++) {
                var li = document.createElement("li");
                var div = document.createElement("div");

                li.setAttribute("class", "table-view-cell");
                div.setAttribute("class", "media-body");
                div.innerHTML = users[i].Name;
                li.appendChild(div);
                listItemsHtml.appendChild(li);
            }
        
        });
    };
    
    var refresh = function() {
        clearRecords('#servers');
        clearRecords('#users');
        clearRecords('#contacts');
        fetchServers();
        fetchContacts();
        fetchUsers();
    }
    var switchUser = function(account) {
        oauth.switchToUser(account).done(function success() {
            refresh();
        });
    }

    var clearRecords = function(table) {
        var elmtTable = document.querySelector(table);
        var tableRows = elmtTable.getElementsByTagName('li');
        var rowCount = tableRows.length;

        for (var x=rowCount-1; x>=0; x--) {
            elmtTable.removeChild(tableRows[x]);
        }
    }

    var logout = function() {
        oauth.logout()
            .then(function() {
                var accounts;
                oauth.getUsers(function success(result) {
                    accounts = result;
                });
                if (accounts.length > 0) {
                    switchUser(accounts[0]);
                } else {
                    startup();
                }
            });
    }

    var startup = function () {
        oauth.configureOAuth("data/bootconfig.json", null)
            .then(function () {
                oauth.getUsers(function success(result) {
                    refresh();
                    
                }, function failure(result) {
                    oauth.loginDefaultServer().done(function () {
                        refresh();
                    }, function (error) {
                        startup();
                    });
                });   
            });
    }

    app.onactivated = function (args) {
        if (args.detail.kind === activation.ActivationKind.launch) {
            if (args.detail.previousExecutionState !== activation.ApplicationExecutionState.terminated) {
                startup();
            } else {
                startup();
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
