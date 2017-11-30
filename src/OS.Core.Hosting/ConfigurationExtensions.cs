using Microsoft.Extensions.Configuration;
using System;

namespace OS.Core
{
    public static class ConfigurationExtensions
    {
        public static IConfigurationBuilder AddWhen(this IConfigurationBuilder configuration, Func<bool> predicate, Action<IConfigurationBuilder> action)
        {
            if (predicate())
            {
                action(configuration);
            }

            return configuration;
        }
    }
}