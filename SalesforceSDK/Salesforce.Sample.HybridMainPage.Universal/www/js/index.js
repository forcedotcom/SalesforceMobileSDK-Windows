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

    var request = rest.RestRequest.getRequestForQuery("v31.0", soql);

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
