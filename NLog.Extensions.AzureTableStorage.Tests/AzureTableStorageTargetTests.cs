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
        private const int TimeOutInMilliseconds = 8000; //8 seconds or fail
        private const string TargetTableName = "TempAzureTableStorageTargetTestsLogs"; //must match table name in AzureTableStorage target in NLog.config

        public AzureTableStorageTargetTests()
        {
            try
            {
                _logger = LogManager.GetLogger(GetType().ToString());
                var storageAccount = GetStorageAccount();
                // Create the table client.
                var tableClient = storageAccount.CreateCloudTableClient();
                //create charts table if not exists.
                _cloudTable = tableClient.GetTableReference(TargetTableName);
                _cloudTable.CreateIfNotExists();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to initialize tests, make sure Azure Storage Emulator is running.", ex);
            }
            
        }


        [Fact(Timeout = TimeOutInMilliseconds)]
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


        [Fact(Timeout = TimeOutInMilliseconds)]
        public void CanLogExceptions()
        {
            Assert.True(GetLogEntities().Count == 0);
            _logger.Log(LogLevel.Error, "exception message", (Exception)new NullReferenceException());
            var entities = GetLogEntities();
            var entity = entities.Single();
            Assert.True(entities.Count == 1);
            Assert.Equal("exception message", entity.Message);
            Assert.Equal("Error", entity.Level);
            Assert.Equal(GetType().ToString(), entity.LoggerName);
            Assert.NotNull(entity.Exception);
        }


        [Fact(Timeout = TimeOutInMilliseconds)]
        public void IncludeExceptionFormattedMessengerInLoggedRow()
        {
            _logger.Debug("exception message {0} and {1}.", 2010, 2014);
            var entity = GetLogEntities().Single();
            Assert.Equal("exception message 2010 and 2014.", entity.Message);
        }


        [Fact(Timeout = TimeOutInMilliseconds)]
        public void IncludeExceptionDataInLoggedRow()
        {
            var exception = new NullReferenceException();
            var errorId = Guid.NewGuid();
            exception.Data["id"] = errorId;
            exception.Data["name"] = "ahmed";
            _logger.Log(LogLevel.Error, "execption messege", (Exception)exception);
            var entities = GetLogEntities();
            var entity = entities.Single();
            Assert.True(entity.ExceptionData.Contains(errorId.ToString()));
            Assert.True(entity.ExceptionData.Contains("name=ahmed"));
        }
        
        
        
        [Fact(Timeout = TimeOutInMilliseconds)]
        public void IncludeExceptionDetailsInLoggedRow()
        {
            var exception = new NullReferenceException();
            _logger.Log(LogLevel.Error, "execption messege", (Exception)exception);
            var entity = GetLogEntities().Single();
            Assert.NotNull(entity.Exception);
            Assert.Equal(exception.ToString().ExceptBlanks(), entity.Exception.ExceptBlanks());
        }



        [Fact(Timeout = TimeOutInMilliseconds)]
        public void IncludeInnerExceptionDetailsInLoggedRow()
        {
            var exception = new NullReferenceException("exception message", new DivideByZeroException());
            _logger.Log(LogLevel.Error, "execption messege", (Exception)exception);
            var entity = GetLogEntities().Single();
            Assert.NotNull(entity.Exception);
            Assert.Equal(exception.ToString().ExceptBlanks(), entity.Exception.ExceptBlanks());
            Assert.NotNull(entity.InnerException);
            Assert.Equal(exception.InnerException.ToString().ExceptBlanks(), entity.InnerException.ExceptBlanks());
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
