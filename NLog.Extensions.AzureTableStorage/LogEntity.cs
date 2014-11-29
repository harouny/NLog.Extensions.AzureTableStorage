using System;
using System.Collections;
using System.Globalization;
using System.Text;
using Microsoft.WindowsAzure.Storage.Table;

namespace NLog.Extensions.AzureTableStorage
{
    public class LogEntity : TableEntity
    {
        private readonly object _syncRoot = new object();

        public LogEntity(string partitionKeyPrefix, LogEventInfo logEvent, string layoutMessage)
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
                    if (logEvent.Exception.Data.Count > 0)
                    {
                        ExceptionData = GetExceptionDataAsString(logEvent.Exception);
                    }
                    if (logEvent.Exception.InnerException != null)
                    {
                        InnerException = logEvent.Exception.InnerException.ToString();
                    }
                }
                if (logEvent.StackTrace != null)
                {
                    StackTrace = logEvent.StackTrace.ToString();
                }
                RowKey = logEvent.TimeStamp.Ticks.ToString(CultureInfo.InvariantCulture);
                var loggerInfo = !string.IsNullOrWhiteSpace(partitionKeyPrefix)
                                ? partitionKeyPrefix + "." + LoggerName
                                : LoggerName;
                PartitionKey = String.Format("{0}__{1}__{2}", Environment.MachineName, loggerInfo, Guid.NewGuid());
            }

        }

        private static string GetExceptionDataAsString(Exception exception)
        {
            var data = new StringBuilder();
            foreach (DictionaryEntry entry in exception.Data)
            {
                data.AppendLine(entry.Key + "=" + entry.Value);
            }
            return data.ToString();
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
        public string StackTrace { get; set; }
        public string MessageWithLayout { get; set; }
        public string ExceptionData { get; set; }

    }
}
