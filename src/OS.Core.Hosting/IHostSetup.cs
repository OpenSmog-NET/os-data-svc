using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace OS.Core
{
    public interface IHostSetup
    {
        IHostSetup ConfigureServices(Action<IServiceCollection> configureServices);

        IHostSetup UseConfiguration(IConfiguration configuration);

        IHostSetup UseLoggerFactory(ILoggerFactory loggerFactory);

        IHostSetup UseStartup<T>() where T : class;

        IHostSetup AddRequiredType<T>(string type) where T : class;
    }
}