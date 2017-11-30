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
            var config = ConfigProvider.Create(Directory.GetCurrentDirectory());
            using (var loggerFactory = LoggingSubsystem.Configure(config, ServiceCodeName))
            {
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