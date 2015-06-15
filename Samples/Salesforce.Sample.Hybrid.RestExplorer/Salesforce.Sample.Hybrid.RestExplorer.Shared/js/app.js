(function () {

    "use strict";

    /* This method will fetch a list of user records from salesforce. 
    Just change the soql query to fetch another sobject. */
    var fetchRecords = function () {
        var soql = 'SELECT Id, Name FROM User LIMIT 10';
        var rest = Salesforce.SDK.Hybrid.Rest;
        var cm = new rest.ClientManager();
        var client = cm.peekRestClient();

        var request = rest.RestRequest.getRequestForQuery("v31.0", soql);

        var response = client.sendAsync(request).then( function (data) {
            var users = data.records;

            var listItemsHtml = '';
            for (var i = 0; i < users.length; i++) {
                listItemsHtml += ('<li class="table-view-cell"><div class="media-body">' + users[i].Name + '</div></li>');
            }

            document.querySelector('#users').innerHTML = listItemsHtml;
        });
    };

    fetchRecords();
})();