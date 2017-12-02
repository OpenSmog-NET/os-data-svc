using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OS.Core;
using OS.Core.Logging;
using OS.Core.WebJobs;
using System;
using System.IO;

namespace OS.Data.Service
{
    internal class Program
    {
        public const string ServiceCodeName = "os-data-svc";

        private static void Main(string[] args)
        {
            var config = ConfigProvider.Create(Directory.GetCurrentDirectory()) as IConfiguration;
            using (var loggerFactory = LoggingSubsystem.Configure(config, ServiceCodeName))
            {
                var log = loggerFactory.CreateLogger(typeof(Program));
                var section = config.GetSection("ConnectionStrings");
                foreach (var kvp in section.AsEnumerable())
                {
                    log.LogInformation("{@connectionString}", kvp);
                    log.LogInformation($"{kvp.Key} : {kvp.Value}");
                }

                try
                {
                    var host = new WebJobServiceHost(ServiceCodeName);
                    host.Configure(builder => builder
                        .UseStartup<Startup>()
                        .UseConfiguration(config)
                        .UseLoggerFactory(loggerFactory)
                        .UseJobClass<DataService>());

                    host.Run();
                }
                catch (Exception ex)
                {
                    var logger = loggerFactory.CreateLogger(typeof(Program));
                    logger.LogCritical(1, ex, "Application exited with error.");
                    throw;
                }
                finally
                {
                    LoggingSubsystem.CloseAndFlush();
                }
            }
        }
    }
}