using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace OS.Core
{
    public class HostSetup : IHostSetup
    {
        private const string StartupClassType = "StartupClass";

        public HostSetup()
        {
            SupportedParameters[typeof(IServiceCollection)] = Services;
        }

        public IConfiguration Configuration { get; private set; }
        public ILoggerFactory LoggerFactory { get; private set; }
        public IServiceCollection Services { get; } = new ServiceCollection();
        public Type StartupClass => RequiredTypes[StartupClassType];
        public IDictionary<Type, object> SupportedParameters { get; } = new Dictionary<Type, object>();

        public IDictionary<string, Type> RequiredTypes { get; } = new Dictionary<string, Type>();

        public IHostSetup ConfigureServices(Action<IServiceCollection> configureServices)
        {
            configureServices(Services);

            return this;
        }

        public IHostSetup UseConfiguration(IConfiguration configuration)
        {
            Configuration = configuration;

            SupportedParameters[typeof(IConfiguration)] = Configuration;

            return this;
        }

        public IHostSetup UseLoggerFactory(ILoggerFactory loggerFactory)
        {
            LoggerFactory = loggerFactory;

            SupportedParameters[typeof(ILoggerFactory)] = LoggerFactory;

            return this;
        }

        public IHostSetup UseStartup<T>() where T : class
        {
            RequiredTypes[StartupClassType] = typeof(T);

            return this;
        }

        public IHostSetup AddRequiredType<T>(string key)
            where T : class
        {
            RequiredTypes[key] = typeof(T);

            return this;
        }
    }
}