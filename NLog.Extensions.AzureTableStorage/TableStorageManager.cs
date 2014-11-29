using Microsoft.WindowsAzure.Storage.Table;

namespace NLog.Extensions.AzureTableStorage
{
    public class TableStorageManager
    {
        private readonly CloudTable _cloudTable;

        public TableStorageManager(ConfigManager configManager, string tableName)
        {
            var storageAccount = configManager.GetStorageAccount();
            // Create the table client.
            var tableClient = storageAccount.CreateCloudTableClient();
            //create charts table if not exists.
            _cloudTable = tableClient.GetTableReference(tableName);
            _cloudTable.CreateIfNotExists();
        }

        public void Add(LogEntity entity)
        {
            var insertOperation = TableOperation.Insert(entity);
            _cloudTable.Execute(insertOperation);
        }
    }
}
