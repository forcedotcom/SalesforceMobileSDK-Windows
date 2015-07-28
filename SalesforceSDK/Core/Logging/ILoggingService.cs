using System;

namespace Salesforce.SDK.Logging
{
    public interface ILoggingService
    {
        void Log(string message, LoggingLevel loggingLevel);
        void Log(Exception exception, LoggingLevel loggingLevel);
    }
}
