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

var oauth = new SalesforceJS.OAuth2();

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
        
    }
}

var userItem = function (account) {
    return function () { switchUser(account); }
}

var fetchContacts = function () {
    var soql = 'SELECT Id, Name FROM Contact LIMIT 10';
    var rest = Salesforce.SDK.Hybrid.Rest;
    var cm = new rest.ClientManager();
    var client = cm.peekRestClient();
    var request = rest.RestRequest.getRequestForQuery(rest.ApiVersionStrings.versionNumber, soql);

    var response = client.sendAsync(request).then(function (data) {
        var smart = Salesforce.SDK.Hybrid.SmartStore;
        var smartstore = smart.SmartStore.getSmartStore();
        var users = JSON.parse(data.asString).records;
        var indexspec = [
            new smart.IndexSpec("Id", smart.SmartStoreType.smartString),
            new smart.IndexSpec("Name", smart.SmartStoreType.smartString),
        ];
        if (smartstore.hasSoup("user")) {
            smartstore.dropSoup("user");
        }
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
            if (registered) {
                var result = smartstore.upsert("user", JSON.stringify(users[i]));
            }
        }
        var qspec = smart.QuerySpec.buildAllQuerySpec("user", "Name", smart.SqlOrder.asc, 20);
        var smartResults = smartstore.query(qspec, 0);
        var json = JSON.parse(smartResults);
    });
};

var refresh = function () {
    clearRecords('#users');
    clearRecords('#contacts');
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
