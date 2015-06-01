// For an introduction to the Blank template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkId=232509
(function () {
    "use strict";

    var app = WinJS.Application;
    var activation = Windows.ApplicationModel.Activation;
    var oauth = new SalesforceJS.OAuth2();

    var fetchServers = function () {
        var listItemsHtml = document.querySelector('#serverlist');
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
            listItemsHtml.classList.remove("hideserverlist");
            listItemsHtml.classList.add("showserverlist");
            listItemsHtml.appendChild(buttonli);
        }
    }

    var serverItem = function (server) {
        return function () {
            oauth.login(server).done(function () {
                //refresh();
            }, function (error) {
                startup();
            });
        }
    }

    var startup = function () {
        
        oauth.configureOAuth("data/bootconfig.json", null)
            .then(function () {
                oauth.getUsers(function success(result) {
                    //refresh();
                }, function failure(result) {
                    oauth.loginDefaultServer().done(function () {
                        //refresh();
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
            var connection = document.getElementById("chooseconnection");
            connection.addEventListener('click', fetchServers());
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
