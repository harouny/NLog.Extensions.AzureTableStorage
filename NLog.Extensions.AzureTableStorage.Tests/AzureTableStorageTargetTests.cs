using Microsoft.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace NLog.Extensions.AzureTableStorage.Tests
{
    public static class Extensions
    {
        public static async Task<IList<T>> ExecuteQueryAsync<T>(this CloudTable table, TableQuery<T> query, CancellationToken ct = default(CancellationToken), Action<IList<T>> onProgress = null) where T : ITableEntity, new()
        {
            var items = new List<T>();
            TableContinuationToken token = null;

            do
            {
                TableQuerySegment<T> seg = await table.ExecuteQuerySegmentedAsync<T>(query, token);
                token = seg.ContinuationToken;
                items.AddRange(seg);
                if (onProgress != null) onProgress(items);

            } while (token != null && !ct.IsCancellationRequested);

            return items;
        }
    }

    public class AzureTableStorageTargetTests : IDisposable
    {
        private readonly Logger _logger;
        private readonly CloudTable _cloudTable;
        private const string TargetTableName = "TempAzureTableStorageTargetTestsLogs"; //must match table name in AzureTableStorage target in NLog.config

        public void SetupAction(AtsOptions options)
        {
            var cb = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            IConfigurationRoot config = cb.Build();
            options.StorageAccountConnectionString = config["AppSettings:StorageAccountConnectionString"];
        }

        public AzureTableStorageTargetTests()
        {
            try
            {
                AtsCoreServiceCollectionExtensions.ConfigureAts(null, SetupAction);
                LogManager.Configuration = new NLog.Config.XmlLoggingConfiguration("NLog.config");
                _logger = LogManager.GetCurrentClassLogger();
                var storageAccount = GetStorageAccount();
                // Create the table client.
                var tableClient = storageAccount.CreateCloudTableClient();
                //create charts table if not exists.
                _cloudTable = tableClient.GetTableReference(TargetTableName);
                _cloudTable.CreateIfNotExistsAsync().Wait();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to initialize tests, make sure Azure Storage Emulator is running.", ex);
            }
        }

        [Fact]
        public async Task CanLogInformation()
        {
            Assert.True((await GetLogEntities()).Count == 0);
            _logger.Log(LogLevel.Info, "information");
            var entities = await GetLogEntities();
            var entity = entities.Single();
            Assert.True(entities.Count == 1);
            Assert.Equal("information", entity.Message);
            Assert.Equal("Info", entity.Level);
            Assert.Equal(GetType().ToString(), entity.LoggerName);
        }

        [Fact]
        public async Task CanLogExceptions()
        {
            Assert.True((await GetLogEntities()).Count == 0);
            _logger.Log(LogLevel.Error, new NullReferenceException(), "exception message");
            var entities = await GetLogEntities();
            var entity = entities.Single();
            Assert.True(entities.Count == 1);
            Assert.Equal("exception message", entity.Message);
            Assert.Equal("Error", entity.Level);
            Assert.Equal(GetType().ToString(), entity.LoggerName);
            Assert.NotNull(entity.Exception);
        }

        [Fact]
        public async Task IncludeExceptionFormattedMessengerInLoggedRow()
        {
            _logger.Debug("exception message {0} and {1}.", 2010, 2014);
            var entity = (await GetLogEntities()).Single();
            Assert.Equal("exception message 2010 and 2014.", entity.Message);
        }

        [Fact]
        public async Task IncludeExceptionDataInLoggedRow()
        {
            var exception = new NullReferenceException();
            var errorId = Guid.NewGuid();
            exception.Data["id"] = errorId;
            exception.Data["name"] = "ahmed";
            _logger.Log(LogLevel.Error, exception, "execption message");
            var entities = await GetLogEntities();
            var entity = entities.Single();
            Assert.True(entity.ExceptionData.Contains(errorId.ToString()));
            Assert.True(entity.ExceptionData.Contains("name=ahmed"));
        }

        [Fact]
        public async Task IncludeExceptionDetailsInLoggedRow()
        {
            var exception = new NullReferenceException();
            _logger.Log(LogLevel.Error, exception, "execption message");
            var entity = (await GetLogEntities()).Single();
            Assert.NotNull(entity.Exception);
            Assert.Equal(exception.ToString().ExceptBlanks(), entity.Exception.ExceptBlanks());
        }

        [Fact]
        public async Task IncludeInnerExceptionDetailsInLoggedRow()
        {
            var exception = new NullReferenceException("exception message", new DivideByZeroException());
            _logger.Log(LogLevel.Error, exception, "execption message");
            var entity = (await GetLogEntities()).Single();
            Assert.NotNull(entity.Exception);
            Assert.Equal(exception.ToString().ExceptBlanks(), entity.Exception.ExceptBlanks());
            Assert.NotNull(entity.InnerException);
            Assert.Equal(exception.InnerException.ToString().ExceptBlanks(), entity.InnerException.ExceptBlanks());
        }

        [Fact]
        public async Task IncludePartitionKeyPrefix()
        {
            var exception = new NullReferenceException();
            _logger.Log(LogLevel.Error, exception, "execption message");
            var entity = (await GetLogEntities()).Single();
            Assert.True(entity.PartitionKey.Contains("customPrefix"));
        }

        [Fact]
        public async Task IncludeMachineName()
        {
            var exception = new NullReferenceException();
            _logger.Log(LogLevel.Error, exception, "execption message");
            var entity = (await GetLogEntities()).Single();
            Assert.Equal(entity.MachineName, Environment.MachineName);
        }

        [Fact]
        public async Task IncludeGuidAndTimeComponentInRowKey()
        {
            var exception = new NullReferenceException();
            _logger.Log(LogLevel.Error, (Exception)exception, "execption message");
            var entity = (await GetLogEntities()).Single();
            const string splitter = "__";
            Assert.True(entity.RowKey.Contains(splitter));
            var splitterArray = "__".ToCharArray();
            var segments = entity.RowKey.Split(splitterArray, StringSplitOptions.RemoveEmptyEntries);
            Guid globalId;
            long timeComponent;
            Assert.True(Guid.TryParse(segments[1], out globalId));
            Assert.True(long.TryParse(segments[0], out timeComponent));
        }



        private string GetStorageAccountConnectionString()
        {
            //try get connection string from app settings or cloud service config
            var connectionStringValue = CloudConfigurationManager.GetSetting("StorageAccountConnectionString");
            if (string.IsNullOrEmpty(connectionStringValue))
            {
                //try get connection string from configured AtsOptions
                connectionStringValue = AtsOptions.Instance.StorageAccountConnectionString;
            }

            return connectionStringValue;
        }

        private CloudStorageAccount GetStorageAccount()
        {
            var connectionString = GetStorageAccountConnectionString();
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            return storageAccount;
        }

        private async Task<List<LogEntity>> GetLogEntities()
        {
            // Construct the query operation for all customer entities where PartitionKey="Smith".
            var query = new TableQuery<LogEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "customPrefix." + GetType()));
            //var entities = _cloudTable.ExecuteQuery(query);
            IList<LogEntity> entities = await _cloudTable.ExecuteQueryAsync(query);

            return entities.ToList();
        }

        public void Dispose()
        {
            //_cloudTable.DeleteIfExists();
            _cloudTable.DeleteIfExistsAsync().Wait();
        }
    }
}
