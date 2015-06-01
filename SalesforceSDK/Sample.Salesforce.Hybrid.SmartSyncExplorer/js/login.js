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
