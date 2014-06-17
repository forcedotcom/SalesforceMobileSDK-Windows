using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salesforce.SDK.Auth
{
    internal class PincodeOptions
    {
        public enum PincodeScreen
        {
            Create,
            Confirm,
            Locked
        }

        public Account User { get; private set; }
        public PincodeScreen Screen { get; private set; }
        public string Passcode { get; private set; }

        public PincodeOptions(PincodeScreen screen, Account user, string passcode)
        {
            Screen = screen;
            User = user;
            Passcode = passcode;
        }
    }
}
