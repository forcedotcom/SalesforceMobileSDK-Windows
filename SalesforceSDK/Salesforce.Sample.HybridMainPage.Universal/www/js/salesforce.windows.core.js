/*
* Copyright (c) 2015, salesforce.com, inc.
* All rights reserved.
*
* Redistribution and use in source and binary forms, with or without modification, are permitted provided
* that the following conditions are met:
*
* Redistributions of source code must retain the above copyright notice, this list of conditions and the
* following disclaimer.
*
* Redistributions in binary form must reproduce the above copyright notice, this list of conditions and
* the following disclaimer in the documentation and/or other materials provided with the distribution.
*
* Neither the name of salesforce.com, inc. nor the names of its contributors may be used to endorse or
* promote products derived from this software without specific prior written permission.
*
* THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED
* WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A
* PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
* ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
* TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
* HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
* NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
* POSSIBILITY OF SUCH DAMAGE.
*/
var SalesforceJS;
(function (SalesforceJS) {
    var OAuth2 = (function () {
        function OAuth2() {
            this.auth = Salesforce.SDK.Hybrid.Auth;
            this.rest = Salesforce.SDK.Hybrid.Rest;
        }

        OAuth2.prototype.getAuthCredentials = function (success, fail) {
            var account = this.auth.HybridAccountManager.getAccount();
            if (account != null) {
                success(this.auth.Account.toJson(account));
            }
            else {
                fail();
            }
        };
        OAuth2.prototype.forcetkRefresh = function (success, fail) {
            var account = this.auth.HybridAccountManager.getAccount();
            if (account != null) {
                this.auth.OAuth2.refreshAuthToken(account).done(function (resolve) {
                    success(resolve);
                }, function (reject) {
                    fail(reject);
                });
            }
            else {
                fail(null);
            }
        };
        OAuth2.prototype.getUsers = function (success, fail) {
            var imapAccounts = this.auth.HybridAccountManager.getAccounts();
            if (imapAccounts != null && imapAccounts.size > 0) {
                var first = imapAccounts.first();
                var accounts = new Array();
                accounts.push(first.current.value);
                while (first.moveNext()) {
                    accounts.push(first.current.value);
                }
                success(accounts);
            }
            else {
                fail(null);
            }
        };
        OAuth2.prototype.getUser = function (success, fail) {
            var account = this.auth.HybridAccountManager.getAccount();
            if (account != null) {
                success(account);
            }
            else {
                fail(null);
            }
        };
        OAuth2.prototype.switchToUser = function (account) {
            return this.auth.HybridAccountManager.switchToAccount(account);
        };
        
        return OAuth2;
    })();
    SalesforceJS.OAuth2 = OAuth2;
})(SalesforceJS || (SalesforceJS = {}));
