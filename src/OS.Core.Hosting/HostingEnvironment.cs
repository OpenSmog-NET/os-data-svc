using System;

namespace OS.Core
{
    public static class HostingEnvironment
    {
        private const string VariableName = "ASPNETCORE_ENVIRONMENT";

        public static string Name => Environment.GetEnvironmentVariable(VariableName) ?? throw new InvalidOperationException("The environment has not been specified");

        public static bool IsDevelopment => Environment.GetEnvironmentVariable(VariableName)
            .Equals("development", StringComparison.InvariantCultureIgnoreCase);

        public static bool IsTest => Environment.GetEnvironmentVariable(VariableName)
            .Equals("test", StringComparison.InvariantCultureIgnoreCase);

        public static bool IsQa => Environment.GetEnvironmentVariable(VariableName)
            .Equals("qa", StringComparison.InvariantCultureIgnoreCase);

        public static bool IsProd => Environment.GetEnvironmentVariable(VariableName)
            .Equals("prod", StringComparison.InvariantCultureIgnoreCase);
    }
}