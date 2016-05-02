using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLog.Extensions.AzureTableStorage
{
    public interface IEntityFactory
    {
        TableEntity Build(LogEventInfo logEvent, string layoutMessage);
    }
}
