namespace OS.Core.WebJobs
{
    public sealed class WebJobServiceHost : Host
    {
        public const string WebJobClassType = "WebJobClass";

        public WebJobServiceHost(string serviceCodeName)
            : base(serviceCodeName)
        {
        }

        protected override void RunHost()
        {
            var host = WebJobHostConfiguration.Configure(Setup.Configuration, ServiceProvider);
            host.CallAsync(Setup.RequiredTypes[WebJobClassType].GetMethod("RunAsync"));
            host.RunAndBlock();
        }
    }
}