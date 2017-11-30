using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System;

namespace OS.Core.Logging
{
    public static class LoggingSubsystem
    {
        private const string DevelopmentEnvironmentPrefix = "Development";

        public static ILoggerFactory Configure(IConfiguration configuration, string applicationName)
        {
            var loggerFactory = new LoggerFactory();

            var environmentName = HostingEnvironment.Name;
            var serilogConfig = configuration.GetSection("Serilog");

            // Note that we remove some of the logs coming from ASP.NET Core (we only accept Warning+).
            var loggerConfiguration = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .MinimumLevel.Is(MapLogLevel(serilogConfig["Minimum-Level"]))
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning);

            if (!string.IsNullOrEmpty(environmentName)
                && environmentName.StartsWith(DevelopmentEnvironmentPrefix, StringComparison.OrdinalIgnoreCase))
            {
                loggerConfiguration.WriteTo.Console(LogEventLevel.Verbose);
            }

            var serilogLogger = loggerConfiguration.CreateLogger();

            // Note that we explicitly set "dispose" parameter to true: We want our Serilog logger to get disposed
            // when the ILoggerFactory gets disposed. This is important, as disposing the Serilog logger will also
            // flush any remaining logs.
            return loggerFactory.AddSerilog(serilogLogger, dispose: true);
        }

        public static void CloseAndFlush()
        {
            Log.CloseAndFlush();
        }

        private static LogEventLevel MapLogLevel(string logLevel)
        {
            return (LogEventLevel)Enum.Parse(typeof(LogEventLevel), logLevel);
        }
    }
}