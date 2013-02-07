using Salesforce.SDK.Auth;
using Salesforce.SDK.Source.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salesforce.SDK.Auth
{
    public interface IAuthHelper
    {
        void StartLoginFlow(LoginOptions loginOptions);

        void EndLoginFlow(LoginOptions loginOptions, AuthResponse authResponse);

        void PersistCredentials(Account account);

        Account RetrievePersistedCredentials();

        void Logout();
    }
}
