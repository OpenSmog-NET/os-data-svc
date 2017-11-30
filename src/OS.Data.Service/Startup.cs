using Marten;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace OS.Data.Service
{
    public class Startup
    {
        //public static IConfigurationRoot ReadConfiguration()
        //{
        //    var builder = new ConfigurationBuilder()
        //        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
        //        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        //        //.AddJsonFile($"appsettings.{WebJobEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true)
        //        //.AddWhen(() => !WebJobEnvironment.IsDevelopment, (cfg) => cfg.AddKeyVault())
        //        .AddEnvironmentVariables();

        //    return builder.Build();
        //}

        //public static IServiceProvider ConfigureServices(IConfigurationRoot configuration)
        //{
        //    var services = new ServiceCollection()
        //        .AddSingleton(configuration)
        //        .AddSingleton<DataService>();

        //    return services.BuildServiceProvider();
        //}
        public void ConfigureServices(IConfiguration configuration, IServiceCollection services, ILoggerFactory loggerFactory)
        {
            services.AddLogging();

            services.AddTransient<IDocumentStore>(provider => DocumentStore.For(configuration.GetConnectionString("Database")));
        }
    }
}