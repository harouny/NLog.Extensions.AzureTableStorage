using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace NLog.Extensions.AzureTableStorage
{
    public class LogEntity : TableEntity
    {
        public LogEntity(LogEventInfo logEvent, string layoutMessage)
        {
            LoggerName = logEvent.LoggerName;
            LogTimeStamp = logEvent.TimeStamp.ToString(CultureInfo.InvariantCulture);
            Level = logEvent.Level.Name;
            Message = logEvent.FormattedMessage;
            LayoutMessage = layoutMessage;
            if (logEvent.Exception != null)
            {
                Exception = logEvent.Exception.ToString();
                if (logEvent.Exception.InnerException != null)
                {
                    InnerException = logEvent.Exception.ToString();
                }
            }
            if (logEvent.Parameters != null)
            {
                Parameters = logEvent.Parameters
                .Select(o => o.ToString())
                .Aggregate((o, o1) => (o == null ? "Null" : o.ToString(CultureInfo.InvariantCulture)) + " , " + (o1 == null ? "Null" : o1.ToString(CultureInfo.InvariantCulture)));

            }
            if (logEvent.StackTrace != null)
            {
                StackTrace = logEvent.StackTrace.ToString();
            }
            RowKey = LoggerName + "-" + logEvent.TimeStamp.Ticks;
            PartitionKey = LoggerName;
        }

        public LogEntity()
        {
        }

        public string LogTimeStamp { get; set; }
        public string Level { get; set; }
        public string LoggerName { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
        public string InnerException { get; set; }
        public string Parameters { get; set; }
        public string StackTrace { get; set; }
        public string LayoutMessage { get; set; }
    }
}
