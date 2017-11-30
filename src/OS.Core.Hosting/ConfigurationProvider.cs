using Microsoft.Extensions.Configuration;
using System;

namespace OS.Core
{
    public static class ConfigProvider
    {
        private const string AspNetPrefix = "ASPNETCORE_";
        private static string environment;

        public static IConfiguration Create(string configBasePath,
            string environmentVariablesPrefix = null,
            string[] args = null)
        {
            DetermineEnvironment(args);

            if (string.IsNullOrEmpty(environment))
            {
                throw new InvalidOperationException(
                    "No environment specified. Please make sure to either set enviornment variable " +
                    "ASPNETCORE_ENVIRONMENT or provide 'environment' command line argument.");
            }

            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(configBasePath)
                .AddJsonFile("appSettings.json", true)
                .AddJsonFile($"appSettings.{environment}.json", true)
                .AddEnvironmentVariables(prefix: AspNetPrefix);

            if (environmentVariablesPrefix != null)
            {
                configBuilder.AddEnvironmentVariables(prefix: environmentVariablesPrefix);
            }

            return configBuilder.Build();
        }

        public static string GetEnvironment()
        {
            if (string.IsNullOrEmpty(environment))
            {
                throw new InvalidOperationException("Please call 'Create()' before calling this method.");
            }

            return environment;
        }

        private static void DetermineEnvironment(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .AddEnvironmentVariables(prefix: AspNetPrefix);

            var config = builder.Build();

            environment = config["environment"];
        }
    }
}