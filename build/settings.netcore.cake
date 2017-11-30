/**
 * File: settings.netcore.cake
 * Desc: CAKE settings for DotNetCore* Tasks
 * Author: mmisztal1980
 */

/**
 * dotnet restore
 */
var getDotNetCoreRestoreSettings  = new Func<DotNetCoreRestoreSettings>(() => new DotNetCoreRestoreSettings()
{
    DisableParallel = false,
});

/**
 * dotnet build
 */
var getDotNetCoreBuildSettings = new Func<DotNetCoreBuildSettings>(() => new DotNetCoreBuildSettings
{
    Configuration = @configuration,
});

/**
 * dotnet test
 */

// loggers
var trxLogger = new Func<string, string>((project) => $"--logger \"trx;LogFileName=../../results/{getProjectName(project)}.trx\"");

var getDotNetCoreTestSettings = new Func<string, string, DotNetCoreTestSettings>((project, testType) => new DotNetCoreTestSettings() {
    ArgumentCustomization = args => args
        .Append(trxLogger(project))
});

/**
 * dotnet publish
 */
var getDotNetCorePublishSettings = new Func<string, DotNetCorePublishSettings>((project) => new DotNetCorePublishSettings() {
    OutputDirectory = $"{ArtifactsDir}/apps/{getProjectName(project)}",
    Configuration = @configuration
});

/**
 * dotnet pack
 */
var getPackageVersionSuffix = new Func<string, string, string>((branchName, buildNo) => branchName == "dev" ? $"pre-{buildNo.Replace(".", string.Empty)}" : null);
var getPackageOutputDirectory = new Func<string, string>((branchName) => branchName == "dev" ? $"{ArtifactsDir}/packages/pre" : $"{ArtifactsDir}/packages/");
var getDotNetCorePackSettings = new Func<string, string, string, DotNetCorePackSettings>((project, branchName, buildNo) => new DotNetCorePackSettings() {
    OutputDirectory = getPackageOutputDirectory(branchName),
    VersionSuffix = getPackageVersionSuffix(branchName, buildNo)
});