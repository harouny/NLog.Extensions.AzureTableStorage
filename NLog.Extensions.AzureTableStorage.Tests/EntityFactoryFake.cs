using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace NLog.Extensions.AzureTableStorage.Tests
{
    public class EntityFactoryFake : IEntityFactory
    {
        public TableEntity Build(LogEventInfo logEvent, string layoutMessage)
        {
            return new EntityFake(logEvent, layoutMessage);
        }
    }

    public class EntityFake : TableEntity
    {
        public EntityFake(LogEventInfo logEvent, string layoutMessage)
        {
            this.RowKey = Guid.NewGuid().ToString();
            this.PartitionKey = "MyCustomColumn";
            this.MyCustomColumn = "My Custom Column created";
        }

        public EntityFake()
        {

        }

        public string MyCustomColumn { get; set; }
    }
}
