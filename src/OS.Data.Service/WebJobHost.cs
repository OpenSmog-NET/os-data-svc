namespace OS.Data.Service
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    namespace OS.Core.Functions
    {
        public abstract class FunctionHost
        {
            protected readonly IDictionary<Type, object> SupportedParameters;
            protected readonly List<string> StartupMethods;

            protected readonly object Startup;
            protected Type StartupType;

            protected FunctionHost(
                IConfiguration configuration,
                ILoggerFactory loggerFactory)
            {
                Configuration = configuration ?? throw new ArgumentException(nameof(configuration));

                LoggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));

                //ServiceCodeName = serviceCodeName;

                Logger = LoggerFactory.CreateLogger(GetType());

                SupportedParameters = new Dictionary<Type, object>()
            {
                { typeof(IConfiguration), this.Configuration },
                { typeof(ILoggerFactory), this.LoggerFactory },
                { typeof(IServiceCollection), this.ServiceCollection }
            };

                StartupMethods = new List<string>()
            {
                "ConfigureServices",
                "Configure",
            };

                Startup = CreateStartupInstance();
            }

            protected IConfiguration Configuration { get; }
            protected ILogger Logger { get; }
            protected ILoggerFactory LoggerFactory { get; }
            protected IServiceCollection ServiceCollection { get; } = new ServiceCollection();

            protected string ServiceCodeName => Configuration["Serilog:Properties:Application"];

            protected void Initialize()
            {
                try
                {
                    RunStartup();
                    StartLogging("Dev");
                }
                catch (Exception e)
                {
                    Logger.LogError(1, e, "Error hosting application: {ErrorMessage}", e.GetBaseException().Message);
                    throw;
                }
            }

            private object CreateStartupInstance()
            {
                StartupType = AppDomain.CurrentDomain.GetAssemblies()
                    .Where(asm => asm.FullName.StartsWith("OS"))
                    .SelectMany(asm => asm.GetTypes().Where(t => t.Name == "Startup"))
                    .SingleOrDefault();

                var constructors = StartupType
                    .GetConstructors(BindingFlags.Public | BindingFlags.Instance);

                if (constructors.Length != 1)
                {
                    throw new InvalidOperationException("Startup class needs to have exactly one public constructor.");
                }

                var constructor = constructors[0];
                var parameterValues = GenerateParameters(constructor.GetParameters());

                return constructor.Invoke(parameterValues.ToArray());
            }

            private void RunStartup()
            {
                this.StartupMethods.ForEach(x =>
                {
                    var method = StartupType.GetMethod(x, BindingFlags.Public | BindingFlags.Instance);
                    if (method != null)
                    {
                        var parameters = GenerateParameters(method.GetParameters()).ToArray();
                        method.Invoke(Startup, parameters);
                    }
                    else
                    {
                        Logger.LogWarning($"Startup method {x} not found");
                    }
                });
            }

            private List<object> GenerateParameters(ParameterInfo[] methodParameters)
            {
                var parameterValues = new List<object>();

                foreach (var methodParameter in methodParameters)
                {
                    if (!SupportedParameters.TryGetValue(methodParameter.ParameterType, out object value))
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

            private void StartLogging(string env)
            {
                Logger.LogInformation("Launching application '{ServiceCodeName}' in '{Environment}' mode.", this.ServiceCodeName, env);
                Logger.LogInformation("Assembly version: {Assembly}", Assembly.GetEntryAssembly().FullName);

                LogEnvironmentVariables();
            }
        }
    }
}