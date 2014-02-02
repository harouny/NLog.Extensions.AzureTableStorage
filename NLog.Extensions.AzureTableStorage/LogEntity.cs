using System.Globalization;
using System.Linq;
using Microsoft.WindowsAzure.Storage.Table;

namespace NLog.Extensions.AzureTableStorage
{
    public class LogEntity : TableEntity
    {
        private readonly object _syncRoot = new object();

        public LogEntity(LogEventInfo logEvent, string layoutMessage)
        {
            lock (_syncRoot)
            {
                LoggerName = logEvent.LoggerName;
                LogTimeStamp = logEvent.TimeStamp.ToString("g");
                Level = logEvent.Level.Name;
                Message = logEvent.FormattedMessage;
                MessageWithLayout = layoutMessage;
                if (logEvent.Exception != null)
                {
                    Exception = logEvent.Exception.ToString();
                    if (logEvent.Exception.InnerException != null)
                    {
                        InnerException = logEvent.Exception.InnerException.ToString();
                    }
                }
                if (logEvent.Parameters != null)
                {
                    Parameters = logEvent.Parameters
                    .Select(o => o.ToString())
                    .Aggregate((o, o1) => (o == null ? "Null" : o.ToString(CultureInfo.InvariantCulture)) + ", " + (o1 == null ? "Null" : o1.ToString(CultureInfo.InvariantCulture)));

                }
                if (logEvent.StackTrace != null)
                {
                    StackTrace = logEvent.StackTrace.ToString();
                }
                RowKey = logEvent.TimeStamp.Ticks.ToString(CultureInfo.InvariantCulture);
                PartitionKey = LoggerName;
            }
            
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
        public string MessageWithLayout { get; set; }
    }
}
