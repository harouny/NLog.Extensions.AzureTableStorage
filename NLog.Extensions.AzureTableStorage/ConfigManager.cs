using System.Configuration;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;

namespace NLog.Extensions.AzureTableStorage
{
    public class ConfigManager
    {
        private readonly string _connectionStringKey;

        public ConfigManager(string connectionStringKey)
        {
            _connectionStringKey = connectionStringKey;
        }

        private string GetStorageAccountConnectionString()
        {
            //try get connection string from app settings or cloud service config
            var connectionStringValue = CloudConfigurationManager.GetSetting(_connectionStringKey);
            if (!string.IsNullOrEmpty(connectionStringValue)) return connectionStringValue;

            //try get connection string from ConfigurationManager.ConnectionStrings
            var connectionString = ConfigurationManager.ConnectionStrings[_connectionStringKey];
            if (connectionString != null)
            {
                connectionStringValue = connectionString.ConnectionString;
            }
            return connectionStringValue;
        }

        public CloudStorageAccount GetStorageAccount()
        {
            var connectionString = GetStorageAccountConnectionString();
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            return storageAccount;
        }

        public string GetSettingByKey(string partitionKeyPrefixKey)
        {
            //try get string from app settings or cloudd service config
            var partitionKeyPrefixValue = CloudConfigurationManager.GetSetting(partitionKeyPrefixKey);
            if (!string.IsNullOrEmpty(partitionKeyPrefixValue))
                return partitionKeyPrefixValue;

            //try get connection string from ConfigurationManager.AppSettings
            var appSetting = ConfigurationManager.AppSettings[partitionKeyPrefixKey];
            return appSetting;
        }
    }
}
