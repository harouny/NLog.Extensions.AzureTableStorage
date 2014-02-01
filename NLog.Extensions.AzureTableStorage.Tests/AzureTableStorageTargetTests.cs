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
            var storageAccount = GetStorageAccount();
            Sut = new Sut(LogManager.GetLogger(typeof(Sut).ToString()));
            // Create the table client.
            var tableClient = storageAccount.CreateCloudTableClient();
            //create charts table if not exists.
            _cloudTable = tableClient.GetTableReference("AzureTableStorageTargetTestsLogs");
            _cloudTable.CreateIfNotExists();
        }
        

        [Fact]
        public void Test()
        {
            Sut.Operation();
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

        public void Dispose()
        {
            _cloudTable.DeleteIfExists();
        }
    }
}
