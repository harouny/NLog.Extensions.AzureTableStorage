using System;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using Microsoft.WindowsAzure;
using NLog.Targets;

namespace NLog.Extensions.AzureTableStorage
{
    [Target("AzureTableStorage")]
    public class AzureTableStorageTarget : TargetWithLayout
    {
        private TableStorageManager _tableStorageManager;

        [Required]
        public string ConnectionStringKey { get; set; }

        [Required]
        public string TableName { get; set; }

        public string PartitionKeyPrefix { get; set; }
        public string PartitionKeyPrefixKey { get; set; }

        protected override void InitializeTarget()
        {
            base.InitializeTarget();
            ValidateParameters();
            _tableStorageManager = new TableStorageManager(ConnectionStringKey, TableName);

            if (!string.IsNullOrWhiteSpace(PartitionKeyPrefixKey))
                PartitionKeyPrefix = GetStorageAccountPartitionKeyPrefix();
        }

        private void ValidateParameters()
        {
            IsNameValidForTableStorage(TableName);
        }

        private void IsNameValidForTableStorage(string tableName)
        {
            var validator = new AzureStorageTableNameValidator(tableName);
            if (!validator.IsValid())
            {
                throw new NotSupportedException(tableName + " is not a valid name for Azure storage table name.")
                {
                    HelpLink = "http://msdn.microsoft.com/en-us/library/windowsazure/dd179338.aspx"
                };
            }
        }

        protected override void Write( LogEventInfo logEvent)
        {
            if (_tableStorageManager != null)
            {
                var layoutMessage = Layout.Render(logEvent);
                _tableStorageManager.Add(new LogEntity(PartitionKeyPrefix, logEvent, layoutMessage));
            }
        }

        private string GetStorageAccountPartitionKeyPrefix()
        {
            //try get string from app settings or cloudd service config
            var partitionKeyPrefixValue = CloudConfigurationManager.GetSetting(PartitionKeyPrefixKey);
            if (!string.IsNullOrEmpty(partitionKeyPrefixValue)) return partitionKeyPrefixValue;

            //try get connection string from ConfigurationManager.AppSettings
            var appSetting = ConfigurationManager.AppSettings[PartitionKeyPrefixKey];
            return appSetting;
        }
    }
}
