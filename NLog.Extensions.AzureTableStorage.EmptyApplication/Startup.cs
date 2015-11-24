
using System.IO;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.Dnx.Runtime;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;

namespace NLog.Extensions.AzureTableStorage.EmptyApplication
{
    public class Startup
    {
        private Microsoft.Framework.Logging.ILogger _log;
        private string logConfigurationFile = string.Empty;

        public Startup(IHostingEnvironment env, IApplicationEnvironment appEnv)
        {
            // Setup configuration sources.

            var builder = new ConfigurationBuilder()
                .SetBasePath(appEnv.ApplicationBasePath)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
            logConfigurationFile = Path.Combine(appEnv.ApplicationBasePath, "NLog.config");

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Configure AtsOption for NLog.Extensions.AzureTableStorage
            services.ConfigureAts(options => {
                options.StorageAccountConnectionString = Configuration["AppSettings:StorageAccountConnectionString"];
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            // add NLog for logging
            loggerFactory.AddNLog(new LogFactory(new NLog.Config.XmlLoggingConfiguration(logConfigurationFile)));
            _log = loggerFactory.CreateLogger<Startup>();

            // generate some log output
            _log.LogInformation("Happy table logging!");
            _log.LogInformation(string.Format("Log configuration: {0}", logConfigurationFile));

            app.UseIISPlatformHandler();

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }
    }
}
