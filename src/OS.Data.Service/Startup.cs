using Marten;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace OS.Data.Service
{
    public class Startup
    {
        public void ConfigureServices(IConfiguration configuration, IServiceCollection services, ILoggerFactory loggerFactory)
        {
            services.AddLogging();

            services.AddTransient<IDocumentStore>(provider => DocumentStore.For(configuration.GetConnectionString("Database")));
            services.AddSingleton<TelemetryClient>(provider => new TelemetryClient(new TelemetryConfiguration(configuration.GetConnectionString("ApplicationInsights"))));
            services.AddSingleton<TelemetryService>();
        }
    }
}