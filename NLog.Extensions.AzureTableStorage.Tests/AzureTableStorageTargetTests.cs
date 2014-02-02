using System;
using System.Collections.Generic;
using System.Linq;
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


        [Fact]
        public void IncludeExeptionDetailsInLoggedRow()
        {
            _logger.LogException(LogLevel.Error, "execption messege", new NullReferenceException());
            var entity = GetLogEntities().Single();
            Assert.NotNull(entity.Exception);
            Assert.True(entity.Exception.Contains(typeof(NullReferenceException).ToString()));
        }



        [Fact]
        public void IncludeArgumentsInLoggedRow()
        {
            _logger.Log(LogLevel.Debug, "debug messege", 5);
            var entity = GetLogEntities().Single();
            Assert.NotNull(entity.Parameters);
            Assert.Equal("5", entity.Parameters);
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
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, GetType().ToString()));
            var entities = _cloudTable.ExecuteQuery(query);
            return entities.ToList();
        }
        public void Dispose()
        {
            _cloudTable.DeleteIfExists();
        }
    }
}
