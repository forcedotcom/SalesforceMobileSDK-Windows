using System;

namespace Salesforce.SDK.Exceptions
{
    public class DeviceOfflineException : Exception
    {
        public DeviceOfflineException() : base() { }

        public DeviceOfflineException(string msg) : base(msg) { }

        public DeviceOfflineException(string msg, Exception innerException) : base(msg, innerException) { }
    }
}
