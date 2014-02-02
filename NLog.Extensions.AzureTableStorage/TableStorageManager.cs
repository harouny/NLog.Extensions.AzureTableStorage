using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace NLog.Extensions.AzureTableStorage
{
    public class TableStorageManager
    {
        private readonly CloudTable _cloudTable;
        private readonly string _connectionStringKey;
        private readonly string _tableName;

        public TableStorageManager(string connectionStringKey, string tableName)
        {
            _connectionStringKey = connectionStringKey;
            _tableName = tableName;
            var storageAccount = GetStorageAccount();
            // Create the table client.
            var tableClient = storageAccount.CreateCloudTableClient();
            //create charts table if not exists.
            _cloudTable = tableClient.GetTableReference(_tableName);
            _cloudTable.CreateIfNotExists();
        }

        private string GetStorageAccountConnectionString()
        {
            return CloudConfigurationManager.GetSetting(_connectionStringKey);
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
