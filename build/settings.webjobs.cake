const string TriggeredWebJob = "triggered";
const string ContinuousWebJob = "continuous";

var getDotNetCoreWebJobPublishSettings = new Func<string, string, string, DotNetCorePublishSettings>((project, type, jobName) => new DotNetCorePublishSettings() {
    OutputDirectory = $"{ArtifactsDir}/apps/{project}/App_Data/jobs/{type}/{jobName}",
    Configuration =  @configuration
});