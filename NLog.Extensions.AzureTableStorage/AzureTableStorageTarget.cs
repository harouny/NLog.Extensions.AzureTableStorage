using System.ComponentModel.DataAnnotations;
using NLog;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLog.Extensions.AzureTableStorage
{
    [Target("AzureTableStorage")]
    public class AzureTableStorageTarget : TargetWithLayout 
    {
        [Required] 
        public string ConnectionString { get; set; }

        protected override void Write(LogEventInfo logEvent)
        {
           //TODO: do the writing to table storage
           
        }
    }
}
