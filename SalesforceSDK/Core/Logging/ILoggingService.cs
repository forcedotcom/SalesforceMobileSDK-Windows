using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Core.Logging
{
    public interface ILoggingService
    {
        void SendToCustomLogger(string message, LoggingLevel loggingLevel);
        void SendToCustomLogger(Exception exception, LoggingLevel loggingLevel);
    }
}
