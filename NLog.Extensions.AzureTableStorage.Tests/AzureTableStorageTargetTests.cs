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
        private readonly Logger _logger;
        private readonly CloudTable _cloudTable;

        public AzureTableStorageTargetTests()
        {
            _logger = LogManager.GetLogger(GetType().ToString());
            var storageAccount = GetStorageAccount();
            // Create the table client.
            var tableClient = storageAccount.CreateCloudTableClient();
            //create charts table if not exists.
            _cloudTable = tableClient.GetTableReference("AzureTableStorageTargetTestsLogs");
            _cloudTable.CreateIfNotExists();
        }
        

        [Fact]
        public void CanLogInformation()
        {
            Assert.True(GetLogEntities().Count == 0);
            _logger.Log(LogLevel.Info, "information");
            var entities = GetLogEntities();
            var entity = entities.Single();
            Assert.True(entities.Count == 1);
            Assert.Equal("information", entity.Message);
            Assert.Equal("Info", entity.Level);
            Assert.Equal(GetType().ToString(), entity.LoggerName);
        }


        [Fact]
        public void CanLogExeptions()
        {
            Assert.True(GetLogEntities().Count == 0);
            _logger.LogException(LogLevel.Error, "execption messege", new NullReferenceException() );
            var entities = GetLogEntities();
            var entity = entities.Single();
            Assert.True(entities.Count == 1);
            Assert.Equal("execption messege", entity.Message);
            Assert.Equal("Error", entity.Level);
            Assert.Equal(GetType().ToString(), entity.LoggerName);
            Assert.NotNull(entity.Exception);
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
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, this.GetType().ToString()));
            var entities = _cloudTable.ExecuteQuery(query);
            return entities.ToList();
        }


        public void Dispose()
        {
            _cloudTable.DeleteIfExists();
        }
    }
}
