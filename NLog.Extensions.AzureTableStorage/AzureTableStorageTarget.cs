using System.ComponentModel.DataAnnotations;
using NLog.Targets;

namespace NLog.Extensions.AzureTableStorage
{
    [Target("AzureTableStorage")]
    public class AzureTableStorageTarget : TargetWithLayout
    {
        private TableStorageManager _tableStorageManager;
        
        [Required] 
        public string ConnectionStringKey { get; set; }
        
        [Required]
        public string TableName { get; set; }


        protected override void InitializeTarget()
        {
            base.InitializeTarget();
            _tableStorageManager = new TableStorageManager(ConnectionStringKey, TableName);
        }

        protected override void Write(LogEventInfo logEvent)
        {
            var layoutMessage = Layout.Render(logEvent);
            _tableStorageManager.Add(new LogEntity(logEvent, layoutMessage));
        }

    }
}
