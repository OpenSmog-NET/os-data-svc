using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;

namespace OS.Core.WebJobs
{
    public static class HostSetupExtensions
    {
        public static IHostSetup UseJobHostConfiguration(this IHostSetup setup, JobHostConfiguration jobHostConfiguration)
        {
            setup.ConfigureServices(services => services.AddSingleton(jobHostConfiguration));

            return setup;
        }

        public static IHostSetup UseJobClass<T>(this IHostSetup setup) where T : class
        {
            setup.ConfigureServices(services => services.AddTransient<T>());
            setup.AddRequiredType<T>(WebJobServiceHost.WebJobClassType);

            return setup;
        }
    }
}