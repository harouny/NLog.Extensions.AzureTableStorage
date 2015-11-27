using System;

namespace NLog.Extensions.AzureTableStorage
{
    public class AtsOptions
    {
        private static AtsOptions _instance;

        public string StorageAccountConnectionString { get; set; }

        private AtsOptions() { }

        public static AtsOptions Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AtsOptions();
                }
                return _instance;
            }
        }
    }
}
