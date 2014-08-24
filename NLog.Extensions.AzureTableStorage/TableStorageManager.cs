using Microsoft.WindowsAzure;
using ConfigurationManager = System.Configuration.ConfigurationManager;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace NLog.Extensions.AzureTableStorage
{
    public class TableStorageManager
    {
        private readonly CloudTable _cloudTable;
        private readonly string _connectionStringKey;

        public TableStorageManager(string connectionStringKey, string tableName)
        {
            _connectionStringKey = connectionStringKey;
            var storageAccount = GetStorageAccount();
            // Create the table client.
            var tableClient = storageAccount.CreateCloudTableClient();
            //create charts table if not exists.
            _cloudTable = tableClient.GetTableReference(tableName);
            _cloudTable.CreateIfNotExists();
        }

        private string GetStorageAccountConnectionString()
        {
            //try get connection string from app settings or could service config
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

        private CloudStorageAccount GetStorageAccount()
        {
            var connectionString = GetStorageAccountConnectionString();
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            return storageAccount;
        }

        public void Add(LogEntity entity)
        {
            var insertOperation = TableOperation.Insert(entity);
            _cloudTable.Execute(insertOperation);
        }
    }
}
