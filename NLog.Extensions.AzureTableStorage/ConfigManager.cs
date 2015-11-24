using Microsoft.Azure;
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
            if (string.IsNullOrEmpty(connectionStringValue))
            {
                //try get connection string from configured AtsOptions
                connectionStringValue = AtsOptions.Instance.StorageAccountConnectionString;
            }

            return connectionStringValue;
        }

        public CloudStorageAccount GetStorageAccount()
        {
            var connectionString = GetStorageAccountConnectionString();
            return CloudStorageAccount.Parse(connectionString);
        }

        public string GetSettingByKey(string key)
        {
            //try get string from app settings or cloud service config
           return CloudConfigurationManager.GetSetting(key);
        }
    }
}
