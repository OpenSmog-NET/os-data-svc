using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace OS.Core
{
    public abstract class Host
    {
        protected readonly HostSetup Setup = new HostSetup();
        protected readonly List<string> StartupMethods;

        protected ILogger Logger;
        private Action<IHostSetup> builderSetup;

        protected Host(string serviceCodeName)
        {
            ServiceCodeName = serviceCodeName;

            StartupMethods = new List<string>()
            {
                "ConfigureServices",
                "Configure",
            };
        }

        public IServiceProvider ServiceProvider { get; private set; }
        protected string ServiceCodeName { get; }
        protected object Startup { get; private set; }

        public void Configure(Action<IHostSetup> builderSetup)
        {
            this.builderSetup = builderSetup ?? throw new ArgumentNullException(nameof(builderSetup));
        }

        public void Run()
        {
            Initialize();
            RunHost();
        }

        protected void Initialize()
        {
            builderSetup(Setup);
            RunStartup();
            StartLogging(HostingEnvironment.Name);
        }

        protected abstract void RunHost();

        private object CreateStartupInstance()
        {
            var constructors = Setup.StartupClass
                .GetConstructors(BindingFlags.Public | BindingFlags.Instance);

            if (constructors == null || constructors.Length != 1)
            {
                throw new InvalidOperationException("Startup class needs to have exactly one public constructor.");
            }

            var constructor = constructors[0];
            var parameterValues = GenerateParameters(constructor.GetParameters());

            return constructor.Invoke(parameterValues.ToArray());
        }

        private List<object> GenerateParameters(ParameterInfo[] methodParameters)
        {
            var parameterValues = new List<object>();

            foreach (var methodParameter in methodParameters)
            {
                if (!Setup.SupportedParameters.TryGetValue(methodParameter.ParameterType, out object value))
                {
                    throw new InvalidOperationException(
                        $"Unsupported parameter type '{methodParameter.ParameterType.Name}'.");
                }

                parameterValues.Add(value);
            }

            return parameterValues;
        }

        private void LogEnvironmentVariables()
        {
            foreach (var enumValue in Enum.GetValues(typeof(EnvironmentVariableTarget)))
            {
                var environmentVariables = Environment.GetEnvironmentVariables((EnvironmentVariableTarget)enumValue);

                var formattedEnvironmentVariables = new Dictionary<string, string>();
                foreach (var key in environmentVariables.Keys)
                {
                    formattedEnvironmentVariables[(string)key] = (string)(environmentVariables[key]);
                }

                Logger.LogDebug(
                    "Environment variables targeting '{EnvironmentVariableTarget}': {EnvironmentVariables}",
                    enumValue,
                    formattedEnvironmentVariables);
            }
        }

        private void RunStartup()
        {
            if (Setup.StartupClass == null)
            {
                throw new InvalidOperationException(
                    "Startup class has not been specified. Please use UseStartup() method.");
            }

            Startup = CreateStartupInstance();

            this.StartupMethods.ForEach(x =>
            {
                var method = Setup.StartupClass.GetMethod(x, BindingFlags.Public | BindingFlags.Instance);
                if (method == null) return;

                var parameters = GenerateParameters(method.GetParameters()).ToArray();
                method.Invoke(Startup, parameters);
            });

            ServiceProvider = Setup.Services.BuildServiceProvider(true);
        }

        private void StartLogging(string env)
        {
            Logger = Setup.LoggerFactory.CreateLogger(GetType());

            Logger.LogInformation("Launching application '{ServiceCodeName}' in '{Environment}' mode.", this.ServiceCodeName, env);
            Logger.LogInformation("Assembly version: {Assembly}", Assembly.GetEntryAssembly().FullName);

            LogEnvironmentVariables();
        }
    }
}