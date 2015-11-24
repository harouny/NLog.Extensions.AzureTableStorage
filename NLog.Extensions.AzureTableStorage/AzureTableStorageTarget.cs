using System;
using System.ComponentModel.DataAnnotations;
using NLog.Targets;

namespace NLog.Extensions.AzureTableStorage
{
    [Target("AzureTableStorage")]
    public class AzureTableStorageTarget : TargetWithLayout
    {
        private ConfigManager _configManager;
        private TableStorageManager _tableStorageManager;

        [Required]
        public string ConnectionStringKey { get; set; }

        [Required]
        public string TableName { get; set; }

        public string PartitionKeyPrefix { get; set; }
        public string PartitionKeyPrefixKey { get; set; }
        public string PartitionKeyPrefixDateFormat { get; set; }

        protected override void InitializeTarget()
        {
            base.InitializeTarget();
            ValidateParameters();
            _configManager = new ConfigManager(ConnectionStringKey);
            _tableStorageManager = new TableStorageManager(_configManager, TableName);

            // use PartitionKeyPrefixKey if present
            if (!string.IsNullOrWhiteSpace(PartitionKeyPrefixKey))
            {
                PartitionKeyPrefix = _configManager.GetSettingByKey(PartitionKeyPrefixKey);
            }
            // else use PartitionKeyPrefixDateFormat if available
            else if (!string.IsNullOrWhiteSpace(PartitionKeyPrefixDateFormat))
            {
                PartitionKeyPrefix = DateTime.UtcNow.ToString(PartitionKeyPrefixDateFormat);
            }
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

        protected override void Write(LogEventInfo logEvent)
        {
            if (_tableStorageManager != null)
            {
                var layoutMessage = Layout.Render(logEvent);
                _tableStorageManager.Add(new LogEntity(PartitionKeyPrefix, logEvent, layoutMessage));
            }
        }
    }
}
