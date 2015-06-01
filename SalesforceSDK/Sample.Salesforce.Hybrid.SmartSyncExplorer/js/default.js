/*
 * Copyright (c) 2015, salesforce.com, inc.
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

// For an introduction to the Blank template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkId=232509
(function () {
    "use strict";

    var app = WinJS.Application;
    var activation = Windows.ApplicationModel.Activation;
    var oauth = new SalesforceJS.OAuth2();
    var contactList;
    
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

    var synccontacts = function () {

        var account;
        oauth.getUser(function success(result) {
            account = result;
        });
        var smart = Salesforce.SDK.Hybrid.SmartStore;
        var smartstore = smart.SmartStore.getSmartStore();

        var indexspec = [
            new smart.IndexSpec("Id", smart.SmartStoreType.smartString),
            new smart.IndexSpec("Name", smart.SmartStoreType.smartString),
            new smart.IndexSpec("__locallyCreated__", smart.SmartStoreType.smartString),
            new smart.IndexSpec("__locallyUpdated__", smart.SmartStoreType.smartString),
            new smart.IndexSpec("__locallyDeleted__", smart.SmartStoreType.smartString),
            new smart.IndexSpec("__local__", smart.SmartStoreType.smartString)

        ];

        smartstore.registerSoup("user", indexspec);
        var registered = smartstore.hasSoup("user");

        var manager = Salesforce.SDK.Hybrid.SmartSync;
        var syncmanager = manager.SyncManager.getInstance(account, null);

        var contactFields = ["FirstName", "LastName", "Title", "HomePhone", "Email", "Department", "MailingStreet"];
        var soqlQuery = manager.Manager.SOQLBuilder.getInstanceWithFields(contactFields).from("Contact").limit(4000).build();
        var target = new manager.Models.SoqlSyncDownTarget(soqlQuery);
        var options = manager.Models.SyncOptions.optionsForSyncDown(manager.Models.MergeModeOptions.leaveIfChanged);
        var callback = "";

        var syncState = syncmanager.syncDown(target.asJson(), "user", callback, options);
        removedeleted();
        var syncUpTarget = new manager.Models.SyncUpTarget();
        syncmanager.syncUp(syncUpTarget, options, "user", callback);
    }

    function removedeleted() {
        var smart = Salesforce.SDK.Hybrid.SmartStore;
        var smartstore = smart.SmartStore.getSmartStore();

        var querySpec = smart.QuerySpec.buildAllQuerySpec("user", "Id", smart.SqlOrder.asc, 4000);
        var result = smartstore.query(querySpec, 0);
        var contactObject = JSON.parse(result);
        for (var i = 0; i < contactObject.length; i++) {
            if (contactObject[i].__locally_deleted__ == true) {
                var element = document.getElementById(contactObject[i].Id);
                var div = document.querySelector('#contacts');
                div.removeChild(element);
            }
        }


    }

    var fetchUsers = function () {
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
        var syncButton = document.querySelector("#cmdSync");
        syncButton.addEventListener("click", synccontacts, false);
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

    var userItem = function (account) {
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
            contactList = users;
            var indexspec = [
                new smart.IndexSpec("Id", smart.SmartStoreType.smartString),
              new smart.IndexSpec("Name", smart.SmartStoreType.smartString)
            ];
            if (smartstore.hasSoup("user")) {
                smartstore.dropSoup("user");
            }
            smartstore.registerSoup("user", indexspec);
            var registered = smartstore.hasSoup("user");
            var listItemsHtml = document.querySelector('#contacts');
            for (var i = 0; i < users.length; i++) {
                (function() {
                    if (registered) {
                        smartstore.upsert("user", JSON.stringify(users[i]));
                    }
                    var li = document.createElement("li");
                    var div = document.createElement("div");
                    var span = document.createElement("span");
                    var button = document.createElement("button");

                    li.setAttribute("class", "table-view-cell");
                    div.setAttribute("class", "media-body");
                    li.setAttribute("id", users[i].Id);

                    var userId = users[i].Id;
                    var item = smartstore.retrieve("user", [smartstore.lookupSoupEntryId("user", "Id", userId)]);

                    span.innerHTML = users[i].Name;
                    button.addEventListener("click", function() {
                        var jobject = JSON.parse(item);
                        jobject[0].__local__ = true;
                        jobject[0].__locally_deleted__ = true;
                        smartstore.upsert("user", JSON.stringify(jobject[0]));
                        var element = document.getElementById(userId);
                        element.className += (" danger");
                        button.style.visibility = 'hidden';
                    }, false);
                    button.innerText = "Delete";

                    div.appendChild(span);
                    div.appendChild(button);
                    li.appendChild(div);
                    listItemsHtml.appendChild(li);

                }());
            }
            var qspec = smart.QuerySpec.buildAllQuerySpec("user", "Name", smart.SqlOrder.asc, 20);
                var smartResults = smartstore.query(qspec, 0);
                var json = JSON.parse(smartResults);
            
        });

        var deletecontact = function (item, store) {
            
            var jobject = JSON.parse(item);
            jobject[0].__local__ = true;
            jobject[0].__locally_deleted__ = true;
            store.upsert("user", JSON.stringify(jobject[0]));
        }
    };

    

    var refresh = function () {
        clearRecords('#servers');
        clearRecords('#users');
        clearRecords('#contacts');
        fetchServers();
        fetchContacts();
        fetchUsers();
    }
    var switchUser = function (account) {
        oauth.switchToUser(account).done(function success() {
            refresh();
        });
    }

    var clearRecords = function (table) {
        var elmtTable = document.querySelector(table);
        var tableRows = elmtTable.getElementsByTagName('li');
        var rowCount = tableRows.length;

        for (var x = rowCount - 1; x >= 0; x--) {
            elmtTable.removeChild(tableRows[x]);
        }
    }

    var logout = function () {
        oauth.logout()
            .then(function () {
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
