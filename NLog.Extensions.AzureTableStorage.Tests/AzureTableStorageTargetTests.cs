using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Xunit;

namespace NLog.Extensions.AzureTableStorage.Tests
{
    public class AzureTableStorageTargetTests : IDisposable
    {
        protected Sut Sut;
        private readonly CloudTable _cloudTable;

        public AzureTableStorageTargetTests()
        {
            Sut = new Sut(LogManager.GetLogger(typeof(Sut).ToString()));
            var storageAccount = GetStorageAccount();
            // Create the table client.
            var tableClient = storageAccount.CreateCloudTableClient();
            //create charts table if not exists.
            _cloudTable = tableClient.GetTableReference("AzureTableStorageTargetTestsLogs");
            _cloudTable.CreateIfNotExists();
        }
        

        [Fact]
        public void Test()
        {
            Assert.True(GetLogEntities().Count == 0);
            Sut.Operation();
            var entities = GetLogEntities();
            Assert.True(entities.Count == 1);
            Assert.Equal("information", entities.Single().Message);
            Assert.Equal("Info", entities.Single().Level);

        }


        private string GetStorageAccountConnectionString()
        {
            return CloudConfigurationManager.GetSetting("StorageAccountConnectionString");
        }
        private CloudStorageAccount GetStorageAccount()
        {
            var connectionString = GetStorageAccountConnectionString();
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            return storageAccount;
        }

        private List<LogEntity> GetLogEntities()
        {
            // Construct the query operation for all customer entities where PartitionKey="Smith".
            var query = new TableQuery<LogEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, typeof(Sut).ToString()));
            var entities = _cloudTable.ExecuteQuery(query);
            return entities.ToList();
        }


        public void Dispose()
        {
            _cloudTable.DeleteIfExists();
        }
    }
}
