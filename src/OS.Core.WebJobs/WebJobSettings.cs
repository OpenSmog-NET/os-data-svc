using Microsoft.Extensions.Configuration;

namespace OS.Core.WebJobs
{
    public sealed class WebJobSettings
    {
        private readonly IConfiguration configuration;

        public WebJobSettings(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string DashboardConnectionStringName { get; set; }
        public string StorageConnectionStringName { get; set; }

        public bool UseTimers { get; set; }

        public string DashboardConnectionString => configuration.GetConnectionString(DashboardConnectionStringName);

        public string StorageConnectionString => configuration.GetConnectionString(StorageConnectionStringName);
    }
}