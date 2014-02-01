using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLog.Extensions.AzureTableStorage.Tests
{
    public class Sut
    {
        private readonly Logger _logger;
        
        public Sut(Logger logger)
        {
            _logger = logger;
        }

        public void Operation()
        {
            _logger.Log(LogLevel.Info, "information");
        }
    }
}
